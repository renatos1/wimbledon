using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Raposa : NetworkBehaviour
{

    private Rigidbody2D rb2d;
    private Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Awake () 
    {
        // anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {  

    }


    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        Debug.Log("Raposa:OnStartLocalPlayer - isLocalPlayer");
        Debug.Log(isLocalPlayer);
        Debug.Log("Raposa:OnStartLocalPlayer - hasAuthority");
        Debug.Log(hasAuthority);
        Debug.Log("Raposa:OnStartLocalPlayer - isClient");
        Debug.Log(isClient);
        Debug.Log("Raposa:OnStartLocalPlayer - isServer");
        Debug.Log(isServer);

        if (hasAuthority) {
            id = netId;
            Camera.main.GetComponent<SmoothCam2D>().target = transform;
        }

        if (!hasAuthority) {
            rb2d.isKinematic = true;
        }   

        // Turn off main camera because GamePlayer prefab has its own camera
        // GetComponentInChildren<Camera>().enabled = true;
        // Camera.main.enabled = false; 
    }

    [SyncVar] public float Health = 100f;

    public CharacterController2D controller;
    public float runSpeed = 2f;
    float horizontalMove = 0f;
    bool jump = false;
    public LayerMask whatIsChest;
    uint id;

    [SyncVar]
    Vector3 serverPosition;
    Vector3 serverPositionSmoothVelocity;
    


    // Update is called once per frame
    void Update()
    {
        if (isServer) {

        }

        if (hasAuthority) {
            AuthorityUpdate();
        }

    }

    void AuthorityUpdate() {

        // Horizontal moving
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetButtonDown("Jump")) {
            print("jumping");
            jump = true;
        }

        controller.Move(horizontalMove * Time.deltaTime, false, jump);
        jump = false;

        if (Input.GetKeyDown(KeyCode.E)) {
            print("Using");
            Collider2D[] chestsToOpen = Physics2D.OverlapBoxAll(transform.position, new Vector2(1f, 0.09f), 0, whatIsChest);
            print("chests found: ");
            print(chestsToOpen.Length);
            for (int i = 0; i < chestsToOpen.Length; i++) {
                chestsToOpen[i].GetComponent<Chest>().Open();
            }
        }

        // Updates the Player position on the server
        CmdUpdatePosition(transform.position);
    }


    public void TakeDamage(int damage, uint from) {
        CmdTakeDamage(damage, from);
    }

     IEnumerator Dead() {
        Debug.Log ("dead");
        GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(5);
        Debug.Log ("respawn");
        GetComponent<Renderer>().enabled = true;
     }

     IEnumerator DeadRaposa(Raposa raposa) {
        Debug.Log ("dead");
        raposa.GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(5);
        Debug.Log ("respawn");
        raposa.GetComponent<Renderer>().enabled = true;
     }


    [Command]
    void CmdUpdatePosition(Vector3 newPosition) {

        //TODO: Check to make surethis move is legal
        // If an illegal move is spotted, do something like:
        // RpcFixPosition( serverPosition )
        // and return

        serverPosition = newPosition;
    }

    [Command]
    public void CmdTakeDamage(int damage, uint from) {
        Debug.Log("Damage Taken");
        print(netId);
        Health -= damage;

        if (Health <= 0) {
            StartCoroutine(Dead());
        }
    }

    [Command]
    public void CmdDealDamage(int damage, uint from, uint to) {
        Debug.Log("Damage Taken From: " + from);
        Raposa raposa = NetworkIdentity.spawned[to].GetComponent<Raposa>();
        raposa.Health -= damage;
        Debug.Log("Raposa " + raposa.netId + " has " + raposa.Health + " health.");
        if (raposa.Health <= 0) {
            // StartCoroutine(DeadRaposa(raposa));
            RpcKillRaposa(to);
        }
        
    }

    [ClientRpc]
    void RpcKillRaposa(uint r) {
        StartCoroutine(DeadRaposa( NetworkIdentity.spawned[r].GetComponent<Raposa>()));
    }

    [ClientRpc]
    void RpcFixPosition(Vector3 newPosition) {
        transform.position = newPosition;
    }
}

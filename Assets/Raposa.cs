using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Raposa : NetworkBehaviour
{

    private Rigidbody2D rb2d;
    private Vector3 velocity = Vector3.zero;
    private GUIName nameTag;

    public Animator animator;

    // Use this for initialization
    void Awake () 
    {
        // anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        nameTag = GetComponent<GUIName>();
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
            // RpcSetName();
            Camera.main.GetComponent<SmoothCam2D>().target = transform;
        }

        if (!hasAuthority) {
            rb2d.isKinematic = true;
        }   

        // Turn off main camera because GamePlayer prefab has its own camera
        // GetComponentInChildren<Camera>().enabled = true;
        // Camera.main.enabled = false; 
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnHealthChanged(Health);
    }   

    [SyncVar(hook="OnHealthChanged")] public float Health = 100f;

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

	animator.SetFloat("Horizontal", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump")) {
            print("jumping");
            jump = true;
	    // animator.SetBoolean("IsJumping", true);
        }

        controller.Move(horizontalMove * Time.deltaTime, false, jump);
        jump = false;

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
        raposa.Health = 100;
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
        RpcDealDamage(damage, from, to);
    }

    [ClientRpc]
    void RpcDealDamage(int damage, uint from, uint to) {
        Debug.Log("Damage Taken From: " + from);
        Raposa raposa = NetworkIdentity.spawned[to].GetComponent<Raposa>();
        raposa.Health -= damage;
        Debug.Log("Raposa " + raposa.netId + " has " + raposa.Health + " health.");
        raposa.nameTag.text = raposa.netId.ToString() + " Health: " + raposa.Health.ToString();
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

    void UpdateName() {
        nameTag.text = netId.ToString() + " Health: " + Health.ToString();
    }

    void OnHealthChanged(float newValue)
{
        // Do something
        Health = newValue;
        nameTag.text = netId.ToString() + " Health: " + Health.ToString();
    }
}

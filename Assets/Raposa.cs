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
            Camera.main.GetComponent<SmoothCam2D>().target = transform;
        }

        // if (!hasAuthority) {
        //     rb2d.isKinematic = true;
        // }   

        // Turn off main camera because GamePlayer prefab has its own camera
        // GetComponentInChildren<Camera>().enabled = true;
        // Camera.main.enabled = false; 
    }


    public CharacterController2D controller;
    public float runSpeed = 2f;
    float horizontalMove = 0f;
    bool jump = false;

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

    void FixedUpdate() 
    {
        // if (hasAuthority) {
        //     AuthorityUpdate();    
        // }   
    }

    void AuthorityUpdate() {

        // Horizontal moving
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetButtonDown("Jump")) {
            print("jumping");
            jump = true;
        }

         if (Input.GetButtonDown("Fire1"))
         {
             print("attacking");
         }

        controller.Move(horizontalMove * Time.deltaTime, false, jump);
        jump = false;

        // Updates the Player position on the server
        CmdUpdatePosition(transform.position);
    }


    [Command]
    void CmdUpdatePosition(Vector3 newPosition) {

        //TODO: Check to make surethis move is legal
        // If an illegal move is spotted, do something like:
        // RpcFixPosition( serverPosition )
        // and return

        serverPosition = newPosition;
    }

    [ClientRpc]
    void RpcFixPosition(Vector3 newPosition) {
        transform.position = newPosition;
    }
}

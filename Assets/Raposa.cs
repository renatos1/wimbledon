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


    float Speed = 5;
    float jumpForce = 70;
    [HideInInspector] public bool jump = false;

    [SyncVar]
    Vector3 serverPosition;
    Vector3 serverPositionSmoothVelocity;

    // animate the game object from -1 to +1 and back
    public float minimum = -1.0F;
    public float maximum =  1.0F;

    // starting value for the Lerp
    static float t = 0.0f;


    // Update is called once per frame
    void Update()
    {
        if (isServer) {

        }

        // if (hasAuthority) {
        //     AuthorityUpdate();    
        // }   

        // if (!hasAuthority) {
        //     transform.position = Vector3.SmoothDamp(
        //         transform.position,
        //         serverPosition,
        //         ref serverPositionSmoothVelocity,
        //         0.25f
        //     );
        // }  
    }

    void FixedUpdate() 
    {
        if (hasAuthority) {
            AuthorityUpdate();    
        }   
    }

    void AuthorityUpdate() {

        if (Input.GetButtonDown("Jump"))
        {
            minimum = transform.position.y;
            maximum = transform.position.y + 2;
            jump = true;
        } 

        float h = Input.GetAxis("Horizontal");
        // float _j = 0;
        float movement = h * Speed * Time.fixedDeltaTime;


        //    if (Input.GetButtonDown("Jump")) {
        //     float movement = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
        //    }
        
        // transform.Translate(movement, _j, 0);
        // transform.position = Vector3.SmoothDamp(
        //         transform.position,
        //         new Vector3(movement, _j, 0),
        //         ref velocity,
        //         0.25f
        //     );

        // if (jump) {
        //     transform.Translate(Vector3.up * jumpForce * Time.deltaTime);
        //     jump = false;
        // }
        // rb2d.AddForce(Vector2.right * h * 100f);
        transform.Translate(movement, 0, 0);
        

        if (jump) {
            // animate the position of the game object...
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(minimum, maximum, t), 0);

            // .. and increase the t interpolater
            t += 0.9f * Time.fixedDeltaTime;

            // now check if the interpolator has reached 1.0
            // and swap maximum and minimum so game object moves
            // in the opposite direction.
            if (t > 1.0f)
            {   
                t=0.0f;
                jump=false;
            }
        }

        
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

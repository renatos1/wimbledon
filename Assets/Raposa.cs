using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Raposa : NetworkBehaviour
{

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

        // Turn off main camera because GamePlayer prefab has its own camera
        // GetComponentInChildren<Camera>().enabled = true;
        // Camera.main.enabled = false; 
    }


    float Speed = 5;

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

        if (!hasAuthority) {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                serverPosition,
                ref serverPositionSmoothVelocity,
                0.25f
            );
        }  
    }

    void AuthorityUpdate() {
        float movement = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;

        transform.Translate(movement, 0, 0);
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

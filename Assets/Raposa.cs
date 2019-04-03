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

    public override void OnStartClient()
    {
        Debug.Log("Raposa:OnClientConnect - isLocalPlayer");
        Debug.Log(isLocalPlayer);
        if (isLocalPlayer) {
            Camera.main.GetComponent<SmoothCam2D>().target = transform;
        } 
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        if (isServer) {
            SpawnRaposa();
        }

        Debug.Log("isLocalPlayer?");
        Debug.Log(isLocalPlayer);
        
        if (isLocalPlayer) {
            Camera.main.GetComponent<SmoothCam2D>().target = myRaposa.transform;
        }   
    }

    // public override void OnStartLocalPlayer()
    // {
    //     Debug.Log("OnStartLocalPlayer");
    //     Debug.Log("isLocalPlayer");
    //     Debug.Log(isLocalPlayer);
    //     Debug.Log("localPlayerAuthority");
    //     Debug.Log(localPlayerAuthority);

    //     if (isLocalPlayer) {
    //         Camera.main.GetComponent<SmoothCam2D>().target = myRaposa.transform; //Fix camera on "me"
    //     }

    // }

    public GameObject RaposaPrefab;
    GameObject myRaposa;

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SpawnRaposa()
    {

        // if( isServer == false )
        // {
        //     Debug.LogError("SpawnTank: Can only do what it's supposed to, from the SERVER!");
        //     return;
        // }

        // This gets called by the game manager when a new round starts
        // and a player needs a tank

        // Instantiate only creates the object on the LOCAL computer. It is
        // NOT sent to anyone else in the game
        myRaposa = Instantiate(RaposaPrefab);

        // The way to tell everyone to spawn the object in network:
        NetworkServer.SpawnWithClientAuthority(myRaposa, connectionToClient);
    }


}

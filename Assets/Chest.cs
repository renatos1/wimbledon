using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Chest : NetworkBehaviour
{

    Animator animator;
    public GameObject WeaponPrefab;

    [SyncVar] public bool isOpened = false;
    // Start is called before the first frame update
    void Start()
    {

        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Server]
    public void RpcOpen() {
        Debug.Log("RpcOpen: Executing RpcOpen on Chest: " + netId);
        if (!isOpened) {
            print("RpcOpen: Opening Chest");
            animator.SetBool("IsOpened", true);
            isOpened = true;
            GameObject wep = Instantiate(WeaponPrefab, transform.position, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(wep);
        } else {
            print("RpcOpen: Chest already opened");
        }
    }

}

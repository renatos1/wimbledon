using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Chest : NetworkBehaviour
{

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {

        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open() {
        print("Opening Chest");
        animator.SetBool("IsOpened", true);
    }
}

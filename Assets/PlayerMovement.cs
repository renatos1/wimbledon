using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public CharacterController2D controller;
    public float runSpeed = 40f;
    float horizontalMove = 0f;
    float horizontalMovement = 0f;
    public Animator animator;
    bool jump = false;
	// Update is called once per frame
	void Update () {
        horizontalMovement = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("Horizontal", horizontalMovement);
        horizontalMove = horizontalMovement * runSpeed;
        
        if (Input.GetButtonDown("Jump")) {
            jump = true;
        }
    }

    private void FixedUpdate() {
        
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;

    }
}

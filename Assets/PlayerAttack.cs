using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAttack : NetworkBehaviour
{

    private float timeBtwAttack;
    public float startTimeBtwAttack;

    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRangeX;
    public float attackRangeY;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
          
        if (timeBtwAttack <= 0) {
            // Then you can attack
            if (Input.GetKey(KeyCode.Mouse0))
            {
                print("attacking");
                Collider2D[] enemiesToDamage = Physics2D.OverlapBoxAll(attackPos.position, new Vector2(attackRangeX, attackRangeY), 0, whatIsEnemies);
                // print("enemies hit: ");
                // print(enemiesToDamage.Length);
                for (int i = 0; i < enemiesToDamage.Length; i++) {
                    if (netId != enemiesToDamage[i].GetComponent<Raposa>().netId) {
                        // TODO: Send a server call to damage the player so clients
                        // can't do this by themselves
                        enemiesToDamage[i].GetComponent<Raposa>().TakeDamage(damage);
                    }
                }

                timeBtwAttack = startTimeBtwAttack; 
            }
            
        } else {
            timeBtwAttack -= Time.deltaTime;
        }
         
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, new Vector3(attackRangeX, attackRangeY, 1));
    }
}

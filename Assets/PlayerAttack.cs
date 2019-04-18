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
    public LayerMask whatIsChest;
    public GameObject WeaponPrefab;
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
        
        if (hasAuthority) {
            if (timeBtwAttack <= 0) {
                // Then you can attack
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    print("Attacking");
                    Collider2D[] enemiesToDamage = Physics2D.OverlapBoxAll(attackPos.position, new Vector2(attackRangeX, attackRangeY), 0, whatIsEnemies);
                    // print("enemies hit: ");
                    // print(enemiesToDamage.Length);
                    for (int i = 0; i < enemiesToDamage.Length; i++) {
                        if (netId != enemiesToDamage[i].GetComponent<Raposa>().netId) {
                            // TODO: Send a server call to damage the player so clients
                            // can't do this by themselves
                            print("Attacker: " + netId + " dealing damage to receiver: " + enemiesToDamage[i].GetComponent<Raposa>().netId);
                            GetComponent<Raposa>().CmdDealDamage(damage, netId, enemiesToDamage[i].GetComponent<Raposa>().netId);
                            // enemiesToDamage[i].GetComponent<Raposa>().CmdTakeDamage(damage, netId);
                        }
                    }

                    timeBtwAttack = startTimeBtwAttack; 
                }
                
            } else {
                timeBtwAttack -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                Collider2D[] chestsToOpen = Physics2D.OverlapBoxAll(attackPos.position, new Vector2(attackRangeX, attackRangeY), 0, whatIsChest);
                print("chests found: ");
                print(chestsToOpen.Length);
                for (int i = 0; i < chestsToOpen.Length; i++) {
                    Chest c = chestsToOpen[i].GetComponent<Chest>();
                    CmdOpenChest(c.netId);
                }
            }
        }
    }

    [Command]
    void CmdOpenChest(uint chest) {
        Chest c = NetworkIdentity.spawned[chest].GetComponent<Chest>();
        if (!c.isOpened) {
            // GameObject wep = Instantiate(c.WeaponPrefab, c.transform.position, Quaternion.identity) as GameObject;
            // NetworkServer.Spawn(wep);
            print("CmdOpenChest: Opening Chest: " + c.netId);
            c.RpcOpen();
        }
    }


    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, new Vector3(attackRangeX, attackRangeY, 1));
    }
}

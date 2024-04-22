using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int attackPower = 10;
    public float damageCooldown = 0.5f;/*represents the duration between damage*/
    public float nextDamageTime = 0f;/*keeps track of the next time damage can be delt*/

    public PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay2D(Collision2D c)
    {
        if (c.collider.gameObject.tag == "Player" && Time.time >= nextDamageTime)
        {
            playerHealth.TakeDamage(attackPower);
            nextDamageTime = Time.time + damageCooldown;/*ensures that the enemy cannot deal damage until after a certail amount of time*/
        }
    }
}

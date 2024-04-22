using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurricaneSwordBehavior : MeleeWeaponBehavior
{
    List<GameObject> markedEnemies;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();
    }

    protected override void OnTriggerEnter2D(Collider2D c)
    {
        if (c.CompareTag("Enemy") && !markedEnemies.Contains(c.gameObject))//if the markedEnemies list contains the gameObject and it has not entered the collider
        {
            EnemyStats enemy = c.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position);

            markedEnemies.Add(c.gameObject);//mark the enemy so it doesnt take another instance of damage
        }
        else if (c.CompareTag("Prop"))
        {
            if (c.gameObject.TryGetComponent(out BreakableProps breakable) && !markedEnemies.Contains(c.gameObject))
            {
                breakable.TakeDamage(GetCurrentDamage());

                markedEnemies.Add(c.gameObject);
            }
        }
    }
}

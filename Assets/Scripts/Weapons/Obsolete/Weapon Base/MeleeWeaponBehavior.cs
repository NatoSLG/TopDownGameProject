using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponBehavior : MonoBehaviour
{
    public WeaponScriptableObject weaponData;//references stats

    public float destroyAfterSec;//Destorys the object after a certain amount of second

    //current stats
    protected float currentDamage;//stores base damage
    protected float currentSpeed;//stores base speed
    protected float currentWeaponCD;//stores weapon cooldown
    protected int currentPierce;//stores the max amount of time a weapon can hit an enemy before it gets destoryed

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentWeaponCD = weaponData.WeaponCD;
        currentPierce = weaponData.Pierce;
    }
    public float GetCurrentDamage()
    {
        return currentDamage *= FindObjectOfType<PlayerStats>().CurrentMight;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSec);
    }

    protected virtual void OnTriggerEnter2D(Collider2D c)
    {
        //references the script based on the collided collider and deals damage using TakeDamage() from the EnemyStats script
        if (c.CompareTag("Enemy"))
        {
            EnemyStats enemy = c.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position);
        }
        else if (c.CompareTag("Prop"))
        {
            if (c.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
            }
        }
    }
}

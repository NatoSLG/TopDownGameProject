using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This is being changed with the Weapon Class")]
public class ProjectileWeaponBehavior : MonoBehaviour
{
    public WeaponScriptableObject weaponData;//references stats

    protected Vector3 direction;//tracks the direction the weapon faces
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

    public void DirectionChecker(Vector3 d)//function to be called to check the direction the weapon should face
    {
        direction = d;

        float directionHorizontal = direction.x;
        float directionVertical = direction.y;

        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;

        // Reset scale to default
        scale.x = Mathf.Abs(scale.x);
        scale.y = Mathf.Abs(scale.y);

        if (directionHorizontal < 0 && directionVertical == 0)//flips the weapon to the left
        {
            scale.x *= -1;
            scale.y *= -1;
        }
        if (directionHorizontal == 0 && directionVertical > 0)//flips the weapon up
        {
            scale.x *= -1;
            scale.y *= 1;
        }
        if (directionHorizontal == 0 && directionVertical < 0)//flips the weapon down
        {
            scale.x *= 1;
            scale.y *= -1;
        }
        if (directionHorizontal > 0 && directionVertical > 0)//flips the weapon right-up
        {
            rotation.z = 0f;
        }
        else if (directionHorizontal > 0 && directionVertical < 0)//flips the weapon right-down
        {
            rotation.z = -90f;
        }
        else if (directionHorizontal < 0 && directionVertical > 0)//flips the weapon left-up
        {
            scale.x *= -1;
            scale.y *= -1;
            rotation.z = -90f;
        }
        else if (directionHorizontal < 0 && directionVertical < 0)//flips the weapon right-down
        {
            scale.x *= -1;
            scale.y *= -1;
            rotation.z = 0f;
        }
        else
        {
            rotation.z = -45f;
        }

        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation);//using Quaternion.Euler() because you cannot conver Quaternion to a Vector3
    }

    protected virtual void OnTriggerEnter2D(Collider2D c)
    {
        //references the script based on the collided collider and deals damage using TakeDamage() from the EnemyStats script
        if (c.CompareTag("Enemy"))
        {
            EnemyStats enemy = c.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position);
            ReducedPierce();
        }
        else if (c.CompareTag("Prop"))
        {
            if (c.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
                ReducedPierce() ;
            }
        }
    }

    void ReducedPierce()
    {
        //destroys the weapon object once reaching 0
        currentPierce--;
        if (currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}

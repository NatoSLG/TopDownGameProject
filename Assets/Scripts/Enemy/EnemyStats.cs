using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]//prevents the ability to attach the script to a game object that does not have a sprite randerer
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    //enemy stats
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentDamage;

    Transform player;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1, 0, 0, 0.5f); //creates a color of the damage flash when the enemy takes damage
    public float damageFlashDuration = 0.2f; //the length of the flash
    public float deathFadeTime = 0.6f; //length of the fade animation when an enemy dies
    Color originalColor;

    public ParticleSystem damageEffect;

    SpriteRenderer sr;
    EnemyMovement movement;

    public static int count;

    void Awake()
    {
        count++;

        currentHealth = enemyData.MaxHealth;
        currentMoveSpeed = enemyData.Speed;
        currentDamage = enemyData.Damage;
    }

    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color; //get the sprites original color
        movement = GetComponent<EnemyMovement>();
    }

    public void TakeDamage(float damage, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= damage;
        StartCoroutine(DamageFlash());

        //creates particle effect if damage is taken
        if (damageEffect)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }
        
        //creates text popup when damage is taken
        if (damage > 0) 
        {
            GameManager.GenerateFloatingText(Mathf.FloorToInt(damage).ToString(), transform);
        }

        //apply knockback
        if (knockbackForce > 0)
        {
            //direction of knockback
            Vector2 direction = (Vector2)transform.position - sourcePosition;
            movement.Knockback(direction.normalized * knockbackForce, knockbackDuration);
        }

        //Kill enemy
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    //using coroutine to making enemy flash when taking damage
    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }
    
    public void Kill()
    {
        /*Enables drops if enemy is killed,
         since drops are disabled by default.*/
        DropRateManager drops = GetComponent<DropRateManager>();
        if (drops)
        {
            drops.active = true;
        }

        StartCoroutine(KillFade());
    }

    //create coroutine to fade the enemy away on death and destroy
    IEnumerator KillFade()
    {
        //waits a single frame
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float originalAlpha = sr.color.a;

        //loop that fires every frame
        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            //set color for frame
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * originalAlpha);
        }

        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            PlayerStats player = c.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
        }
    }

    void OnDestroy()
    {
        count--;
    }
}

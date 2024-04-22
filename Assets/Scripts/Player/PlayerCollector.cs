using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;

    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        playerCollector.radius = player.CurrentMagnet;
    }

    void OnTriggerStay2D(Collider2D c)
    {
        //uses TryGetComponent check if the component has the ICollectable function on it
        if (c.gameObject.TryGetComponent(out ICollectable collectible))
        {
            //pulling animation when collecting items
            Rigidbody2D rb = c.gameObject.GetComponent<Rigidbody2D>();

            //creates a vector from the item to the player and applies forces to the object
            Vector2 forceDirection = (transform.position - c.transform.position).normalized;
            rb.AddForce(forceDirection * pullSpeed);

            collectible.Collect();
        }
    }
}

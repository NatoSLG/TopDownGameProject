using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D detector;
    public float pullSpeed;

    void Start()
    {
        player = GetComponentInParent<PlayerStats>();
    }

    public void SetRadius(float r)
    {
        if (!detector)
        {
            detector = GetComponent<CircleCollider2D>();
        }
        detector.radius = r;
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        //uses TryGetComponent check if the component is a Pickup
        if (c.TryGetComponent(out Pickup p))
        {
            //if it does, call Collect
            p.Collect(player, pullSpeed);
        }
    }
}

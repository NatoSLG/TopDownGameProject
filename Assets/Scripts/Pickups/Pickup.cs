using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, ICollectable
{
    public bool hasBeenCollected = false;

    public virtual void Collect()
    {
        hasBeenCollected = true;
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        //destorys object once reaching the player
        if (c.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    public float frequency; //speed of movement
    public float magnitude; //range of movement
    public Vector3 direction; //direction of movement
    Vector3 initialPosition; //original position of game object before applying effect
    Pickup pickup;

    void Start()
    {
        pickup = GetComponent<Pickup>();

        initialPosition = transform.position; //saves the starting position of the game object
    }

    void Update()
    {
        //check to ensure that animation only takes effect when the game object is in a collectable state
        if (pickup && !pickup.hasBeenCollected)
        {
            transform.position = initialPosition + direction * Mathf.Sin(Time.time * frequency) * magnitude; //applies sine function to create smooth bobbing effect
        }
    }
}

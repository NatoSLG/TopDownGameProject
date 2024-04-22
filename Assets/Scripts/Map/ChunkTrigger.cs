using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTrigger : MonoBehaviour
{
    MapController mc;
    public GameObject targetChunk; //used to change the current chunk

    // Start is called before the first frame update
    void Start()
    {
        mc = FindObjectOfType<MapController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //sets the current chunk to be the target chunk if the player if within the boundaries of the chunk area
    private void OnTriggerStay2D(Collider2D c)
    {
        if (c.CompareTag("Player"))
        {
            mc.currentChunk = targetChunk;
        }
    }

    //sets the current chunk to be null if the player exits the chunk area
    private void OnTriggerExit2D(Collider2D c)
    {
        if (c.CompareTag("Player"))
        {
            if(mc.currentChunk == targetChunk)
            {
                mc.currentChunk = null;
            }
        }
    }
}

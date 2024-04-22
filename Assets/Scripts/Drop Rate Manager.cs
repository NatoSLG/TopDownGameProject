using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }

    public List<Drops> drops;

    void OnDestroy()
    {
        //checks if the current scene is loaded
        if (!gameObject.scene.isLoaded)
        {
            return;
        }    

        //creates a randome % between 0 and 100
        float randomNumber = UnityEngine.Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops> ();

        foreach (Drops rate in drops)
        {
            if (randomNumber <= rate.dropRate) 
            {
                possibleDrops.Add (rate);
            }
        }
        //check the amount of possible drops and if there are more than 1 drop that can drop at the same time, roll to see which one will drop
        if (possibleDrops.Count > 0) 
        {
            Drops drops = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)];
            Instantiate(drops.itemPrefab, transform.position, Quaternion.identity);
        }
    }
}

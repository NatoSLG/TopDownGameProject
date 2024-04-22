using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnPoints;
    public List<GameObject> propPrefab;

    // Start is called before the first frame update
    void Start()
    {
        spawnProps();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnProps()
    {
        foreach (GameObject sp in propSpawnPoints)
        {
            int rand = Random.Range(0, propPrefab.Count);
            GameObject prop = Instantiate(propPrefab[rand], sp.transform.position, Quaternion.identity);
            prop.transform.parent = sp.transform;
        }
    }
}

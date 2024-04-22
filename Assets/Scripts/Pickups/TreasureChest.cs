using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    InventoryManager inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<InventoryManager>();
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            OpenTreasureChest();
            Destroy(gameObject);
        }
    }

    public void OpenTreasureChest()
    {
        if (inventory.GetPossibleEvolutions().Count <= 0)
        {
            Debug.Log("No Available Evolutions");
            return;
        }

        WeaponEvolutionBlueprint toEvolve = inventory.GetPossibleEvolutions()[Random.Range(0, inventory.GetPossibleEvolutions().Count)];

        inventory.EvolveWeapon(toEvolve); //randomly chooses evolution if there are more than 1 choice

    }
}

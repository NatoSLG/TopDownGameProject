using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStats;

public class TreasureChest : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D c)
    {
        PlayerInventoy p = c.GetComponent<PlayerInventoy>();
        
        //evolve weapon
        if (p)
        {
            bool randomBool = Random.Range(0, 2) == 0;

            OpenTreasureChest(p, randomBool);
            Destroy(gameObject);
        }
    }

    public void OpenTreasureChest(PlayerInventoy inventory, bool isHigherTier)
    {
        bool evolutionAttempted = false; // Flag to track if any evolution attempt was made
        PlayerStats ps = FindObjectOfType<PlayerStats>();

        //loop through every weapon to check if it can evolve
        foreach (PlayerInventoy.Slot s in inventory.weaponSlots)
        {
            Weapon w = s.item as Weapon; //if s.item is a weapon, w will hold the weapon, otherwise it will return null

            if (w == null || w.data == null || w.data.evolutionData == null)
            {
                continue; // Ignore weapon if it cannot evolve
            }

            //loop through every possible evolution of the weapon
            foreach (ItemData.Evolution e in w.data.evolutionData)
            {
                //only attempt to evolve weapons by treasure chest
                if (e.condition == ItemData.Evolution.Condition.treasureChest)
                {
                    Debug.Log(w.name + " | " + s.item + " | " + e.condition);
                    bool attempt = w.AttemptEvolution(e, 0);
                    
                    //if evolution suceeds
                    if (attempt) 
                    {
                        evolutionAttempted = true; //set flag to true
                        return;
                    }
                }
            }
        }
        //Open level up screen if weapon cannot evolve
        if (ps != null && !evolutionAttempted) 
        {
            ps.level++;//level up player
            ps.experience = 0;
            
            //Determine the players experience cap for the next level
            int experienceCapIncrease = 0;
            if (ps.levelRanges != null)
            {
                foreach (LevelRange range in ps.levelRanges)
                {
                    if (ps.level >= range.startLevel && ps.level <= range.endLevel)
                    {
                        experienceCapIncrease = range.experienceCapIncrease;
                        break;
                    }
                }
            }
               
            ps.experienceCap += experienceCapIncrease; //increase the players experience cap for the next level

            ps.expBar.fillAmount = (float)ps.experience / ps.experienceCap;//update exp bar fill amount
            ps.UpdateLevelText();
            GameManager.instance.StartLevelUp();
        }
    }
}

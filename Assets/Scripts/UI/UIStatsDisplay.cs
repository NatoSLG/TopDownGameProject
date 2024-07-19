using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Reflection;
using TMPro;

public class UIStatsDisplay : MonoBehaviour
{
    public PlayerStats player; //reference players stats
    public bool displayCurrentHealth = false;
    public bool updateInEditor = false;
    TextMeshProUGUI statNames, statValues;

    //Update this stat display whenever it is set to be active
    void OnEnable()
    {
        UpdateStatFields();
    }

    //QOL to allow the text to fill in editor
    void OnDrawGizmosSelected()
    {
        if (updateInEditor)
        {
            UpdateStatFields();
        }
    }

    public void UpdateStatFields()
    {
        if (!player)
        {
            return;
        }

        //reference both text object to render stat names and values
        if (!statNames)
        {
            statNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        if (!statValues) 
        {
            statValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        /*render all stat names and values.
        Use StringBuilders so that the string manipulation runs faster*/
        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();

        //Add the current health to the stat box
        if (displayCurrentHealth) 
        {
            names.AppendLine("Health");
            values.AppendLine(player.CurrentHealth.ToString());
        }

        FieldInfo[] fields = typeof(CharacterData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields) 
        {
            //Render stat names
            names.AppendLine(field.Name);

            //get stat values
            object val = field.GetValue(player.Stats);
            float fval = val is int ? (int)val : (float)val;

            //print is as a percentage if it has an attribute assigned and is a float
            PropertyAttribute attribute = (PropertyAttribute)PropertyAttribute.GetCustomAttribute(field, typeof(PropertyAttribute));

            if (attribute != null && field.FieldType == typeof(float))
            {
                float percentage = Mathf.Round(fval * 100 - 100);

                //if the stat is 0, put a dash
                if (Mathf.Approximately(percentage, 0))
                {
                    values.Append('-').Append('\n');
                }
                else
                {
                    if (percentage > 0)
                    {
                        values.Append('+');
                    }
                    values.Append(percentage).Append('%').Append('\n');
                }
            }
            else
            {
                values.Append(fval).Append('\n');
            }

            //Updates the fields with the strings we built
            statNames.text = PrettifyNames(names);
            statValues.text = values.ToString();
        }
    }

    public static string PrettifyNames(StringBuilder input)
    {
        //return an empty string if the StringBuilder is empty
        if (input.Length <= 0)
        {
            return string.Empty;
        }

        StringBuilder result = new StringBuilder();
        char last = '\0';

        for (int i = 0; i < input.Length; i++) 
        {
            char c = input[i];

            //check when to uppercase or add spaces to a character
            if (last == '\0' || char.IsWhiteSpace(last)) 
            {
                c = char.ToUpper(c); //capitalize the character
            }
            else if (char.IsUpper(c))
            {
                result.Append(' '); //insert a space before the capital letter
            }
            result.Append(c);

            last = c;
        }
        return result.ToString();
    }

    //QOL to auto assign the player if something happens
    void Reset()
    {
        player = FindObjectOfType<PlayerStats>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    WeaponData weaponData;
    string[] weaponSubtypes;
    int selectedWeaponSubtype;

    //stores information on the different types of weapons
    void OnEnable()
    {
        //cache the weapon data value
        weaponData = (WeaponData)target;

        //Retireve all the weapon subtypes and store it for reference
        System.Type baseType = typeof(Weapon);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
            .ToList();

        //Add a none option in front
        List<string> subTypesString = subTypes.Select(t => t.Name).ToList();
        subTypesString.Insert(0, "None");
        weaponSubtypes = subTypesString.ToArray();

        //ensure that we are using the correct weapon subtype
        selectedWeaponSubtype = Math.Max(0, Array.IndexOf(weaponSubtypes, weaponData.behaviour));
    }

    //creates a drop down menu to access the stored information
    public override void OnInspectorGUI()
    {
        //draw a dropdown in the inspector
        selectedWeaponSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, selectedWeaponSubtype), weaponSubtypes);

        if (selectedWeaponSubtype > 0)
        {
            //updates the behvaiour field
            weaponData.behaviour = weaponSubtypes[selectedWeaponSubtype].ToString(); //save inside the behaviour field
            EditorUtility.SetDirty(weaponData); //Marks the object to save
            DrawDefaultInspector(); // draw the default inspector elements
        }
    }
}

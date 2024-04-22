using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTransform : MonoBehaviour
{
    public Transform objectTransform;

    // Start is called before the first frame update
    void Start()
    {
        objectTransform.eulerAngles = new Vector3(45, 0, 0);
    }
}

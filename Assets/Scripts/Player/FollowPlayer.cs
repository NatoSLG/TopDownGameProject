using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Vector3 screen;
    public Transform player;//reference to targeting player

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + screen;
        //CameraTurn();
    }

    //void CameraTurn()
    //{
    //    float roatateDir = 0f;
    //
    //    if (Input.GetKey(KeyCode.Q))
    //    {
    //        roatateDir = +1f;
    //    }
    //
    //    if (Input.GetKey(KeyCode.E))
    //    {
    //        roatateDir = -1f;
    //    }
    //
    //    transform.eulerAngles += new Vector3();
    //}
}

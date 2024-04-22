using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    //References
    Animator am;
    PlayerMovement pm;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pm.movement.x != 0 || pm.movement.y != 0)
        {
            am.SetBool("Move", true);

            SpriteDirectionTracker();
        }
        else
        {
            am.SetBool("Move", false);
        }
    }

    void SpriteDirectionTracker()
    {
        if (pm.lastHorizontalVector < 0)
        {
            //flips sprits facing left
            sr.flipX = true;
        }
        else
        {
            //keeps sprite facing right or flips sprite to face right
            sr.flipX = false;
        }
    }
}

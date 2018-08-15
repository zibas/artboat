using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiimoteOarsTest : MonoBehaviour
{

    public WiimoteOars oars;

    public Animator oar1Animator;

    // Use this for initialization
    void Start()
    {
        oars.Initialize();
    }

    // Update is called once per frame
    void Update()
    {

        if (oars.oars[0].isPaddling)
        {
            switch (oars.oars[0].direction)
            {
                case WiimoteOars.DIRECTIONS.FORWARDS:
                    oar1Animator.SetBool("Forwards", true);
                    oar1Animator.SetBool("Backwards", false);
                    break;
                case WiimoteOars.DIRECTIONS.BACKWARDS:
                    oar1Animator.SetBool("Forwards", false);
                    oar1Animator.SetBool("Backwards", true);
                    break;
            }
        }
        else
        {
            oar1Animator.SetBool("Forwards", false);
            oar1Animator.SetBool("Backwards", false);

        }
    }

}

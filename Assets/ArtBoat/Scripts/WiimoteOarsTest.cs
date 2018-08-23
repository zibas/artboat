using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiimoteOarsTest : MonoBehaviour
{

    public WiimoteOars oars;

    public Animator oar1Animator;

    public UnityEngine.UI.Text text;

    // Use this for initialization
    void Start()
    {
        oars.Initialize();
    }

    // Update is called once per frame
    void Update()
    {

            string s = "Oars:\n";
            foreach(var oar in oars.oars)
            {
                s += "\nOar: " + oar.index + ": ";
                if (oar.inWater)
                {
                    s += "In Water ";
                }
                if (oar.isPaddling)
                {
                    s += "Paddling " + oar.direction;
                }

              
            }

            text.text = s;

    }

}

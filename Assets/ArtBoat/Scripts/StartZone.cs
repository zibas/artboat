using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour {
    private void OnTriggerExit(Collider other)
    {
        Rigidbody otherBody = other.attachedRigidbody;
        if(otherBody != null)
        {
            BoatController boat = otherBody.GetComponent<BoatController>();

            if(boat != null)
            {
                boat.riverForceEnabled = true;

                UI ui = FindObjectOfType<UI>();

                if(ui != null)
                {
                    ui.StartTime();
                }
            }
        }
    }
}

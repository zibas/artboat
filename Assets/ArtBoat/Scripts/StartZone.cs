using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour {
    private void OnTriggerExit(Collider other)
    {
        UI ui = FindObjectOfType<UI>();
        ui.StartTime();
    }
}

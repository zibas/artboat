using UnityEngine;

public class BoatCam : MonoBehaviour {
    [SerializeField] GameObject Boat;
    [SerializeField] Vector3 Offset;

    void Start() {
        UpdatePosition();
    }
    
    void Update() {
        UpdatePosition();
    }

    void UpdatePosition() {
        var riverPos = Boat.transform.position;
        riverPos.x = 0;
        transform.position = riverPos + Offset;
    }
}
using UnityEngine;

public class OarAnimator : MonoBehaviour {
    [SerializeField] Transform BackLeft;
    [SerializeField] Transform FrontLeft;
    
    [SerializeField] Transform BackRight;
    [SerializeField] Transform FrontRight;

    [SerializeField] float backStrokeRotation = 30;
    [SerializeField] float frontStrokeRotation = -30;

    void Update() {
        FrontLeft.localRotation = BackLeft.localRotation = Quaternion.Euler(
            0, Input.GetKey(KeyCode.LeftArrow) ? frontStrokeRotation : backStrokeRotation, 0);
        
        FrontRight.localRotation = BackRight.localRotation = Quaternion.Euler(
            0, Input.GetKey(KeyCode.RightArrow) ? -frontStrokeRotation : -backStrokeRotation, 0);
    }
}
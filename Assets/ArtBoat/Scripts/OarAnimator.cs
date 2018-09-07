using UnityEngine;

public class OarAnimator : MonoBehaviour
{
    [SerializeField] Transform BackLeft;
    [SerializeField] Transform FrontLeft;

    [SerializeField] Transform BackRight;
    [SerializeField] Transform FrontRight;

    [SerializeField] float backStrokeRotation = 30;
    [SerializeField] float noStrokeRotation = 0;
    [SerializeField] float frontStrokeRotation = -30;

    public WiimoteOars oars;

    public Animator animator;

    void Update()
    {
        if (oars.totalRemotes > 0)
        {
            int leftSide = 0;
            int rightSide = 0;
           
            
                foreach (var oar in oars.oars)
                {
                    if (oar.isPaddling)
                    {
                        if (oar.direction == WiimoteOars.DIRECTIONS.FORWARDS)
                        {
                            if (oar.side == WiimoteOars.Oar.SIDES.LEFT)
                            {
                                leftSide = 1;
                            }
                            else
                            {
                                rightSide = 1;
                            }
                        }
                        else
                        {
                            if (oar.side == WiimoteOars.Oar.SIDES.LEFT)
                            {
                                leftSide = -1;
                            }
                            else
                            {
                                rightSide = -1;
                            }
                        }
                    }
                }
            animator.SetInteger("Left Direction", leftSide);
            animator.SetInteger("Right Direction", rightSide);

            //  FrontLeft.localRotation = BackLeft.localRotation = Quaternion.Euler(0, leftSide, 0);
            //  FrontRight.localRotation = BackRight.localRotation = Quaternion.Euler(0, rightSide, 0);
        }
        else // just keyboard
        {
            FrontLeft.localRotation = BackLeft.localRotation = Quaternion.Euler(0, Input.GetKey(KeyCode.LeftArrow) ? frontStrokeRotation : backStrokeRotation, 0);

            FrontRight.localRotation = BackRight.localRotation = Quaternion.Euler(0, Input.GetKey(KeyCode.RightArrow) ? -frontStrokeRotation : -backStrokeRotation, 0);
        }
    }


}
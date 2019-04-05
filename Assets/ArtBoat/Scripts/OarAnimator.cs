using UnityEngine;

public class OarAnimator : MonoBehaviour
{
    [SerializeField]
    Transform BackLeft;
    [SerializeField]
    Transform FrontLeft;

    [SerializeField]
    Transform BackRight;
    [SerializeField]
    Transform FrontRight;


    public WiimoteOars oars;

    public Animator animator;

    void Update()
    {
        int leftSide = 0;
        int rightSide = 0;
        if (oars.totalRemotes > 0)
        {



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
                        else if
                            (oar.side == WiimoteOars.Oar.SIDES.RIGHT)
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
                        else if
                            (oar.side == WiimoteOars.Oar.SIDES.RIGHT)
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
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                leftSide = 1;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                rightSide = 1;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                leftSide = -1;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                rightSide = -1;
            }
            animator.SetInteger("Left Direction", leftSide);
            animator.SetInteger("Right Direction", rightSide);

        }
    }


}
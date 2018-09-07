using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [SerializeField] float MaxSpeed = 4;
    [SerializeField] float MaxRotation = 45;

    [SerializeField] float TorquePerStroke = 5;
    [SerializeField] float SpeedPerStroke = 1;
    [SerializeField] float VelocityDamping = 0.01f;
    [SerializeField] float RotationDamping = 0.01f;
    [SerializeField] float BackwardsSpeedMultiplier = 0.25f;
    [SerializeField] float BackwardsTorqueMultiplier = 1.5f;

    [SerializeField] Vector3 RiverFlowForce = new Vector3(0f, 0f, -0.47f);

    [Tooltip("Velocity will be multiplied by this value on a non-powerup collision (ie, how much to slow down on hitting a wall)")]
    [SerializeField] float BaseCollisionVelocityMultiplier = 0.75f;

    float currentRotation;
    float currentTorque;
    float currentSpeed;

    public float updateDelay = 0.1f;
    float inputClock = 0;

    public WiimoteOars oars;

    public float keyboardAmplification = 10;

    float findClock = 0;
    float findInterval = 1;

    //we only want the river to add force after we're past the starting line
    [HideInInspector]
    public bool riverForceEnabled = false;

    public void Start()
    {
        oars.Initialize();
    }

    void Update()
    {
        findClock += Time.deltaTime;
        if(findClock >= findInterval)
        {
            Wii.StartSearch();
            findClock = 0;
        }
        inputClock += Time.deltaTime;
        if (inputClock > -updateDelay)
        {
            foreach (var oar in oars.oars)
            {            

                if (oar.isPaddling)
                {
                    if (oar.direction == WiimoteOars.DIRECTIONS.FORWARDS)
                    {
                        if (oar.side == WiimoteOars.Oar.SIDES.RIGHT)
                        {
                            currentTorque -= TorquePerStroke;

                        }
                        else
                        {
                            currentTorque += TorquePerStroke;

                        }
                        currentSpeed += SpeedPerStroke;
                    }
                    else
                    {
                        if (oar.side == WiimoteOars.Oar.SIDES.RIGHT)
                        {
                            currentTorque += TorquePerStroke * BackwardsTorqueMultiplier;

                        }
                        else
                        {
                            currentTorque -= TorquePerStroke * BackwardsTorqueMultiplier;

                        }
                        currentSpeed -= SpeedPerStroke * BackwardsSpeedMultiplier;
                    }
                }

            }

            bool right = Input.GetKeyUp(KeyCode.LeftArrow);
            bool left = Input.GetKeyUp(KeyCode.RightArrow);

            if (left) { currentTorque -= TorquePerStroke * keyboardAmplification; }
            if (right) { currentTorque += TorquePerStroke * keyboardAmplification; }

            if (left) { currentSpeed += SpeedPerStroke * keyboardAmplification; }
            if (right) { currentSpeed += SpeedPerStroke * keyboardAmplification; }

            currentSpeed = Mathf.Clamp(currentSpeed, -MaxSpeed, MaxSpeed);
        }
    }

    void FixedUpdate()
    {
        currentSpeed = Mathf.Clamp(currentSpeed * (1f - VelocityDamping), -MaxSpeed, MaxSpeed);

        var position = transform.position + transform.forward * -currentSpeed * Time.fixedDeltaTime;

        currentTorque -= currentTorque * RotationDamping;
        currentRotation = currentRotation + currentTorque * Time.fixedDeltaTime;
        currentRotation = Mathf.Clamp(currentRotation, -MaxRotation, MaxRotation);

        var rigidBody = GetComponent<Rigidbody>();
        rigidBody.velocity = transform.forward * -currentSpeed;
        if(riverForceEnabled)
        {
            rigidBody.velocity += RiverFlowForce;
        }
        rigidBody.MoveRotation(Quaternion.Euler(0, currentRotation, 0));
    }

    // I: Slow the boat down if it hits a non-powerup:
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Pickup")
        {
            currentSpeed *= BaseCollisionVelocityMultiplier;
            Debug.Log("Hit!");
        }
    }

}
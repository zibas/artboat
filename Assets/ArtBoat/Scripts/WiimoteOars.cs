using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WiimoteOars : MonoBehaviour
{
    public Oar[] oars;

    public float speedMultiplier = 1;

    public float individualAxisThreshold = 0.075f;
    public float maxYAccelerationDeltaForRotation = 0.1f;

    public float inWaterSpeedThreshold = 1;

    public enum DIRECTIONS { FORWARDS, BACKWARDS };

    public int totalRemotes = 0;

    List<int> activeWiimotes = new List<int>();

    public float angleToBeInWater = -0.2f;


    public int paddleStateFramesToAverage = 20;
    [System.Serializable]
    public class Oar
    {
        public enum SIDES { RIGHT, LEFT, MASTER };
        public SIDES side = SIDES.RIGHT;
        public int index = 0;
        public DIRECTIONS direction;
        public Vector3 acceleration;
        public float angle;
        public bool enabled = false;
        public float speed = 0;
        public bool inWater = false;
        public float smoothSpeed;
        public float smoothTime = 0.3F;
        public float yVelocity = 0.0F;
        public bool isRotating = false;
        public bool isPaddling = false;
        public Queue<int> paddlingStateQueue = new Queue<int>();

        public bool GetButtonDown(string name)
        {
            return Wii.GetButtonDown(index, name);

        }

    }

    void OnEnable()
    {
        Wii.OnDiscoveryFailed += OnDiscoveryFailed;
        Wii.OnWiimoteDiscovered += OnWiimoteDiscovered;
        Wii.OnWiimoteDisconnected += OnWiimoteDisconnected;
    }

    void OnDisable()
    {
        Wii.OnDiscoveryFailed -= OnDiscoveryFailed;
        Wii.OnWiimoteDiscovered -= OnWiimoteDiscovered;
        Wii.OnWiimoteDisconnected -= OnWiimoteDisconnected;
    }

    public void BeginSearch()
    {
        //searching = true;
        Wii.StartSearch();
        Debug.Log("I'm looking.");
        Time.timeScale = 1.0f;
    }

    public void OnDiscoveryFailed(int i)
    {
        Debug.Log("Error:" + i + ". Try Again.");
    }

    public void OnWiimoteDiscovered(int thisRemote)
    {
        Debug.Log("found this one: " + thisRemote);
        activeWiimotes.Add(thisRemote);
    }

    public void OnWiimoteDisconnected(int whichRemote)
    {
        Debug.Log("lost this one: " + whichRemote);
    }

    public void Initialize(int count = 4)
    {
        BeginSearch();
        oars = new Oar[count];
        for (int i = 0; i < count; i++)
        {
            oars[i] = new Oar();
            oars[i].index = i;
            switch (i)
            {
                case 0:
                    oars[i].side = Oar.SIDES.LEFT;
                    break;
                case 1:
                    oars[i].side = Oar.SIDES.RIGHT;
                    break;
                default:
                    oars[i].side = Oar.SIDES.MASTER;
                    break;
            }

        }

    }

    public void FixedUpdate()
    {
        totalRemotes = Wii.GetRemoteCount();

        for (int i = 0; i < 4; i++)
        {
            if (Wii.IsActive(i))
            {
                Vector3 wiiAccel = Wii.GetWiimoteAcceleration(i);
                ParseData(oars[i], wiiAccel);
            }
        }
    }

    private void ParseData(Oar oar, Vector3 acceleration)
    {
        Vector3 accelerationDelta = (oar.acceleration - acceleration) * Time.deltaTime;
        oar.acceleration = acceleration;
        oar.inWater = acceleration.y < angleToBeInWater;

        oar.isRotating = Mathf.Abs(accelerationDelta.x) > individualAxisThreshold && Mathf.Abs(accelerationDelta.z) > individualAxisThreshold;


        float radius = Mathf.Sqrt(oar.acceleration.x * oar.acceleration.x + oar.acceleration.z * oar.acceleration.z);
        float angle = Mathf.Atan2(oar.acceleration.z, oar.acceleration.x) * Mathf.Rad2Deg;
        float angleDelta = (oar.angle - angle) * Time.deltaTime;
        oar.angle = angle;

        if (oar.isRotating)
        {
            // Probably crossed the line between 180 and -180
            if (Mathf.Abs(angleDelta) > 1)
            {
                oar.speed = 0;
            }
            else
            {
                oar.speed = angleDelta * speedMultiplier;
            }


        }
        else
        {
            oar.speed = 0;
        }

        oar.smoothSpeed = Mathf.SmoothDamp(oar.smoothSpeed, oar.speed, ref oar.yVelocity, oar.smoothTime);

        if (oar.inWater)
        {
            if (oar.side == Oar.SIDES.RIGHT)
            {
                oar.direction = oar.smoothSpeed > 0 ? DIRECTIONS.BACKWARDS : DIRECTIONS.FORWARDS;

            }
            else if (oar.side == Oar.SIDES.LEFT)
            {
                oar.direction = oar.smoothSpeed < 0 ? DIRECTIONS.BACKWARDS : DIRECTIONS.FORWARDS;

            }
        }

        oar.paddlingStateQueue.Enqueue(oar.inWater && oar.isRotating ? 1 : 0);
        if (oar.paddlingStateQueue.Count > paddleStateFramesToAverage)
        {
            // relies on fixed update for speed consistency
            oar.paddlingStateQueue.Dequeue();
        }
        oar.isPaddling = oar.paddlingStateQueue.Average() > 0.5f;

    }

}

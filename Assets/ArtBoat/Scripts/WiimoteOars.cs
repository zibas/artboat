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

    int totalRemotes = 0;

    List<int> activeWiimotes = new List<int>();


    public int paddleStateFramesToAverage = 20;
    [System.Serializable]
    public class Oar
    {
        public bool leftSide = true;
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
        }
      
    }

    public void FixedUpdate()
    {
        totalRemotes = Wii.GetRemoteCount();

        if(totalRemotes < 4)
        {
          //  BeginSearch();
        }


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
        oar.inWater = acceleration.y < -0.2f;
        
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

        if(oar.speed > 100)
        {
            Debug.LogWarning("Speed " + oar.speed + " angle " + oar.angle + " angle delta " + angleDelta);
        } else
        {
          //  Debug.Log(angleDelta);
        }

        oar.smoothSpeed = Mathf.SmoothDamp(oar.smoothSpeed, oar.speed, ref oar.yVelocity, oar.smoothTime);
       // oar.inWater = Mathf.Abs(oar.smoothSpeed) > inWaterSpeedThreshold;
        if (oar.inWater)
        {
            if (oar.leftSide)
            {
                oar.direction = oar.smoothSpeed > 0 ? DIRECTIONS.BACKWARDS : DIRECTIONS.FORWARDS;

            } else
            {
                oar.direction = oar.smoothSpeed < 0 ? DIRECTIONS.BACKWARDS : DIRECTIONS.FORWARDS;

            }
        }

        oar.paddlingStateQueue.Enqueue(oar.inWater && oar.isRotating ? 1 : 0);
        if(oar.paddlingStateQueue.Count > paddleStateFramesToAverage)
        {
            // relies on fixed update for speed consistency
            oar.paddlingStateQueue.Dequeue();
        }
        oar.isPaddling = oar.paddlingStateQueue.Average() > 0.5f;
        
     //   oar.isPaddling = oar.inWater && oar.isRotating;
    }
    
    /*
    private Vector3 GetAccelVector(Wiimote wiimote)
    {
        float accel_x;
        float accel_y;
        float accel_z;

        float[] accel = wiimote.Accel.GetCalibratedAccelData();
        accel_x = accel[0];
        accel_y = -accel[2];
        accel_z = -accel[1];

        return new Vector3(accel_x, accel_y, accel_z).normalized;
    }
    */
}

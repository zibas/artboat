using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WiimoteApi;

public class WiimoteOars : MonoBehaviour
{
    public Oar[] oars;

    private int oarCount = 4;

    public float speedMultiplier = 1;

    public float individualAxisThreshold = 0.075f;
    public float maxYAccelerationDeltaForRotation = 0.1f;

    public float inWaterSpeedThreshold = 1;

    public enum DIRECTIONS { FORWARDS, BACKWARDS };

    [System.Serializable]
    public class Oar
    {
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
    }

    public void Initialize(int count = 4)
    {
        oarCount = count;
        WiimoteManager.FindWiimotes();

        if (WiimoteManager.Wiimotes.Count < oarCount)
        {
            Debug.LogError("Could not find " + oarCount + " wiimotes");
            return;
        }

        oars = new Oar[oarCount];
        for (int i = 0; i < oarCount; i++)
        {
            oars[i] = new Oar();
        }

        foreach (var wiimote in WiimoteManager.Wiimotes)
        {
            Debug.Log("Enable accelerator on " + wiimote);
            wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL);
        }

        //WiimoteManager.Wiimotes[0];
    }

    public void Update()
    {
        if (!WiimoteManager.HasWiimote()) { return; }
        if (WiimoteManager.Wiimotes.Count < oarCount)
        {
            Debug.LogError("Do not have " + oarCount + " wiimotes");
            return;
        }
        for (int i = 0; i < oarCount; i++)
        {
            var wiimote = WiimoteManager.Wiimotes[i];
            int ret;
            do
            {
                ret = wiimote.ReadWiimoteData();
            } while (ret > 0);

            ParseData(oars[i], wiimote);

        }
    }

    private void ParseData(Oar oar, Wiimote wiimote)
    {
        Vector3 acceleration = GetAccelVector(wiimote);
        Vector3 accelerationDelta = (oar.acceleration - acceleration) * Time.deltaTime;
        oar.acceleration = acceleration;

        oar.isRotating = Mathf.Abs(accelerationDelta.x) > individualAxisThreshold && Mathf.Abs(accelerationDelta.z) > individualAxisThreshold && Mathf.Abs(accelerationDelta.y) < maxYAccelerationDeltaForRotation;
        float radius = Mathf.Sqrt(oar.acceleration.x * oar.acceleration.x + oar.acceleration.z * oar.acceleration.z);
        float angle = Mathf.Atan2(oar.acceleration.z, oar.acceleration.x) * Mathf.Rad2Deg;
        float angleDelta = (oar.angle - angle) * Time.deltaTime;
        oar.angle = angle;

        if (oar.isRotating)
        {

            // Probably crossed the line between 180 and -180
            if (Mathf.Abs(angleDelta) > 100)
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
        oar.inWater = Mathf.Abs(oar.smoothSpeed) > inWaterSpeedThreshold;
        if (oar.inWater)
        {
            oar.direction = oar.smoothSpeed > 0 ? DIRECTIONS.BACKWARDS : DIRECTIONS.FORWARDS;
        }

        oar.isPaddling = oar.inWater && oar.isRotating;
    }


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

}

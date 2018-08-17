﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour {
    [SerializeField] float MaxSpeed = 4;
    [SerializeField] float MaxRotation = 45;

    [SerializeField] float TorquePerStroke = 5;
    [SerializeField] float SpeedPerStroke = 1;
    [SerializeField] float VelocityDamping = 0.01f;
    [SerializeField] float RotationDamping = 0.01f;
    
    float currentRotation;
    float currentTorque;
    float currentSpeed;

    void Update() {
        bool left = Input.GetKeyUp(KeyCode.LeftArrow);
        bool right = Input.GetKeyUp(KeyCode.RightArrow);

        if (left) { currentTorque -= TorquePerStroke; }
        if (right) { currentTorque += TorquePerStroke; }

        if (left) { currentSpeed += SpeedPerStroke; }
        if (right) { currentSpeed += SpeedPerStroke; }
        
        currentSpeed = Mathf.Clamp(currentSpeed, -MaxSpeed, MaxSpeed);
    }

    void FixedUpdate() {
        currentSpeed = Mathf.Clamp(currentSpeed * (1f - VelocityDamping), -MaxSpeed, MaxSpeed);

        var position = transform.position + transform.forward * -currentSpeed * Time.fixedDeltaTime;

        currentTorque -= currentTorque * RotationDamping;
        currentRotation = currentRotation + currentTorque * Time.fixedDeltaTime;
        currentRotation = Mathf.Clamp(currentRotation, -MaxRotation, MaxRotation);

        var rigidBody = GetComponent<Rigidbody>();
        rigidBody.velocity = Vector3.zero;
        rigidBody.MovePosition(position);
        rigidBody.MoveRotation(Quaternion.Euler(0, currentRotation, 0));
    }

    // I: Slow the boat down if it hits a non-powerup:
    void OnCollisionEnter (Collision col) {
        if(col.gameObject.tag != "Pickup") {
            currentSpeed*=-0.5f;
             Debug.Log("Hit!");
        }
    }

}
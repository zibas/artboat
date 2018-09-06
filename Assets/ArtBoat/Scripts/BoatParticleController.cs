using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatParticleController : MonoBehaviour {
    public ParticleSystem rippleParticles;
    public Rigidbody body;
    public float threshold = .5f;

	void Update () {
	    if(body.velocity.magnitude < threshold)
        {
            if(!rippleParticles.isEmitting)
            {
                rippleParticles.Play();
            }
        } else
        {
            if (rippleParticles.isEmitting)
            {
                rippleParticles.Pause();
            }
        }
	}
}

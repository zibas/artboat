using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkingObstacle : MonoBehaviour {
    [Tooltip("If hit by a rigidbody, multiply its velocity by this value")]
    public float VelocityPenalty = .75f;

    [Tooltip("Total time it takes to sink")]
    public float SinkTime = 1.2f;
    [Tooltip("Collider will be disabled after this time")]
    public float ColliderDisableTime = .5f;
    [Tooltip("Distance to sink")]
    public float SinkDistance = 3f;
    [Tooltip("Randomly rotate a maximum of this many degrees in X/Z while sinking")]
    public float SinkAngleMax = 10f;
    [Tooltip("Rate at which we sink (0 to 1)")]
    public AnimationCurve SinkCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Tooltip("Rate at which we turn while sinking (0 to 1, or less than 1)")]
    public AnimationCurve SinkAngleCurve = AnimationCurve.EaseInOut(0, 0, .25f, 1);

    private bool sinking = false;
    private new Collider collider;

    const string playerTag = "Player";

    private void Awake()
    {
        collider = GetComponent<Collider>();

        if(gameObject.isStatic)
        {
            Debug.LogWarning("Sinking obstacles cannot be static! Sender: " + gameObject.name);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(sinking)
        {
            return;
        }

        if(col.gameObject.CompareTag(playerTag))
        {
            if(col.collider.attachedRigidbody != null)
            {
                col.collider.attachedRigidbody.velocity = col.collider.attachedRigidbody.velocity * VelocityPenalty;
            }

            StartCoroutine(SinkCoroutine());
        }
    }

    IEnumerator SinkCoroutine()
    {
        float time = 0;
        float percent = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + new Vector3(0, -SinkDistance, 0);

        Vector3 startRotation = transform.eulerAngles;
        Vector3 endRotation = transform.eulerAngles + new Vector3(Random.Range(-SinkAngleMax, SinkAngleMax), 0, Random.Range(-SinkAngleMax, SinkAngleMax));

        while (time < SinkTime)
        {
            if(time >= ColliderDisableTime)
            {
                collider.enabled = false;
            }

            percent = SinkCurve.Evaluate(time / SinkTime);
            transform.position = Vector3.Lerp(startPos, endPos, percent);
            transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, percent);

            time += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}

using UnityEngine;
using System.Collections.Generic;

public class VacPac : MonoBehaviour
{
    [Header("Shooting")]
    public ParticleSystem slimeAmmo;
    public float slimeEmitRate = 8f;
    private float slimeEmitTimer;

    [Header("Absorption")]
    public float maxAbsorbDistance = 15f;
    public LayerMask absorbableLayers = ~0;
    public float pullForce = 80f;
    public float minDistanceToAbsorb = 1.2f;

    [Header("Effects")]
    public ParticleSystem absorbParticles;
    public GameObject absorbImpactVFX;

    // This is the fix: track GameObjects, NOT Rigidbodies
    private readonly HashSet<GameObject> beingAbsorbed = new HashSet<GameObject>();
    private readonly List<GameObject> toRemove = new List<GameObject>();
    private Transform myTransform;

    private void Awake() => myTransform = transform;

    private void Update()
    {
        // Shooting
        if (Input.GetMouseButton(0) && slimeAmmo != null)
        {
            slimeEmitTimer += Time.deltaTime;
            if (slimeEmitTimer >= 1f / slimeEmitRate)
            {
                slimeAmmo.Emit(1);
                slimeEmitTimer = 0f;
            }
        }

        // Particles
        if (absorbParticles != null)
        {
            bool holding = Input.GetMouseButton(1);
            if (holding && !absorbParticles.isPlaying) absorbParticles.Play();
            else if (!holding && absorbParticles.isPlaying) absorbParticles.Stop();
        }
    }

    private void FixedUpdate()
    {
        bool isPulling = Input.GetMouseButton(1);
        toRemove.Clear();

        // 1. Add new objects (only while pulling)
        if (isPulling)
        {
            foreach (Collider col in Physics.OverlapSphere(myTransform.position, maxAbsorbDistance, absorbableLayers))
            {
                GameObject go = col.gameObject;
                if (go != null && go.TryGetComponent<Absorbable>(out _) && go.TryGetComponent<Rigidbody>(out _))
                {
                    beingAbsorbed.Add(go);
                }
            }
        }

        // 2. Process every object
        foreach (GameObject go in beingAbsorbed)
        {
            if (go == null)
            {
                toRemove.Add(go);
                continue;
            }

            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb == null)
            {
                toRemove.Add(go);
                continue;
            }

            float distance = Vector3.Distance(rb.position, myTransform.position);

            // INSTANT ABSORB — THIS WORKS NOW
            if (distance <= minDistanceToAbsorb)
            {
                AbsorbNow(go);
                toRemove.Add(go);
                continue;
            }

            // Pull only while holding RMB
            if (isPulling)
            {
                Vector3 dir = (myTransform.position - rb.position).normalized;
                float boost = Mathf.Clamp01(2f / go.transform.lossyScale.magnitude);
                rb.AddForce(dir * pullForce * (1f + boost), ForceMode.Acceleration);
            }
        }

        // 3. Clean up
        foreach (GameObject go in toRemove)
            beingAbsorbed.Remove(go);
    }

    private void AbsorbNow(GameObject obj)
    {
        if (obj.TryGetComponent<Absorbable>(out var absorbable))
            absorbable.Absorb(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.15f);
        Gizmos.DrawSphere(transform.position, maxAbsorbDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistanceToAbsorb);
    }
}
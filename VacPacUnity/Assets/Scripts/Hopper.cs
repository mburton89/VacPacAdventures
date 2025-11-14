using System.Collections;
using UnityEngine;

/// <summary>
/// Makes an object hop/wander randomly in 3D space using physics impulses.
/// Attach to any GameObject with a Rigidbody.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Hopper : MonoBehaviour
{
    [Header("Hop Settings")]
    [SerializeField] private float minHopForce = 1f;
    [SerializeField] private float maxHopForce = 2f;
    [SerializeField] private float hopMinInterval = 2f;
    [SerializeField] private float hopMaxInterval = 4f;
    [SerializeField] private Vector2 horizontalRange = new Vector2(-5f, 5f);
    [SerializeField] private Vector2 verticalRange = new Vector2(5f, 10f);

    [Header("Spin Settings")]
    [SerializeField] private float torqueForce = 2f;
    [SerializeField] private Vector3 torqueRange = new Vector3(1f, 1f, 1f); // Multipliers for X/Y/Z torque randomness

    private Rigidbody rb;
    private bool isHopping = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(HopRoutine());
    }

    private IEnumerator HopRoutine()
    {
        while (isHopping)
        {
            // Generate random hop direction
            float randX = Random.Range(horizontalRange.x, horizontalRange.y);
            float randY = Random.Range(verticalRange.x, verticalRange.y);
            float randZ = Random.Range(horizontalRange.x, horizontalRange.y);

            Vector3 hopDirection = new Vector3(randX, randY, randZ);

            // Apply impulse force
            rb.AddForce(hopDirection * Random.Range(minHopForce, maxHopForce), ForceMode.Impulse);

            // Add random torque for spin (makes hops more lively!)
            Vector3 randomTorque = new Vector3(
                Random.Range(-torqueRange.x, torqueRange.x),
                Random.Range(-torqueRange.y, torqueRange.y),
                Random.Range(-torqueRange.z, torqueRange.z)
            ) * torqueForce;

            rb.AddTorque(randomTorque, ForceMode.Impulse);

            // Optional: Play sound here if desired
            // AudioManager.Instance.PlaySound("SlimeJump", false, transform.position);

            // Wait before next hop
            yield return new WaitForSeconds(Random.Range(hopMinInterval, hopMaxInterval));
        }
    }

    // Public method to stop hopping (e.g., when caught or fleeing)
    public void StopHopping()
    {
        isHopping = false;
        StopAllCoroutines();
    }

    // Public method to resume hopping
    public void ResumeHopping()
    {
        if (!isHopping)
        {
            isHopping = true;
            StartCoroutine(HopRoutine());
        }
    }
}
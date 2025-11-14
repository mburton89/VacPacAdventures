using UnityEngine;

public class FloatingEnemy : MonoBehaviour
{
    [Header("Floating")]
    public float floatStrength = 0.5f; // How high it floats
    public float floatSpeed = 2f; // How fast it floats

    [Header("Movement")]
    public float moveSpeed = 2f; // How fast it follows the player
    public float attackRange = 5f; // Distance to start following player

    private Vector3 startPos;
    private GameObject player;

    void Start()
    {
        startPos = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        FloatUpDown();

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= attackRange)
            {
                FollowPlayer();
            }
            // Else: stays floating in place (no movement)
        }
    }

    void FloatUpDown()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void FollowPlayer()
    {
        // Move horizontally only (no Y-axis change)
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0; // Prevent vertical movement
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void OnDrawGizmosSelected()
    {
        // Visualize attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
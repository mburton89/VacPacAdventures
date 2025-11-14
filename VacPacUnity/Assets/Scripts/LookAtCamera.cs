using UnityEngine;

[ExecuteInEditMode] // Optional: works in Editor too
public class BillboardYOnly : MonoBehaviour
{
    [Tooltip("Leave empty to use Main Camera automatically")]
    public Camera targetCamera;

    private Transform camTransform;
    private Transform myTransform;

    private void Awake()
    {
        myTransform = transform;
        FindCamera();
    }

    private void Start()
    {
        FindCamera();
    }

    private void LateUpdate()
    {
        if (camTransform == null)
        {
            FindCamera();
            return;
        }

        // Get direction to camera on the horizontal plane only
        Vector3 toCamera = camTransform.position - myTransform.position;
        toCamera.y = 0; // Ignore height difference → only rotate on Y axis

        if (toCamera.sqrMagnitude > 0.01f)
        {
            // This makes the canvas face the camera correctly (UI readable)
            myTransform.rotation = Quaternion.LookRotation(-toCamera);
        }
    }

    private void FindCamera()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
            camTransform = targetCamera.transform;
    }

    // Optional: visualize in Scene view
    private void OnDrawGizmosSelected()
    {
        if (camTransform != null)
        {
            Gizmos.color = Color.green;
            Vector3 flatDir = camTransform.position - transform.position;
            flatDir.y = 0;
            Gizmos.DrawRay(transform.position, flatDir.normalized * 2f);
        }
    }
}
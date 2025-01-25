using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // The GameObject to follow
    public Vector3 offset;    // Offset position of the camera relative to the target

    void LateUpdate()
    {
        if (target != null)
        {
            // Update the camera's position based on the target's position
            transform.position = target.position + offset;
        }
    }
}

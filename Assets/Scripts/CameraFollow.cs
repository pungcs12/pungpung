using UnityEngine;
using System.Collections;
public class CameraFollow : MonoBehaviour
{
    public Transform target;  // The GameObject to follow
    public Vector3 offset;    // Offset position of the camera relative to the target
    public Camera cam;
    private float targetSize = 8f; // Desired zoom level
    public float duration = 1f; // Time to complete the zoom
    private bool isFinishFocusPlayer = false;

    public void Start()
    {
        StartCoroutine(ZoomCamera(targetSize, duration));
    }

    IEnumerator ZoomCamera(float newSize, float duration)
    {
        yield return new WaitForSeconds(3f);
        float startSize = gameObject.GetComponent<Camera>().orthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            gameObject.GetComponent<Camera>().orthographicSize = Mathf.Lerp(startSize, newSize, elapsed / duration);
            transform.position = Vector3.Lerp(new Vector3(0f,0f,0f),(target.position + offset), elapsed / duration);
            elapsed += Time.deltaTime;

            yield return null;
        }

        gameObject.GetComponent<Camera>().orthographicSize = newSize; // Ensure exact final value
        isFinishFocusPlayer = true;
        GameManager.Instance.isPlayerDie = false;
    }



//     void Start(){

//     }
//     private void Update()
//     {
//         gameObject.GetComponent<Camera>().orthographicSize = Mathf.Lerp(gameObject.GetComponent<Camera>().orthographicSize, 5f, 2f * Time.deltaTime);
//     }
    void LateUpdate()
    {
        if (target != null && isFinishFocusPlayer)
        {
            // Update the camera's position based on the target's position
            transform.position = target.position + offset;
        }
    }
}


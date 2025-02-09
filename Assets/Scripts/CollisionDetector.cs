using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;

public class CollisionDetector : MonoBehaviour
{
    private Vector3 start_position;
    private PlayerManager playerManager;
    private SwipeDetection swipeInstance;
    public string SceneToLoad;
   // private Pinch myScriptInstance;
    private void Start(){
        start_position = transform.position;
        playerManager = FindFirstObjectByType<PlayerManager>();
        swipeInstance = FindFirstObjectByType<SwipeDetection>();
    }
    private void OnDestroy(){
        playerManager = null;
        swipeInstance = null;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("wallpath"))
        {
            //Debug.Log("Collided with the Tilemap!");
            //myScriptInstance.moveDirection = new Vector3(0,0,0);
            playerManager.current_speed = 0f;
            GameManager.Instance.isPlayerDie = true;
            LifeManager.Instance.LoseLife();
            StartCoroutine(delayToViewWhereToDie());
        }
        if (collision.gameObject.CompareTag("finishline"))
        {
            Debug.Log("Finish!");
            GameManager.Instance.isPlayerDie = true;
            playerManager = null;
            SceneManager.LoadScene(SceneToLoad);
            
        }
    }
    IEnumerator delayToViewWhereToDie(){
        yield return new WaitForSeconds(3f);
        transform.position = start_position;
        GameManager.Instance.isPlayerDie = false;
        swipeInstance.moveDirection = new Vector3 (0f,0f,0f);
        LifeManager.Instance.DisplayLife();
    }
}

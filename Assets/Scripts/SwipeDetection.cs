using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SwipeDetection : MonoBehaviour
{

    [SerializeField] private InputAction position,press,secondaryTouch;
    private Vector2 currentPos => position.ReadValue<Vector2>();
    private Vector2 initialPos;
    private Vector3 playerTargetDefaultScale = new Vector3(5.0f,5.0f,5.0f);
    private Vector3 playerTargetScaleDown = new Vector3(2.0f,2.0f,2.0f);
    public Vector3 moveDirection; 
    private bool movePlayer = false;
    private float minSwipeDistance = 5f; // Minimum swipe distance
    //public GameObject Player;
    private PlayerManager playerManager;
    //bool pinchDetect = false;

    public TMP_Text LogText;

    private void Awake(){
        position.Enable();
        press.Enable();
        secondaryTouch.Enable();
        press.performed += _ => {initialPos = currentPos;};
        press.canceled += _ => DetectSwipe();
       // secondaryTouch.performed += _ => {pinchDetect = true;};
       // secondaryTouch.canceled += _ => {pinchDetect = false;};
    }
    private void OnDestroy(){
        playerManager = null;
    }
    private void Start(){
        playerManager = FindFirstObjectByType<PlayerManager>();
        Debug.Log("start11 "+playerManager);
    }
    // private void PinchDetect(){
    //     pinchDetect = true;
    //     LogText.text = "waited";
    // }
    private void Update()
    {
        //Debug.Log(movePlayer + " die "+ GameManager.Instance.isPlayerDie);
        if(movePlayer){

            if((Vector3.Distance(playerManager.transform.localScale,playerTargetScaleDown) < 0.5f))
                playerManager.current_speed = 1f;

            playerManager.transform.position += moveDirection * playerManager.current_speed * Time.deltaTime;
        }
    }

    private void DetectSwipe(){
        if(GameManager.Instance.isPlayerDie || (playerManager == null))
            return;

        Vector2 swipeDelta = currentPos - initialPos;
       //LogText.text = playerManager.transform.localScale +" "+ (!(Vector3.Distance(playerManager.transform.localScale,playerTargetDefaultScale) < 0.5f)).ToString() +" "+ (!(Vector3.Distance(playerManager.transform.localScale,playerTargetScaleDown) < 0.5f)).ToString();
        if ((swipeDelta.magnitude < minSwipeDistance) || ((!(Vector3.Distance(playerManager.transform.localScale,playerTargetDefaultScale) < 0.5f)) && (!(Vector3.Distance(playerManager.transform.localScale,playerTargetScaleDown) < 0.5f)))){
            return;
        }

        swipeDelta.Normalize();


        if (Mathf.Abs(swipeDelta.x) > 0.5f && Mathf.Abs(swipeDelta.y) > 0.5f)
        {
            if ((playerManager.current_scene == "level00") || (playerManager.current_scene == "level01"))
                return;

            if (swipeDelta.x > 0 && swipeDelta.y > 0)
            {
                LogText.text = "Diagonal Swipe: Top-Right";
                moveDirection = new Vector3(1,1,0);
                movePlayer = true;
               
            }
            else if (swipeDelta.x < 0 && swipeDelta.y > 0)
            {
                LogText.text = "Diagonal Swipe: Top-Left";
                moveDirection = new Vector3(-1,1,0);
                movePlayer = true;
            }
            else if (swipeDelta.x < 0 && swipeDelta.y < 0)
            {
                LogText.text = "Diagonal Swipe: Bottom-Left";
                moveDirection = new Vector3(-1,-1,0);
                movePlayer = true;
            }
            else if (swipeDelta.x > 0 && swipeDelta.y < 0)
            {
                LogText.text = "Diagonal Swipe: Bottom-Right";
                moveDirection =  new Vector3(1,-1,0);
                movePlayer = true;
                //currentMoveDirection = moveDirection;
            }

        }else if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y)) // Horizontal swipe
        {
            if (swipeDelta.x > 0)
            {
                LogText.text = "Swipe Right Detected";
                moveDirection =  new Vector3(1,0,0);
                movePlayer = true;
                //currentMoveDirection = moveDirection;
                
            }
            else
            {
                LogText.text = "Swipe Left Detected";
                moveDirection =  new Vector3(-1,0,0);
                movePlayer = true;
            }
        }
        else // Vertical swipe
        {
            if (swipeDelta.y > 0)
            {
                LogText.text = "Swipe Up Detected";
                moveDirection =  new Vector3(0,1,0);
                movePlayer = true;
            }
            else
            {
                LogText.text = "Swipe Down Detected";
                moveDirection =  new Vector3(0,-1,0);
                movePlayer = true;
            }
        }
    }
}
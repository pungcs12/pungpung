
// using System.Numerics;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Pinch : MonoBehaviour
{
    public Camera targetCamera; // Assign your camera here
    public float speed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 60f;
    public TMP_Text PinchText;
    public TMP_Text DoubleText;
    public TMP_Text SwipeText;
    public TMP_Text LogText;

    private InputAction touchPositionAction;

    public InputActionReference tapAction; // Reference to the Input Action for Tap
    private float lastTapTime_doubleTab = 0f;
    private Vector2 lastTapPosition_doubleTab ;
    private float doubleTapThreshold = 0.3f; // Time threshold for double-tap (in seconds)

    private float initialDistance; // Initial distance between two touches


    //diagonal swipe
    private Vector2 startTouchPosition;   // Start position of the swipe
    private Vector2 endTouchPosition;     // End position of the swipe
    private bool isSwiping = false;       // Flag to track swipe status
    private float swipeThreshold = 50f;   // Minimum swipe distance in pixels



    private float primaryTouchTime = -1f;  // Timestamp for primary touch
    private float secondaryTouchTime = -1f; // Timestamp for secondary touch

    private bool isPinching = false;
    private bool movePlayer = false;
    private bool pinchPlayerDown = false;
    private bool pinchPlayerUp = false;

    public GameObject player;
    private Vector3 moveDirection; 

    public Vector3 targetScale_pinchDown = new Vector3(1f, 1f, 1f); // Target scale
    public Vector3 targetScale_pinchUp = new Vector3(3f, 3f, 3f); // Target scale
    public float duration_pinch = 2f; // Time to reach the target scale

    private float elapsedTime_pinch = 0f;

    private float holdThreshold123 = 0.3f; // Time in seconds to qualify as a hold
    private float touchStartTime;
    private bool isHolding =false;
    private bool isHolding_complete = false;
    private bool isDoubleTab =false;

    private void Awake()
    {
        // Load the Input Actions from PlayerInput
        var inputActionAsset = GetComponent<PlayerInput>().actions;

        // Find the pinch action
        touchPositionAction = inputActionAsset.FindAction("Pinch");
        if (touchPositionAction != null)
        {
            touchPositionAction.Enable();
        }
    }


    private void Start(){
        PinchText.text = "wait";
        DoubleText.text = "wait";
        SwipeText.text = "wait";
    }

    private void Update()
    {

        // if (Touchscreen.current.touches.Count < 2)
        //     return;
        if (Touchscreen.current == null)
            return;
        
        var touches = Touchscreen.current.touches;

        // Track primary touch (index 0) wasPressedThisFrame
        if (touches[0].press.wasPressedThisFrame)
        {
            primaryTouchTime = Time.time;            
        }
        float timeDifference = 0f;
        // Track secondary touch (index 1)
        if (touches[1].press.wasPressedThisFrame)
        {
            secondaryTouchTime = Time.time;
            timeDifference = Mathf.Abs(secondaryTouchTime - primaryTouchTime);
        }



        if (touches[0].press.IsPressed())  
        {
            isDoubleTab = false;
            if (!isHolding)
            {
                // If just started, record the time
                if (touchStartTime == 0)
                {
                    touchStartTime = Time.time;
                }

                // Check if the touch duration exceeds the threshold
                if ((Time.time - touchStartTime) > holdThreshold123)
                {
                    speed = Mathf.Lerp(speed, 0f, Time.deltaTime * 100f);

                    // Stop completely when speed is very low
                    if (speed < 0.01f)
                    {
                        isHolding = true;
                        isHolding_complete = true;
                        speed = 0f;
                    }
                }
            }
        }
        else
        {
            // Reset when touch is released
            touchStartTime = 0;
            isHolding = false;
        }

    
        //if (primaryTouchTime > 0 && secondaryTouchTime > 0 && timeDifference == 0)
        if ( touches[0].press.isPressed && touches[1].press.isPressed)
        {
            Vector2 touch1Position = touches[0].position.ReadValue();
            Vector2 touch2Position = touches[1].position.ReadValue();

            // Calculate the current distance between the two touches
            float currentDistance = Vector2.Distance(touch1Position, touch2Position);

            if (!isPinching)
            {
                // Set the initial distance on the first frame of the pinch
                initialDistance = currentDistance;
                isPinching = true;
            }
            else
            {
                // Calculate the change in distance
                float distanceDelta = currentDistance - initialDistance;

                if (Mathf.Abs(distanceDelta) > 10f)
                {
                    if (distanceDelta > 0)
                    {
                        PinchText.text = "Pinch Out (Zoom In)" +Mathf.Abs(currentDistance) ;
                        DoubleText.text = "wait";
                        SwipeText.text = "wait";
                        pinchPlayerUp = true;

                    }
                    else
                    {
                        PinchText.text =  "Pinch In (Zoom Out)" +Mathf.Abs(currentDistance);
                        DoubleText.text = "wait";
                        SwipeText.text = "wait";
                        pinchPlayerDown = true;
                    }

                    // Update the initial distance for smooth detection
                    initialDistance = currentDistance;
                    // touch2.position = Vector2.zero;
                }
            }
        }
        // else{
        //     isPinching = false;
        // }

        if (!isPinching && !isDoubleTab){
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame && primaryTouchTime > 0 && (secondaryTouchTime == -1f || primaryTouchTime == -1f))
            {
                Debug.Log($"Detected swipe start");
                startTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                isSwiping = true;
            }
            else if (Touchscreen.current.primaryTouch.press.wasReleasedThisFrame && isSwiping  && (secondaryTouchTime == -1f || primaryTouchTime == -1f)) // Touch ends
            {
                if(isHolding_complete){ 
                    speed = 2.0f;
                    isHolding_complete = false;
                }
                Debug.Log($"Detected swipe end" +isPinching + " "+ isDoubleTab);
                endTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                DetectSwipeDirection();
                isSwiping = false;
                elapsedTime_pinch = 0f;
                pinchPlayerUp = false;
                pinchPlayerDown = false;
            }
        }
        primaryTouchTime = -1f;
        secondaryTouchTime = -1f;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            // Get the time of the current tap
            float currentTime = Time.time;
            

            // Check if the time difference between the current tap and the last tap is within the threshold
            // Debug.Log("currentTime: " + currentTime +" lastTapTime: "+lastTapTime_doubleTab);

            // if (currentTime - lastTapTime_doubleTab <= doubleTapThreshold)
            float distance = Vector2.Distance(lastTapPosition_doubleTab,Touchscreen.current.primaryTouch.position.ReadValue());
            // if ((currentTime - lastTapTime_doubleTab <= doubleTapThreshold) && (lastTapPosition_doubleTab == Touchscreen.current.primaryTouch.position.ReadValue()))
            if ((currentTime - lastTapTime_doubleTab <= doubleTapThreshold) && (distance < 30.0f))
            {
                Debug.Log("Double Tap Detected!");
                DoubleText.text = "Double Tap Detaced";
                
                PinchText.text = "wait";
                SwipeText.text = "wait";

                if (speed == 2f){
                    speed = 5f;
                    isDoubleTab = true;

                }else if (speed == 5f){
                    speed = 2f;
                    isDoubleTab = false;

                }


            }

            // Update the last tap time
            lastTapTime_doubleTab = currentTime;
            lastTapPosition_doubleTab = Touchscreen.current.primaryTouch.position.ReadValue();

        }
        if(movePlayer){
            player.transform.position += moveDirection * speed * Time.deltaTime;
        }

        if(pinchPlayerDown){
            if (elapsedTime_pinch < duration_pinch)
            {
                player.transform.localScale = Vector3.Lerp(player.transform.localScale, targetScale_pinchDown, elapsedTime_pinch / duration_pinch);
                elapsedTime_pinch += Time.deltaTime;
                if (player.transform.localScale == targetScale_pinchDown) {
                    pinchPlayerDown = false;
                    elapsedTime_pinch = 0f;
                    isPinching =false;
                }

            }
        }
        if(pinchPlayerUp){
            if (elapsedTime_pinch < duration_pinch)
            {
                player.transform.localScale = Vector3.Lerp(player.transform.localScale, targetScale_pinchUp, elapsedTime_pinch / duration_pinch);
                elapsedTime_pinch += Time.deltaTime;
                if (player.transform.localScale == targetScale_pinchUp) {
                    pinchPlayerUp = false;
                    elapsedTime_pinch = 0f;
                    isPinching =false;
                }
            }
        }
    }
    private void DetectSwipeDirection()
    {
        Vector2 swipeVector = endTouchPosition - startTouchPosition; // Calculate swipe direction
        float distance = swipeVector.magnitude;

        if (distance >= swipeThreshold)
        {
            swipeVector.Normalize(); // Normalize the direction for easier comparison

            // Check for diagonal swipe
            if (Mathf.Abs(swipeVector.x) > 0.5f && Mathf.Abs(swipeVector.y) > 0.5f)
            {
                if (swipeVector.x > 0 && swipeVector.y > 0)
                {
                    Debug.Log("Diagonal Swipe: Top-Right");
                    SwipeText.text = "Top-Right";
                    moveDirection = new Vector3(1,1,0);
                    movePlayer = true;
                }
                else if (swipeVector.x < 0 && swipeVector.y > 0)
                {
                    Debug.Log("Diagonal Swipe: Top-Left");
                    SwipeText.text = "Top-Left";
                    moveDirection = new Vector3(-1,1,0);
                    movePlayer = true;
                }
                else if (swipeVector.x < 0 && swipeVector.y < 0)
                {
                    Debug.Log("Diagonal Swipe: Bottom-Left");
                    SwipeText.text = "Bottom-Left";
                    moveDirection = new Vector3(-1,-1,0);
                    movePlayer = true;
                }
                else if (swipeVector.x > 0 && swipeVector.y < 0)
                {
                    Debug.Log("Diagonal Swipe: Bottom-Right");
                    SwipeText.text = "Bottom-Right";
                    moveDirection =  new Vector3(1,-1,0);
                    movePlayer = true;
                }

            }else if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y)) // Horizontal swipe
            {
                if (swipeVector.x > 0)
                {
                    Debug.Log("Swipe Right Detected");
                    SwipeText.text = "Right";
                    moveDirection =  new Vector3(1,0,0);
                    movePlayer = true;
                }
                else
                {
                    Debug.Log("Swipe Left Detected");
                    SwipeText.text = "Left";
                    moveDirection =  new Vector3(-1,0,0);
                    movePlayer = true;
                }
            }
            else // Vertical swipe
            {
                if (swipeVector.y > 0)
                {
                    Debug.Log("Swipe Up Detected");
                    SwipeText.text = "Up";
                    moveDirection =  new Vector3(0,1,0);
                    movePlayer = true;
                }
                else
                {
                    Debug.Log("Swipe Down Detected");
                    SwipeText.text = "Down";
                    moveDirection =  new Vector3(0,-1,0);
                    movePlayer = true;
                }
            }
                DoubleText.text = "wait";
                PinchText.text = "wait";
        }
        
    }

    // for returning to initial stage for pinch touch
   // private float previousDistance = 0f;
}

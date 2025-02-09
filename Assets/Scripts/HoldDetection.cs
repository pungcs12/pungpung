using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class HoldDetection : MonoBehaviour
{

    [SerializeField] private InputAction position,press;
    private bool isHolding = false;
    private Vector2 initialPosition;
    private float holdDuration = 0.3f; // Time in seconds to trigger hold
    private float holdTimer = 0f;
    private PlayerManager playerManager;

    private void Start(){
        playerManager = FindFirstObjectByType<PlayerManager>();
    }
    private void OnDestroy(){
        playerManager = null;
    }
    private void OnEnable()
    {
        press.Enable();
        position.Enable();

        press.performed += ctx => StartHold();

        press.canceled += ctx => CancelHold();
    }

    private void OnDisable()
    {
        press.performed -= ctx => StartHold();
        press.canceled -= ctx => CancelHold();
        press.Disable();
        position.Disable();
    }

    private void StartHold()
    {
        initialPosition = position.ReadValue<Vector2>(); // Capture start position
        isHolding = true;
        holdTimer = 0f;
        Debug.Log("Hold started at: " + initialPosition);
    }

    private void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdDuration)
            {
                Debug.Log("Hold success!");
                playerManager.current_speed = 0f;
                isHolding = false; // Prevent multiple triggers
            }
        }
    }

    private void CancelHold()
    {
        if(playerManager.current_speed == 0f || playerManager.current_speed != 8.0f){
            playerManager.current_speed = playerManager.default_speed ;
        }
        isHolding = false;
        //Debug.Log("Hold canceled!");
    }
}
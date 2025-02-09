using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class DoubleTapDetection : MonoBehaviour
{

    [SerializeField] private InputAction tap;
    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.3f; // Max time between taps
    private PlayerManager playerManager;
    private float speed_acceleration= 8f;

    private void Start(){
        playerManager = FindFirstObjectByType<PlayerManager>();
    }
    private void OnDestroy(){
        playerManager = null;
    }
    private void OnEnable()
    {
        tap.Enable();
        tap.performed += ctx => OnTap();
    }

    private void OnDisable()
    {
        tap.Disable();
        tap.performed -= ctx => OnTap();
    }

    private void OnTap()
    {
        float currentTime = Time.time;

        if (currentTime - lastTapTime < doubleTapThreshold)
        {
            if ((playerManager.current_scene == "level00") || (playerManager.current_scene == "level01") || (playerManager.current_scene == "level02") || (playerManager.current_scene == "level04") || (playerManager.current_scene == "level05"))
                return;

            if(playerManager.current_speed == playerManager.default_speed){
                playerManager.current_speed = speed_acceleration;
            }else if (playerManager.current_speed == 8.0f){
                playerManager.current_speed = playerManager.default_speed;
            }
            lastTapTime = 0f; // Reset to avoid triple tap issues
        }
        else
        {
            Debug.Log("Single Tap");
            lastTapTime = currentTime;
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections; // Required for coroutines

public class PinchDetection : MonoBehaviour
{
    public TMP_Text LogText;
    private bool isScaleUpPlayer =false;
    private bool isScaleDownPlayer =false;
    private float scaleSpeed = 3f;
    private Vector3 targetScaleDown= new Vector3(2f,2f,2f);
    private Vector3 targetScaleUp= new Vector3(5f,5f,5f);
    private PlayerManager playerManager;

    [SerializeField] private InputAction PrimaryFingerPosition,SecondaryFingerPosisition,SecondaryTouchContact;

    private Coroutine pinchCoroutine;
    
    private void OnEnable()
    {
        PrimaryFingerPosition.Enable();
        SecondaryFingerPosisition.Enable();
        SecondaryTouchContact.Enable();

        SecondaryTouchContact.started += ctx => PinchStart();
        SecondaryTouchContact.canceled += ctx => PinchEnd();

    }

    private void OnDisable()
    {
        PrimaryFingerPosition.Disable();
        SecondaryFingerPosisition.Disable();
        SecondaryTouchContact.Disable();
        SecondaryTouchContact.started -= ctx => PinchStart();
        SecondaryTouchContact.canceled -= ctx => PinchEnd();
    }
    private void Start(){
        playerManager = FindFirstObjectByType<PlayerManager>();
    }
    private void OnDestroy(){
        playerManager = null;
    }
    private void Update(){
        if ((playerManager.current_scene == "level00") || (playerManager.current_scene == "level01") || (playerManager.current_scene == "level02") || (playerManager.current_scene == "level04"))
            return;
        
        if(isScaleUpPlayer){
            playerManager.transform.localScale = Vector3.Lerp(playerManager.transform.localScale, targetScaleUp , scaleSpeed * Time.deltaTime);
            if((Vector3.Distance(playerManager.transform.localScale,targetScaleUp) < 0.5f)){
                playerManager.current_speed = playerManager.default_speed;
                isScaleUpPlayer = false;
            }
        }else if(isScaleDownPlayer){
            playerManager.transform.localScale = Vector3.Lerp(playerManager.transform.localScale, targetScaleDown , scaleSpeed * Time.deltaTime);
            if((Vector3.Distance(playerManager.transform.localScale,targetScaleDown) < 0.5f)){
                playerManager.current_speed = 1.0f;
                isScaleDownPlayer = false;
            }
        }
    }
    private void PinchStart(){
        pinchCoroutine = StartCoroutine(PinchDetect());

    }
    private void PinchEnd(){
        StopCoroutine(pinchCoroutine);
    }

    IEnumerator PinchDetect(){
        float previousDistance = 0f , distance = 0f;
        while(true){
            distance = Vector2.Distance(PrimaryFingerPosition.ReadValue<Vector2>(),SecondaryFingerPosisition.ReadValue<Vector2>());
            if (distance > previousDistance){
                LogText.text = "Zoom out";
                isScaleUpPlayer = true;
                isScaleDownPlayer = false;
                
            }else if (distance < previousDistance){
                LogText.text = "Zoom in";
                isScaleUpPlayer = false;
                isScaleDownPlayer = true;
            }
            previousDistance = distance;
            yield return null;
        }
    }
}
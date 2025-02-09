using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    private float speed = 4f;
    public float current_speed;
    public float default_speed;
    public string current_scene;

    private void Start(){
        default_speed = speed;
        current_speed = default_speed;
        current_scene = SceneManager.GetActiveScene().name;
        Debug.Log("Scene : "+current_scene +" "+  current_speed);
    }

    private void Update(){

    }
    
}

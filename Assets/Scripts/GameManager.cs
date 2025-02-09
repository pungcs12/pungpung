

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance  {get; private set;}
    public bool isPlayerDie = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake(){
        if (Instance == null){
            Instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }
}

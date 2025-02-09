using UnityEngine;
using System;
using TMPro;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance  {get; private set;}
    public TMP_Text LifeText;

    public void Awake(){
        if (Instance == null){
            Instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }
    void OnApplicationQuit() // Called when the app is closed
    {
        PlayerPrefs.SetString("LastExitTime", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    void OnApplicationPause(bool pause) // Called when the app goes to the background
    {
        if (pause)
        {
            PlayerPrefs.SetString("LastExitTime", DateTime.Now.ToString());
            PlayerPrefs.Save();
        }
    }
    void Start()
    {


        //To be comment out as it helps to set life to 100 
        // PlayerPrefs.SetInt("CurrentLife", 100);
        // PlayerPrefs.Save();
        if (PlayerPrefs.HasKey("FirstTimeUse"))
        {
            Debug.Log("Welcome back!");
        }
        else
        {
            Debug.Log("First-time setup...");
            PlayerPrefs.SetInt("FirstTimeUse", 1); // Save that the player has launched the game
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("CurrentLife", 100); // Save that the player has launched the game
            PlayerPrefs.Save();
            DisplayLife();
        }
        RegenerateLife();
    }

    public void RegenerateLife()
    {
        int maxLife = 100;         // Maximum life a player can have
        //int regenRate = 1;       // Amount of life regenerated per interval
        int regenInterval = 300; // Time in seconds (5 minutes per life)

        // Get the last exit time
        if (PlayerPrefs.HasKey("LastExitTime"))
        {
            string lastExitTimeString = PlayerPrefs.GetString("LastExitTime");
            DateTime lastExitTime = DateTime.Parse(lastExitTimeString);
            TimeSpan timeAway = DateTime.Now - lastExitTime;

            // Calculate how much life has been regenerated
            int lifeGained = (int)(timeAway.TotalSeconds / regenInterval);

            // Load current life
            int currentLife = PlayerPrefs.GetInt("CurrentLife", maxLife);

            // Update life, ensuring it doesn't exceed maxLife
            currentLife = Mathf.Min(currentLife + lifeGained, maxLife);
            PlayerPrefs.SetInt("CurrentLife", currentLife);
            PlayerPrefs.Save();
            LifeText.text = "Life :" +currentLife.ToString();
        }
    }
    public void LoseLife()
    {
        int currentLife = PlayerPrefs.GetInt("CurrentLife", 100); // Default to full life
        if (currentLife > 0)
        {
            currentLife--;
            PlayerPrefs.SetInt("CurrentLife", currentLife);
            PlayerPrefs.Save();
        }
    }

    public void DisplayLife(){
        LifeText.text  = "Life :" + PlayerPrefs.GetInt("CurrentLife", 100);
    }

}

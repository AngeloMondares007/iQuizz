using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;

    private void Start()
    {
        ResumeGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // You can use any key or button to trigger pausing.
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Stop time to pause gameplay
        pauseMenu.SetActive(true); // Show the pause menu UI
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resume time to continue gameplay
        pauseMenu.SetActive(false); // Hide the pause menu UI
    }
}
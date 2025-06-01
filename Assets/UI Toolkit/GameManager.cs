using System.Collections;
using System;
using System.Timers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _startingScreenTransition;
    [SerializeField] private GameObject _endingScreenTransition;

    private void Start()
    {
        _startingScreenTransition.SetActive(true);
       
    }

    private void DisableStartingSceneTransition()
    {
        _startingScreenTransition.SetActive(false);
    }

    private void Update()
    {
       
    }

    public static void LoadNextLevel()
    {
        SceneManager.LoadScene("Gameplay");
    }

}

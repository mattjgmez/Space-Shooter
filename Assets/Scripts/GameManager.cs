using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    bool _gameOver;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _gameOver)
        {
            _gameOver = false;
            SceneManager.LoadScene(1); //Loads current scene
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Game exiting.");
        }
    }

    public void GameOver()
    {
        _gameOver = true;
    }
}

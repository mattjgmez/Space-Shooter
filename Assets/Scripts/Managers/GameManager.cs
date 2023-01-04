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
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.TogglePauseMenu();
        }
    }

    public void GameOver()
    {
        _gameOver = true;
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        _gameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(1); //Loads current scene
    }

    public IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0.001f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}

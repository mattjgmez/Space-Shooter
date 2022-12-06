using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] Sprite[] _liveSprites;
    [SerializeField] Image _liveImage;
    [SerializeField] GameObject _pauseUI, _gameOverUI;

    bool _gameOver;

    void Start()
    {
        UpdateScore(0);
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = $"Score: {playerScore}";
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives <= 0)
            _liveImage.sprite = _liveSprites[0];
        else
            _liveImage.sprite = _liveSprites[currentLives];
    }

    public void ShowGameOverText()
    {
        _gameOverUI.SetActive(true);
        _gameOver = true;
        StartCoroutine(GameOverTextBlink());
    }

    public void TogglePauseMenu()
    {
        _pauseUI.SetActive(!_pauseUI.activeSelf);
        Time.timeScale = _pauseUI.activeSelf ? 0f : 1f;
    }

    IEnumerator GameOverTextBlink()
    {
        while (_gameOver)
        {
            _gameOverUI.SetActive(!_gameOverUI.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
    }
}

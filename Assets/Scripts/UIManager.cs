using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] TMP_Text _scoreText, _gameOverText, _restartText;
    [SerializeField] Sprite[] _liveSprites;
    [SerializeField] Image _liveImage;

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
        _gameOverText.enabled = true;
        _restartText.enabled = true;
        _gameOver = true;
        StartCoroutine(GameOverTextBlink());
    }

    IEnumerator GameOverTextBlink()
    {
        while (_gameOver)
        {
            _gameOverText.enabled = !_gameOverText.enabled;
            _restartText.enabled = !_restartText.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }
}

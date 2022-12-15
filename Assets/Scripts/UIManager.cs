using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] TMP_Text _scoreText, _ammoText, _missileText;
    [SerializeField] Sprite[] _liveSprites;
    [SerializeField] Image _liveImage, _boostUI;
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

    public void UpdateAmmo(string currentAmmo)
    {
        _ammoText.text = $"Ammo: {currentAmmo}";
    }

    public void UpdateMissiles(int currentMissiles)
    {
        _missileText.text = $"x {currentMissiles}";
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

    public IEnumerator BoostUICoroutine(float cooldown)
    {
        _boostUI.fillAmount = 1;

        while (_boostUI.fillAmount > 0)
        {
            _boostUI.fillAmount -= 1.0f / cooldown * Time.deltaTime;
            yield return null;
        }

        _boostUI.fillAmount = 0;
    }
}

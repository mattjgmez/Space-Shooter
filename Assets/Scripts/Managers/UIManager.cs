using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] TMP_Text _scoreText, _ammoText, _missileText, _waveText;
    [SerializeField] Sprite[] _liveSprites;
    [SerializeField] Image _liveImage, _boostUI;
    [SerializeField] GameObject _pauseUI, _gameOverUI, _bossWaveText;
    [SerializeField] Slider _bossHealthBar;

    public bool _gameOver;
    Color _textColor;

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
        _gameOver = true;
        StopAllCoroutines();
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

    public void UpdateWaveText(int currentWave)
    {
        _waveText.text = $"WAVE {currentWave}";
        StartCoroutine(WaveText());

        if (currentWave % 5 == 0)
            StartCoroutine(BossWaveTextBlink());
    }

    public IEnumerator WaveText()
    {
        _textColor = Color.white;
        _waveText.color = _textColor;
        yield return new WaitForSeconds(1);

        while (_waveText.color.a > 0)
        {
            _textColor.a -= 1 * Time.deltaTime;
            _waveText.color = _textColor;
            yield return null;
        }

        _textColor.a -= 0;
        _waveText.color = _textColor;
    }

    IEnumerator BossWaveTextBlink()
    {
        float timer = Time.time + 3;
        while (Time.time < timer)
        {
            _bossWaveText.SetActive(!_bossWaveText.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
        _bossWaveText.SetActive(false);
    }

    public IEnumerator InitializeBossHealthBar(float maxHealth)
    {
        _bossHealthBar.maxValue = maxHealth;
        _bossHealthBar.value = maxHealth;

        while (_bossHealthBar.transform.localScale.x < 1)
        {
            _bossHealthBar.transform.localScale += Vector3.one * Time.deltaTime;
            yield return null;
        }
        _bossHealthBar.transform.localScale = Vector3.one;
    }

    public void UpdateBossHealthBar(float currentHealth)
    {
        _bossHealthBar.value = currentHealth;
        if (currentHealth <= 0)
            StartCoroutine(DisableBossHealthBar());
    }

    IEnumerator DisableBossHealthBar()
    {
        while (_bossHealthBar.transform.localScale.x > 0)
        {
            _bossHealthBar.transform.localScale -= Vector3.one * Time.deltaTime;
            yield return null;
        }
        _bossHealthBar.transform.localScale = Vector3.zero;
    }
}

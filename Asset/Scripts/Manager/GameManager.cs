using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using System.Collections;
using EasyTransition;
using System.Threading;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool testGame = false;

    [Header("LV")]
    [SerializeField] private LevelUI[] levels;
    [SerializeField] private LocalizedString[] cheerText;
    [SerializeField] private int lvLoad;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject skipButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI messengerText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Purchased color")]

    [SerializeField] public AnimationCurve curve;
    [HideInInspector] public PlayerMovement player;
    [HideInInspector] public List<Crystal> crystals;
    private DemoLoadScene scene;

    private int _currentLevelIndex = 0;
    private GameObject _currentLevel;
    private int _savedLevelIndex = -1;
    private int comboCount = 1;
    private int loadPreviousLevelCount = 0;
    private float timeCount = 0f;
    private int deathCount = 0;
    private int loadLevelCount = 0; // đếm số lần loadvel
    private int score;

    private void Start()
    {
        instance = this;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        scene = FindObjectOfType<DemoLoadScene>();
        loadLevelCount = 0;

        // Nếu testGame = true và lvLoad có giá trị hợp lệ, sử dụng nó để thiết lập CurrentLevel
        if (testGame && lvLoad >= 0 && lvLoad < levels.Length)
        {
            _currentLevelIndex = lvLoad;
            PlayerPrefs.SetInt("CurrentLevel", _currentLevelIndex);
        }
        else if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            _currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel");
        }
        else
        {
            _currentLevelIndex = 0;
        }

        if (PlayerPrefs.HasKey("MessengerText"))
        {
            messengerText.text = PlayerPrefs.GetString("MessengerText");
        }

        LoadGameData();

        LoadLevel(_currentLevelIndex);

        StartCoroutine(DisplayBanner());

        // Đăng ký sự kiện thay đổi ngôn ngữ
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }


    private void OnDestroy()
    {
        // Unregister from localization change event
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        if (_currentLevelIndex >= 0 && _currentLevelIndex < levels.Length)
        {
            UpdateMessengerText(_currentLevelIndex);
        }
    }

    private IEnumerator DisplayBanner()
    {
        yield return new WaitForSeconds(1f);
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        timeCount += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timeCount / 60f);
        int seconds = Mathf.FloorToInt(timeCount % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void LoadNextLevel()
    {
        AudioManager.instance.PlaySFX(5);

        if (_savedLevelIndex != -1)
        {
            LoadLevel(_savedLevelIndex);
            _savedLevelIndex = -1;
        }
        else if (_currentLevelIndex < levels.Length - 1)
        {
            _currentLevelIndex++;
            comboCount++;  // Tăng combo multiplier khi người chơi vượt qua level
            LoadLevel(_currentLevelIndex);
        }
        else
        {
            // Lưu lại combo multiplier lớn nhất khi đạt đến bàn chơi cuối cùng
            PlayerPrefs.SetInt("HighestComboMultiplier", comboCount);

            // Chuyển sang màn End Menu
            scene.LoadSceneContinue("End Menu");
        }

        UpdateComboText();
    }


    private bool isLoadingPreviousLevel = false; // Cờ để kiểm soát quá trình tải lại level

    public void LoadPreviousLevel()
    {
        // Kiểm tra nếu quá trình tải lại đang diễn ra, không làm gì thêm
        if (isLoadingPreviousLevel) return;

        // Đặt cờ để ngăn việc gọi hàm nhiều lần
        isLoadingPreviousLevel = true;

        AudioManager.instance.PlaySFX(6);

        if (_currentLevelIndex > 0 && !testGame)
        {
            _savedLevelIndex = _currentLevelIndex;
            int previousIndex = _currentLevelIndex - 1;
            LoadLevel(previousIndex);

            comboCount = 1;  // Reset combo multiplier khi quay lại level trước
            loadPreviousLevelCount++;  // Tăng số lần load previous level

            // Tăng số lần chết (load previous level)
            deathCount++;
            Debug.Log("Player Death Count: " + deathCount);

            // Hiển thị nút skip nếu load previous level 3 lần
            if (loadPreviousLevelCount >= 3)
            {
                skipButton.SetActive(true);
            }
        }
        else
        {
            RestartLv();
        }

        // Cập nhật UI
        UpdateComboText();
        UpdateMessengerTextWithCheer();

        // Đặt cờ lại sau khi hoàn thành tải
        StartCoroutine(ResetLoadFlag());
    }

    public void LoadLevel(int index)
    {
        CameraShake.MyInstance.StopShake();

        CameraShake.MyInstance.StartCoroutine(CameraShake.MyInstance.Shake(0.3f, curve));

        if (_currentLevel != null)
        {
            Destroy(_currentLevel);
        }

        _currentLevel = Instantiate(levels[index].levelPrefab, Vector3.zero, Quaternion.identity);

        crystals = new List<Crystal>(FindObjectsOfType<Crystal>());

        foreach (var lazerGun in FindObjectsOfType<LazerGun>())
        {
            lazerGun.DisableEndVFXs();
        }

        foreach (var bullet in FindObjectsOfType<Bullet>())
        {
            Destroy(bullet.gameObject);
        }

        if (player != null)
        {
            player.transform.position = spawnPoint.position;
            player.Data = levels[index].playerData;
        }

        _currentLevelIndex = index;
        player.OnLoadLevel();
        CalculateScore();
        SaveGameData();
        PlayerPrefs.SetString("MessengerText", levels[index].message.GetLocalizedString());

        UpdateLevelText(index);
        UpdateMessengerText(index);
        Camera.main.orthographicSize = levels[index].cameraSize;

        loadLevelCount++;
        if (loadLevelCount % 10 == 0)
        {
            ADSManager.instance.LoadInterstitialAd();
            ADSManager.instance.ShowInterstitialAd();
            loadLevelCount = 0;
        }
    }

    private IEnumerator ResetLoadFlag()
    {
        // Chờ một frame trước khi reset cờ, tránh việc kích hoạt quá sớm
        yield return new WaitForEndOfFrame();
        isLoadingPreviousLevel = false;
    }

    public void SkipLv()
    {
        skipButton.SetActive(false);
        loadPreviousLevelCount = 0;
        LoadNextLevel();
    }


    public void RestartLv()
    {
        CameraShake.MyInstance.StartCoroutine(CameraShake.MyInstance.Shake(0.15f, curve));

        LoadLevel(_currentLevelIndex);

        comboCount = 1;

        UpdateComboText();
    }

    private void UpdateLevelText(int index)
    {
        levelText.text = "Level " + (index + 1);
    }

    private void UpdateMessengerText(int index)
    {
        levels[index].message.GetLocalizedStringAsync().Completed += (result) =>
        {
            messengerText.text = result.Result;
        };
    }

    private void UpdateMessengerTextWithCheer()
    {
        StartCoroutine(GetRandomCheerTextLocalized());
    }

    private IEnumerator GetRandomCheerTextLocalized()
    {
        if (cheerText.Length == 0) yield break;

        int randomIndex = Random.Range(0, cheerText.Length);

        var localizedString = cheerText[randomIndex];
        var operation = localizedString.GetLocalizedStringAsync();

        yield return operation;

        if (operation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            messengerText.text = operation.Result;
        }
    }

    private void UpdateComboText()
    {
        comboText.text = "x" + comboCount;
    }

    public void LoadGameData()
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            _currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel");
        }

        if (PlayerPrefs.HasKey("ElapsedTime"))
        {
            timeCount = PlayerPrefs.GetFloat("ElapsedTime");
        }

        if (PlayerPrefs.HasKey("ComboMultiplier"))
        {
            comboCount = PlayerPrefs.GetInt("ComboMultiplier");
        }

        if (PlayerPrefs.HasKey("deathCount"))
        {
            deathCount = PlayerPrefs.GetInt("deathCount");
        }

        if (PlayerPrefs.HasKey("Score"))
        {
            score = PlayerPrefs.GetInt("Score");
        }

        UpdateComboText();
    }

    public Transform GetCurrentLevelTransform()
    {
        return _currentLevel.transform;
    }
    private void CalculateScore()
    {
        float minTime = 1f;
        float timeUsed = Mathf.Max(timeCount, minTime);

        float baseScore = 200000f / timeUsed;

        score = Mathf.FloorToInt(baseScore * comboCount);

        score = Mathf.Max(score, 0);

    }
    public void SaveGameData()
    {
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("CurrentLevel", _currentLevelIndex);
        PlayerPrefs.SetFloat("ElapsedTime", timeCount);
        PlayerPrefs.SetInt("ComboMultiplier", comboCount);
        PlayerPrefs.SetInt("deathCount", deathCount);
        PlayerPrefs.Save();
    }
}

using UnityEngine;
using TMPro;
using EasyTransition;

public class EndScene : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI time;
    [SerializeField] private TextMeshProUGUI failure;
    [SerializeField] private TextMeshProUGUI combo;
    [SerializeField] private TextMeshProUGUI scoreCount;

    // Các biến kiểm thử được hiển thị trong Inspector
    [Header("Test Values")]
    [SerializeField] private float testElapsedTime; // thời gian mặc định là 60 giây
    [SerializeField] private int testFailureCount = 0; // số lần thất bại mặc định
    [SerializeField] private int testComboMultiplier = 1; // combo multiplier mặc định

    private DemoLoadScene scene;

    public bool test;

    private int deathCount;
    private float timeCount;
    private int comboCount;
    private int score;

    private void Start()
    {
        scene = FindObjectOfType<DemoLoadScene>();

        if (test)
        {
            timeCount = testElapsedTime;
            deathCount = testFailureCount;
            comboCount = testComboMultiplier;
        }
        else
        {
            // Load data từ PlayerPrefs
            deathCount = PlayerPrefs.GetInt("DeathCount", 0);
            timeCount = PlayerPrefs.GetFloat("ElapsedTime", 0f);

            // Lấy combo multiplier lớn nhất từ PlayerPrefs
            comboCount = PlayerPrefs.GetInt("HighestComboMultiplier", 1);
        }

        DisplayValuesSequentially();
    }


    private void DisplayValuesSequentially()
    {
        // Hiển thị timeCount
        time.text = $"{FormatTime(timeCount)}";

        // Hiển thị deathCount
        failure.text = $"{deathCount}";

        // Hiển thị comboCount
        combo.text = $"{comboCount}";

        // Tính toán điểm số
        CalculateScore();

        // Hiển thị scoreCount
        scoreCount.text = $"{score}";
    }

    private void CalculateScore()
    {
        float minTime = 1f;
        float timeUsed = Mathf.Max(timeCount, minTime);

        float baseScore = 200000f / timeUsed;

        score = Mathf.FloorToInt(baseScore * comboCount);

        score = Mathf.Max(score, 0);

        scoreCount.text = $"{score}";

        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("HighestComboMultiplier", comboCount);
        PlayerPrefs.SetFloat("ElapsedTime", timeCount);
        PlayerPrefs.Save(); // Lưu lại tất cả thay đổi vào PlayerPrefs

    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void BackMainMenu()
    {
        scene.LoadSceneContinue("Main Menu");
    }
}

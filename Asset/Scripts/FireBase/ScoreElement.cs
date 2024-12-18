using UnityEngine;
using TMPro;

public class ScoreElement : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text usernameText;
    public TMP_Text scoreText;
    public TMP_Text comboText;
    public TMP_Text timeText;

    // Hàm để cập nhật thông tin mới của phần tử bảng điểm
    public void NewScoreElement(int rank, string _username, int _score, int _combo, float _time)
    {
        rankText.text = rank.ToString();
        usernameText.text = _username;
        scoreText.text = _score.ToString();
        comboText.text = _combo.ToString();

        int minutes = Mathf.FloorToInt(_time / 60f);
        int seconds = Mathf.FloorToInt(_time % 60f);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

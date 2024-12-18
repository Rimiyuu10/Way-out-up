using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Giữ GameManager khi tải các cảnh mới
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetGameData()
    {
        // Ví dụ: Reset level về level đầu tiên
        PlayerPrefs.SetInt("CurrentLevel", 0);

        // Reset thời gian, điểm combo
        PlayerPrefs.SetFloat("ElapsedTime", 0f);
        PlayerPrefs.SetInt("ComboMultiplier", 1);
        PlayerPrefs.Save();
    }
}

using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class LevelUI
{
    public GameObject levelPrefab; // Prefab của level
    public LocalizedString message; // Thông điệp hiển thị sử dụng LocalizedString
    public float cameraSize = 10f; // Kích thước camera
    public PlayerData playerData; // Dữ liệu người chơi
}

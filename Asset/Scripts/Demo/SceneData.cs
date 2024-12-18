using UnityEngine;

[CreateAssetMenu(menuName = "Scene Data")]
public class SceneData : ScriptableObject
{
    [Header("Camera")]
    public float camSize = 10f;

    [Header("Colors")]
    public Color backgroundColor;
    public Color foregroundColor;
}

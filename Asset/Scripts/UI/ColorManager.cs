using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ColorToSell
{
    public Color color;
}

public enum ColorType
{
    playerColor,
    platformColor,
    backgroundColor
}

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;

    [Header("Platform colors")]
    public Color platformColor;
    [SerializeField] private GameObject platformColorButton;
    [SerializeField] private Transform platformColorParent;
    [SerializeField] private ColorToSell[] platformColors;

    [Header("Background colors")]
    public Color backgroundColor;
    [SerializeField] private GameObject backgroundColorButton;
    [SerializeField] private Transform backgroundColorParent;
    [SerializeField] private ColorToSell[] backgroundColors;

    [Header("Player colors")]
    public Color playerColor;
    [HideInInspector] public SpriteRenderer playerColorSprite;
    [SerializeField] private GameObject playerColorButton;
    [SerializeField] private Transform playerColorParent;
    [SerializeField] private ColorToSell[] playerColors;

    void Start()
    {
        instance = this;
        playerColorSprite = GameObject.FindWithTag("Player").GetComponentInChildren<SpriteRenderer>();

        LoadColors(); // Load saved colors at the start

        InitializeColorButtons();
    }

    private void InitializeColorButtons()
    {
        // Initialize platform color buttons
        foreach (var colorToSell in platformColors)
        {
            CreateColorButton(colorToSell.color, platformColorButton, platformColorParent, ColorType.platformColor);
        }

        // Initialize background color buttons
        foreach (var colorToSell in backgroundColors)
        {
            CreateColorButton(colorToSell.color, backgroundColorButton, backgroundColorParent, ColorType.backgroundColor);
        }

        // Initialize player color buttons
        foreach (var colorToSell in playerColors)
        {
            CreateColorButton(colorToSell.color, playerColorButton, playerColorParent, ColorType.playerColor);
        }
    }

    private void CreateColorButton(Color color, GameObject buttonPrefab, Transform parent, ColorType colorType)
    {
        GameObject newButton = Instantiate(buttonPrefab, parent);
        newButton.transform.GetChild(0).GetComponent<Image>().color = color;
        newButton.GetComponent<Button>().onClick.AddListener(() => PurchaseColor(color, colorType));
    }

    public void PurchaseColor(Color color, ColorType colorType)
    {
        if (colorType == ColorType.platformColor)
        {
            platformColor = color;
            SaveColor(color, "PlatformColor");
        }
        else if (colorType == ColorType.backgroundColor)
        {
            backgroundColor = color;
            SaveColor(color, "BackgroundColor");
        }
        else if (colorType == ColorType.playerColor)
        {
            playerColorSprite.color = color;
            playerColor = color;
            SaveColor(color, "PlayerColor");
        }
    }

    private void SaveColor(Color color, string colorPre)
    {
        PlayerPrefs.SetFloat(colorPre + "R", color.r);
        PlayerPrefs.SetFloat(colorPre + "G", color.g);
        PlayerPrefs.SetFloat(colorPre + "B", color.b);
        PlayerPrefs.SetFloat(colorPre + "A", color.a);
        PlayerPrefs.Save();
    }

    private void LoadColors()
    {
        // Load and apply player color
        playerColor = LoadColor("PlayerColor");
        playerColorSprite.color = playerColor;

        // Load and apply platform color
        platformColor = LoadColor("PlatformColor");
    }

    private Color LoadColor(string colorPre)
    {
        float r = PlayerPrefs.GetFloat(colorPre + "R", 1f);
        float g = PlayerPrefs.GetFloat(colorPre + "G", 1f);
        float b = PlayerPrefs.GetFloat(colorPre + "B", 1f);
        float a = PlayerPrefs.GetFloat(colorPre + "A", 1f);
        return new Color(r, g, b, a);
    }
}

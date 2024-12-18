using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class ChangeLanguage : MonoBehaviour
{
    [SerializeField] private LocalizedString localString;
    [SerializeField] private TextMeshProUGUI textComp;

    private int score;

    private void OnEnable()
    {
        localString.Arguments = new object[] { score };
        localString.StringChanged += UpdateText;
    }

    private void OnDisable()
    {
        localString.StringChanged -= UpdateText;
    }

    private void UpdateText(string value)
    {
        textComp.text = value;
    }

    public void IncreaseScore()
    {
        score++;
        localString.Arguments[0] = score;
    }
}
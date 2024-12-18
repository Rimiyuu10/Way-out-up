using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    private bool active = false;

    private void Start()
    {
        // Load the saved language ID from PlayerPrefs (default is 0)
        int savedLanguageID = PlayerPrefs.GetInt("LocalKey", 0);
        ChangeLocale(savedLanguageID); // Apply the saved language
    }

    public void ChangeLocale(int localeID)
    {
        if (active == true)
            return;

        StartCoroutine(SetLocal(localeID));

        // Save the selected language to PlayerPrefs
        PlayerPrefs.SetInt("LocalKey", localeID);
        PlayerPrefs.Save(); // Make sure to save the changes
    }

    IEnumerator SetLocal(int _localID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localID];
        active = false;
    }
}

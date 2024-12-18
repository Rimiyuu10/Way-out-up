using UnityEngine;
using UnityEngine.SceneManagement; // Import Scene Management for changing scenes and quitting the game
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    private bool gamePaused;
    private bool gameMuted;

    [SerializeField] private GameObject mainMenu;

    [Header("Volume info")]
    [SerializeField] private UI_VolumeSlider[] slider;
    [SerializeField] private Image muteIcon;
    [SerializeField] private Image inGameMuteIcon;

    [Header("Auth")]
    public GameObject loginUI;
    public GameObject userDataUI;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < slider.Length; i++)
        {
            slider[i].SetupSlider();
        }

        SwitchMenuTo(mainMenu);
    }

    public void SwitchMenuTo(GameObject uiMenu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        uiMenu.SetActive(true);

        AudioManager.instance.PlaySFX(4);
    }

    public void RestartLV()
    {
        GameManager.instance.RestartLv();
    }

    public void MuteButton()
    {
        gameMuted = !gameMuted;

        if (gameMuted)
        {
            muteIcon.color = new Color(1, 1, 1, .5f);
            AudioListener.volume = 0;
        }
        else
        {
            muteIcon.color = Color.white;
            AudioListener.volume = 1;
        }
    }

    public void StartGameButton()
    {
        muteIcon = inGameMuteIcon;

        if (gameMuted)
            muteIcon.color = new Color(1, 1, 1, .5f);
    }

    public void PauseGameButton()
    {
        if (gamePaused)
        {
            Time.timeScale = 1;
            gamePaused = false;
        }
        else
        {
            Time.timeScale = 0;
            gamePaused = true;
        }
    }
    
    public void LoginScreen()
    {
        SwitchMenuTo(loginUI);
    }

    public void UserDataScreen()
    {
        SwitchMenuTo(userDataUI);
    }

    public void MainMenu()
    {
        // Tải dữ liệu đã lưu
        GameManager.instance.LoadGameData();

        // Chuyển đến scene tiếp theo
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    // Quit Game function to exit the application
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIAuthManager : MonoBehaviour
{
    public static UIAuthManager instance;

    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject userDataUI;
    public GameObject scoreboardUI;
    public GameObject mainMenu;

    // Button variables
    public GameObject loginButton;
    public GameObject scoreboardButton;

    // Authentication status
    private bool isUserLoggedIn = false; // This will be true after the user successfully logs in

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    // Functions to change the login screen UI

    public void ClearScreen() // Turn off all screens
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        userDataUI.SetActive(false);
        scoreboardUI.SetActive(false);
        mainMenu.SetActive(false);
    }

    public void LoginScreen() // Show login screen
    {
        ClearScreen();
        loginUI.SetActive(true);
        Debug.Log("LoginScreen");
    }

    public void RegisterScreen() // Show register screen
    {
        ClearScreen();
        registerUI.SetActive(true);
    }

    public void UserDataScreen() // Show user data screen
    {
        if (isUserLoggedIn)
        {
            ClearScreen();
            userDataUI.SetActive(true);
        }
        else
        {
            LoginScreen(); // Redirect to login screen if not logged in
        }
    }

    public void ScoreboardScreen() // Show scoreboard screen
    {
        if (isUserLoggedIn)
        {
            ClearScreen();
            scoreboardUI.SetActive(true);
        }
        else
        {
            LoginScreen(); // Redirect to login screen if not logged in
        }
    }

    public void MainMenuScreen() // Show main menu screen
    {
        ClearScreen();
        mainMenu.SetActive(true);
        Debug.Log("MainMenuScreen");
    }

    // This method should be called after successful login to update login status
    public void OnUserLogin()
    {
        isUserLoggedIn = true;
        MainMenuScreen();
    }

    public void OnUserSignOut()
    {
        isUserLoggedIn = false;
        MainMenuScreen();
    }
}

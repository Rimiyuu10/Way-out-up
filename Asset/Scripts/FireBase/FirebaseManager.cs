using UnityEngine;
using System.Collections;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading;
public class FirebaseManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    //User Data variables
    [Header("UserData")]
    public TMP_InputField rankField;
    public TMP_InputField usernameField;
    public TMP_InputField scoreField;
    public TMP_InputField comboField;
    public TMP_InputField timeField;
    public GameObject scoreElement;
    public Transform scoreboardContent;

    void Awake()
    {
        // Kiểm tra các phụ thuộc của Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();

                // Kiểm tra nếu có người dùng đã đăng nhập
                if (auth.CurrentUser != null)
                {
                    User = auth.CurrentUser;
                    Debug.Log("Đăng nhập tự động: " + User.DisplayName);
                    usernameField.text = User.DisplayName;
                    usernameField.characterLimit = 10;
                    usernameRegisterField.characterLimit = 10;
                    // Chờ một chút để đảm bảo LoadUserData() đã hoàn tất trước khi chuyển đến màn hình chính
                    StartCoroutine(ShowMainMenuAfterLoad());
                }
            }
            else
            {
                Debug.LogError("Không thể giải quyết các phụ thuộc của Firebase: " + dependencyStatus);
            }
        });

        rankField.interactable = false;
        scoreField.interactable = false;
        comboField.interactable = false;
        timeField.interactable = false;

    }

    private IEnumerator ShowMainMenuAfterLoad()
    {
        // Đảm bảo rằng dữ liệu người dùng đã được tải
        yield return StartCoroutine(LoadUserData());
        UIAuthManager.instance.OnUserLogin(); // Chuyển đến màn hình chính
        SaveDataButton();
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void ClearLoginFeilds()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }
    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }
    //Function for the sign out button
    // Hàm đăng xuất
    public void SignOutButton()
    {
        ClearRegisterFeilds();
        ClearLoginFeilds();
        auth.SignOut();
        UIAuthManager.instance.OnUserSignOut(); // Quay lại màn hình đăng nhập
    }

    //Function for the save button
    // Hàm lưu dữ liệu từ PlayerPrefs vào Firebase
    public void SaveDataButton()
    {
        // Lấy dữ liệu từ PlayerPrefs
        int score = PlayerPrefs.GetInt("Score", 0);
        int combo = PlayerPrefs.GetInt("ComboMultiplier", 0);
        float time = PlayerPrefs.GetFloat("ElapsedTime", 0f);
        int lv = PlayerPrefs.GetInt("CurrentLevel", 1);

        StartCoroutine(UpdateUsernameAuth(usernameField.text));
        StartCoroutine(UpdateUsernameDatabase(usernameField.text));

        StartCoroutine(UpdateScore(score));
        StartCoroutine(UpdateCombo(combo));
        StartCoroutine(UpdateTime(time));
        StartCoroutine(UpdateLV(lv));
    }

    public void UserDatadButton()
    {
        UIAuthManager.instance.UserDataScreen();
    }

    //Function for the scoreboard button
    public void ScoreboardButton()
    {
        StartCoroutine(LoadScoreboardData());
    }

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            StartCoroutine(LoadUserData());

            yield return new WaitForSeconds(1);

            usernameField.text = User.DisplayName;
            UIAuthManager.instance.OnUserLogin(); // Change to user data UI
            confirmLoginText.text = "";
            ClearLoginFeilds();
            ClearRegisterFeilds();
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result.User;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    Task ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        UIAuthManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                        SaveDataButton();
                        ClearRegisterFeilds();
                        ClearLoginFeilds();
                    }
                }
            }
        }
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        Task ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateScore(int _score)
    {
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("score").SetValueAsync(_score);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to update score with {DBTask.Exception}");
        }
    }

    private IEnumerator UpdateCombo(int _combo)
    {
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("combo").SetValueAsync(_combo);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to update combo with {DBTask.Exception}");
        }
    }

    private IEnumerator UpdateTime(float _time)
    {
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("time").SetValueAsync(_time);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to update time with {DBTask.Exception}");
        }
    }

    private IEnumerator UpdateLV(float lv)
    {
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("lv").SetValueAsync(lv);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to update lv with {DBTask.Exception}");
        }
    }

    private IEnumerator LoadUserData()
    {
        // Lấy tất cả dữ liệu người dùng từ cơ sở dữ liệu
        Task<DataSnapshot> DBTask = DBreference.Child("users").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to load user data with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            // Không có dữ liệu nào tồn tại
            scoreField.text = "0";
            comboField.text = "0";
            timeField.text = "00:00"; // Đặt thời gian mặc định thành 00:00
            rankField.text = "-";
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            // Lấy dữ liệu của người dùng hiện tại
            DataSnapshot currentUserSnapshot = snapshot.Child(User.UserId);

            if (currentUserSnapshot.Exists)
            {
                // Hiển thị điểm số, combo, thời gian
                scoreField.text = currentUserSnapshot.Child("score").Value.ToString();
                comboField.text = currentUserSnapshot.Child("combo").Value.ToString();

                float time = float.Parse(currentUserSnapshot.Child("time").Value.ToString());
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                timeField.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                // Tính toán thứ hạng (rank)
                List<int> scores = new List<int>();

                foreach (DataSnapshot userSnapshot in snapshot.Children)
                {
                    if (userSnapshot.Child("score").Exists)
                    {
                        int userScore = int.Parse(userSnapshot.Child("score").Value.ToString());
                        scores.Add(userScore);
                    }
                }

                // Sắp xếp danh sách điểm số giảm dần
                scores.Sort((x, y) => y.CompareTo(x));

                // Xác định thứ hạng của người dùng hiện tại
                int currentScore = int.Parse(currentUserSnapshot.Child("score").Value.ToString());
                int rank = scores.IndexOf(currentScore) + 1;

                // Hiển thị thứ hạng
                rankField.text = rank.ToString();
            }
            else
            {
                // Nếu không có dữ liệu cho người dùng hiện tại
                scoreField.text = "0";
                comboField.text = "0";
                timeField.text = "00:00";
                rankField.text = "-";
            }
        }
    }

    private IEnumerator LoadScoreboardData()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser == null) // Kiểm tra nếu chưa đăng nhập
        {
            Debug.Log("User is not logged in. Redirecting to login screen.");
            UIAuthManager.instance.LoginScreen(); // Chuyển đến màn hình đăng nhập
            yield break; // Thoát IEnumerator nếu chưa đăng nhập
        }

        Task<DataSnapshot> DBTask = DBreference.Child("users").OrderByChild("score").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to load scoreboard data with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            // Xóa các phần tử hiện tại của bảng xếp hạng
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            // Biến đếm thứ hạng
            int rank = 1;
            int userRank = -1; // Biến để lưu thứ hạng của người dùng hiện tại
            string currentUserID = User.UserId; // ID của người dùng hiện tại

            // Duyệt qua dữ liệu từ Firebase, theo thứ tự giảm dần của điểm số
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                int score = int.Parse(childSnapshot.Child("score").Value.ToString());
                int combo = int.Parse(childSnapshot.Child("combo").Value.ToString());
                float time = float.Parse(childSnapshot.Child("time").Value.ToString());

                // Tạo phần tử mới cho bảng xếp hạng
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);

                // Truyền thông tin vào phần tử, bao gồm cả thứ hạng
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(rank, username, score, combo, time);

                // Kiểm tra nếu người chơi này là người dùng hiện tại
                if (childSnapshot.Key == currentUserID)
                {
                    userRank = rank; // Cập nhật thứ hạng của người dùng hiện tại
                }

                // Tăng thứ hạng cho người chơi tiếp theo
                rank++;
            }

            // Nếu tìm thấy thứ hạng của người dùng hiện tại, cập nhật vào rankField
            if (userRank != -1)
            {
                rankField.text = userRank.ToString(); // Gán thứ hạng cho rankField
            }
            else
            {
                rankField.text = "N/A"; // Nếu không tìm thấy, hiển thị "N/A"
            }

            // Chuyển đến màn hình bảng xếp hạng
            UIAuthManager.instance.ScoreboardScreen();
        }
    }
}

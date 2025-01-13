using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class KSD_BackendManager : MonoBehaviour
{
    public static KSD_BackendManager Instance { get; private set; }

    private FirebaseApp app;
    public static FirebaseApp App { get { return Instance.app; } }

    private FirebaseAuth auth;
    public static FirebaseAuth Auth { get { return Instance.auth; } }

    private FirebaseDatabase database;
    public static FirebaseDatabase Database { get { return Instance.database; } }

    private void Awake()
    {
        SetSIngleton();
    }

    void SetSIngleton()
    {
        if (Instance == null)
        {
            Instance = this;
            CheckDependency();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CheckDependency()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                Debug.Log("Firebase dependencies check success");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
                // Firebase Unity SDK is not safe to use here.
                app = null;
                auth = null;
                database = null;
            }
        });
    }
}


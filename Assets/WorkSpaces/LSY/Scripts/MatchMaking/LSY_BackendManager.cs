using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_BackendManager : MonoBehaviour
{
    public static LSY_BackendManager Instance { get; private set; }

    private FirebaseApp app;
    public static FirebaseApp App { get { return Instance.app; } }

    private FirebaseAuth auth;
    public static FirebaseAuth Auth { get { return Instance.auth; } }

    private FirebaseDatabase database;
    public static FirebaseDatabase Database { get { return Instance.database; } }

    private void Awake()
    {
        // 해당 스크립트를 참조한 오브젝트를 싱글톤으로 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        CheckDependency();
    }

    // 데이터베이스 의존성 검사 함수
    private void CheckDependency()
    {
        // 구현 방식 : 요청한 것에 대한 task를 비동기식으로 호환성 체크하는 방식
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;

                Debug.Log("Firebase 의존성 체크 성공!!!!!!!!!");

                DatabaseReference root = LSY_BackendManager.Database.RootReference;
                DatabaseReference userData = root.Child("UserData");

                // TODO : (원하는 변수의 경로 위치)자리에 데이터베이스상의 변수 경로를 작성
                // DatabaseReference data = userData.Child("(원하는 변수의 경로 위치)");
                //
                //data.SetValueAsync(3);
            }
            else
            {
                Debug.LogError($"Firebase 의존성 체크 실패! (사유 : {task.Result})");
                app = null;
                auth = null;
                database = null;
            }
        });
    }
}

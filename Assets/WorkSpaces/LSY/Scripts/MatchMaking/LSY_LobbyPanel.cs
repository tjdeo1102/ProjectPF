using Photon.Pun;
using TMPro;
using UnityEngine;

public class LSY_LobbyPanel : MonoBehaviourPunCallbacks
{
    public const string lsy_RoomName = "TestRoomlsy";
    public TMP_InputField nickName;

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void CheckGuestNickname()
    {
        string nickname = nickName.text;

        if (nickname == "")
        {
            Debug.LogWarning("닉네임을 입력해주세요");
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = nickname;
        Debug.Log($"닉네임: {PhotonNetwork.LocalPlayer.NickName}");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject Loading;
    public TMP_InputField join;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Loading.SetActive(false);
    }

    public void JoinGame()
    {
        PhotonNetwork.JoinRoom(join.text);
    } 
    public void CreateGame()
    {
        PhotonNetwork.CreateRoom(join.text);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("AI_1_FSM");
    }
}

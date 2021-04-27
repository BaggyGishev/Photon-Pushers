﻿using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Gisha.Pushers.Photon
{
    public class PhotonMaster : MonoBehaviourPunCallbacks
    {
        #region Singleton
        public static PhotonMaster Instance { get; private set; }
        #endregion

        public static ClientData ClientData { private set; get; }

        private void Awake()
        {
            ClientData = new ClientData();
            ClientData.SetNickname($"Player {Random.Range(0, 10000)}");
            CreateInstance();
        }

        private void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            ConnectToPhoton();
        }

        private void CreateInstance()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                if (Instance != this)
                {
                    Destroy(Instance.gameObject);
                    Instance = this;
                }
            }
            DontDestroyOnLoad(gameObject);
        }

        private void ConnectToPhoton()
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting to Photon...");
        }

        public void CreateRoom(string name)
        {
            Debug.Log("Creating Room...");

            // Setting room name.
            string roomName = name;

            if (roomName.Length <= 1)
                roomName = $"{ClientData.NickName}'s Room";

            // Setting room options.
            RoomOptions opt = new RoomOptions() { MaxPlayers = 2, IsOpen = true, IsVisible = true };
            PhotonNetwork.JoinOrCreateRoom(roomName, opt, TypedLobby.Default);
        }

        #region Callbacks
        public override void OnConnectedToMaster()
        {
            Debug.Log("Successfully connected to Photon.");
            PhotonNetwork.JoinLobby();

            Debug.Log("Joining to a lobby...");
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("<color=green>Successfully joined to a lobby</color>");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("<color=green>Successfully joined to a room</color>");
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("<color=green>Room was successfully created!</color>");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("<color=red>Failed to create a room!</color>");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("<color=red>Failed to join a room!</color>");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            PhotonNetwork.JoinLobby();
        }
        #endregion
    }

    public struct ClientData
    {
        public string NickName { get; private set; }

        public void SetNickname(string name)
        {
            NickName = name;
            PhotonNetwork.LocalPlayer.NickName = name;
        }
    }
}
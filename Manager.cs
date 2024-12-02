using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;

public class Manager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField RoomName;
    [SerializeField] RectTransform contentRT;
    [SerializeField] TMP_InputField InputPassword;
    [SerializeField] TMP_InputField InputMaxNumberOfPeople;

    public static List<string> roomNamesList = new List<string>();
    string SavedRooms;
    string CurrentDialog;
    string LastCurrentRoom;

    public GameObject RoomButtonPrefab;
    public GameObject LoadingScreen;
    public GameObject MainScreen;
    public GameObject DeleteScreen;
    public GameObject InfoScreen;
    public GameObject Content;
    public GameObject ServerSettings;

    public static bool StartJoin = false;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        DownloadRooms();

        QualitySettings.vSyncCount = 0;
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true, 120);
        Application.targetFrameRate = 120;
    }

    private void DownloadRooms()
    {
        SavedRooms = PlayerPrefs.GetString("SavedRooms");
        CurrentDialog = PlayerPrefs.GetString("CurrentDialog");
        LastCurrentRoom = PlayerPrefs.GetString("CurrentRoom");
        int count = 1;
        if (SavedRooms != "")
        {
            roomNamesList = SavedRooms.Split(' ').ToList();
            roomNamesList.RemoveAt(roomNamesList.Count - 1);
            foreach (string i in roomNamesList)
            {
                RoomName.text = i;
                GameObject NewRoomButton = GameObject.Instantiate(RoomButtonPrefab, contentRT);
                NewRoomButton.GetComponentInChildren<Text>().text = RoomName.text;
                contentRT.sizeDelta += new Vector2(0, -440);
                count += 1;
                NewRoomButton.transform.position += new Vector3(0, 220 * count, 0);
                contentRT.position += new Vector3(0, -440, 0);
                RoomName.text = "";
            }
        }

        if (CurrentDialog != "")
        {
            PlayerPrefs.SetString(LastCurrentRoom, CurrentDialog);
        }
    }

    public override void OnConnectedToMaster()
    {
        LoadingScreen.SetActive(false);
        MainScreen.SetActive(true);
    }

    public void CreateChat(GameObject NewRoomButton)
    {        
        if (RoomName.text != "")
        {
            if (InputPassword.text != "" && InputMaxNumberOfPeople.text != "")
            {
                int MaxNumberOfPeople = Int32.Parse(InputMaxNumberOfPeople.text);
                if (MaxNumberOfPeople < 16 && MaxNumberOfPeople > 1)
                {
                    roomNamesList.Add(RoomName.text);
                    NewRoomButton = GameObject.Instantiate(RoomButtonPrefab, contentRT);
                    NewRoomButton.GetComponentInChildren<Text>().text = RoomName.text;
                    contentRT.sizeDelta += new Vector2(0, -440);
                    int count = roomNamesList.Count + 1;
                    NewRoomButton.transform.position += new Vector3(0, 220 * count, 0);
                    contentRT.position += new Vector3(0, -440, 0);

                    SavedRooms = "";
                    foreach (string i in roomNamesList)
                    {
                        SavedRooms += i + " ";
                    }
                    PlayerPrefs.SetString("SavedRooms", SavedRooms);

                    PlayerPrefs.SetInt(RoomName.text + "MaxP", MaxNumberOfPeople);
                    PlayerPrefs.SetString(RoomName.text + "password", InputPassword.text);
                    ServerSettings.SetActive(false);
                }            
            }          
        }
    }

    public void SetSettings()
    {
        ServerSettings.SetActive(true);
    }

    public void Back()
    {
        ServerSettings.SetActive(false);
    }

    public void CreateOrJoinRoom()
    {
        RoomName.text = RoomButton.CurrentRoomName;
        PlayerPrefs.SetString("CurrentRoom", RoomName.text);

        if (PhotonNetwork.IsConnected && RoomName.text != "")
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = PlayerPrefs.GetInt(RoomName.text + "MaxP");
            PhotonNetwork.CreateRoom(RoomName.text, roomOptions, TypedLobby.Default);
        }
    }

    private void Update()
    {
        if (StartJoin == true)
        {
            StartJoin = false;
            CreateOrJoinRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Chat");
    }

    public void IwantClearAll()
    {
        DeleteScreen.SetActive(true);
    }

    public void Otmena()
    {
        DeleteScreen.SetActive(false);
    }

    public void Info_Open()
    {
        InfoScreen.SetActive(true);
    }

    public void Info_Close()
    {
        InfoScreen.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ClearAll()
    {
        DeleteScreen.SetActive(false);
        PlayerPrefs.SetString("CurrentDialog", "");
        PlayerPrefs.SetString("Nick", "");

        foreach (string i in roomNamesList)
        {
            if (i != "")
            {
                PlayerPrefs.SetString(i, "");
            }
        }
        roomNamesList.Clear();

        PlayerPrefs.SetString("SavedRooms", "");
        SavedRooms = "";

        int CountOfChats = contentRT.transform.childCount;
        while (CountOfChats > 0)
        {
            Transform child = Content.transform.GetChild(CountOfChats - 1);
            Destroy(child.gameObject);
            CountOfChats--;
        }

        PlayerPrefs.DeleteAll();

        contentRT.position = new Vector3(1540, 0, 0);
        contentRT.sizeDelta = new Vector2(0, 0);
    }
}

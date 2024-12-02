using UnityEngine;
using TMPro;
using Photon.Pun;

public class ManagerChat : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField InputMassage;
    [SerializeField] TMP_Text Massage;
    [SerializeField] TMP_Text RoomName;
    [SerializeField] TMP_Text NumberOfPeople;

    [SerializeField] RectTransform textRT;
    [SerializeField] RectTransform contentRT;

    private PhotonView PhotonView;

    public string Diolog;
    string CurrentMessage;
    int CountOfPeople;
    int CountOfPeople2;
    bool Ending = false;
    string Password;

    private void Start()
    {
        PhotonView = GetComponent<PhotonView>();
        RoomName.text = PlayerPrefs.GetString("CurrentRoom");

        if (PlayerPrefs.GetString(RoomName.text) != "")
        {
            InputMassage.text = PlayerPrefs.GetString(RoomName.text);
            SendMassage();
            InputMassage.text = "";
        }

        SendPassword();

        QualitySettings.vSyncCount = 0;
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true, 120);
        Application.targetFrameRate = 120;
    }



    private void Update()
    {
        var size = contentRT.sizeDelta;
        size.y = textRT.sizeDelta.y;
        contentRT.sizeDelta = size;

        if (Ending == false)
        {
            CountOfPeople = PhotonNetwork.CurrentRoom.PlayerCount;
        }
        if (CountOfPeople2 != CountOfPeople)
        {
            UpdateView();
            CountOfPeople2 = CountOfPeople;
        }

        NumberOfPeople.text = CountOfPeople-1 + "/" + PlayerPrefs.GetInt(RoomName.text + "MaxP");
    }

    public void ExitRoom()
    {
        PlayerPrefs.SetString("CurrentDialog", Diolog);
        Ending = true;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    public void UpdateView()
    {
        PhotonView.RpcSecure("Update_View", RpcTarget.AllBufferedViaServer, true, Massage.text);
    }

    [PunRPC]
    private async void Update_View(string diolog)
    {
        if (diolog != "")
        {
            Massage.text = diolog;
        }
    }

    public void SendPassword()
    {
        PhotonView.RpcSecure("Send_Password", RpcTarget.AllBufferedViaServer, true, PlayerPrefs.GetString(RoomName.text + "password"));
    }

    public void SendMassage()
    {
        PhotonView.RpcSecure("Send_Data", RpcTarget.AllBufferedViaServer, true, PhotonNetwork.NickName, InputMassage.text);
    }

    [PunRPC]
    private async void Send_Data(string nick, string message)
    {
        if (Diolog == "" && InputMassage.text == "")
        {
            CurrentMessage = nick + ": " + message;
        }
        else
        {
            CurrentMessage = nick + ": " + message;
        }
        Diolog = Diolog + "\n" + CurrentMessage;
        Massage.text = Diolog;

        await System.Threading.Tasks.Task.Delay(100);
        if (contentRT.sizeDelta.y > 1800)
        {
            if (CurrentMessage.Length < 45)
            {
                contentRT.position = new Vector2(contentRT.position.x, contentRT.position.y + 46);
            }
            else if (CurrentMessage.Length < 45 * 2)
            {
                contentRT.position = new Vector2(contentRT.position.x, contentRT.position.y + (46) * 2);
            }
            else if (CurrentMessage.Length < 45 * 3)
            {
                contentRT.position = new Vector2(contentRT.position.x, contentRT.position.y + (46) * 3);
            }
            else
            {
                contentRT.position = new Vector2(contentRT.position.x, contentRT.position.y + (46) * 4);
            }
        }
        
    }

}

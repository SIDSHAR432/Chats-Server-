using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RoomButton : MonoBehaviour
{
    public static string CurrentRoomName;
    string SavedRooms;

    List<string> roomNamesList = new List<string>();

    public void Join(GameObject CurrentRoomButton)
    {
        CurrentRoomName = CurrentRoomButton.GetComponentInChildren<Text>().text;
        Manager.StartJoin = true;
    }

    public void DeleteRoom(GameObject CurrentRoomButton)
    {
        CurrentRoomName = CurrentRoomButton.GetComponentInChildren<Text>().text;
        Manager.roomNamesList.Remove(CurrentRoomName);
        SavedRooms = PlayerPrefs.GetString("SavedRooms");
        roomNamesList = SavedRooms.Split('/').ToList();
        roomNamesList.Remove(CurrentRoomName);
        SavedRooms = "";
        foreach (string i in roomNamesList)
        {
            if (i != "")
            {
                SavedRooms += i + "/";
            }
        }
        PlayerPrefs.SetString("SavedRooms", SavedRooms);
        PlayerPrefs.SetString(CurrentRoomName, "");
        Transform child = CurrentRoomButton.transform.GetChild(2);
        child.gameObject.SetActive(true);
    }
}

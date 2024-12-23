using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public static class LSY_CustomProperties
{
    private static PhotonHashtable customProperty = new PhotonHashtable();

    public const string READY = "Ready";
    public const string ROOM_PASSWORD = "RoomPassword";
    public const string IS_PASSWORD_PROTECTED = "IsPasswordProtected";

    public static void SetReady(this Player player, bool ready)
    {
        customProperty.Clear();
        customProperty[READY] = ready;
        player.SetCustomProperties(customProperty);
    }

    public static bool GetReady(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        if (customProperty.ContainsKey(READY))
        {
            return (bool)customProperty[READY];
        }
        else
        {
            return false;
        }
    }

    public static bool ReadyCheck(Player player)
    {
        if (player.CustomProperties.TryGetValue(READY, out object isReady))
        {
            return (bool)isReady;
        }
        return false;
    }

    public const string LOAD = "Load";

    public static void SetLoad(this Player player, bool load)
    {
        customProperty.Clear();
        customProperty[LOAD] = load;
        player.SetCustomProperties(customProperty);
    }

    public static bool GetLoad(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        if (customProperty.ContainsKey(LOAD))
        {
            return (bool)customProperty[LOAD];
        }
        else
        {
            return false;
        }
    }

    public static void SetRoomPassword(this Room room, string password)
    {
        customProperty.Clear();
        customProperty[ROOM_PASSWORD] = password;
        room.SetCustomProperties(customProperty);
    }

    public static string GetRoomPassword(this Room room)
    {
        PhotonHashtable customProperty = room.CustomProperties;
        if (customProperty.ContainsKey(ROOM_PASSWORD))
        {
            return (string)customProperty[ROOM_PASSWORD];
        }
        return string.Empty; 
    }

    public static void SetIsPasswordProtected(this Room room, bool isProtected)
    {
        customProperty.Clear();
        customProperty[IS_PASSWORD_PROTECTED] = isProtected;
        room.SetCustomProperties(customProperty);
    }

    public static bool GetIsPasswordProtected(this Room room)
    {
        PhotonHashtable customProperty = room.CustomProperties;
        if (customProperty.ContainsKey(IS_PASSWORD_PROTECTED))
        {
            return (bool)customProperty[IS_PASSWORD_PROTECTED];
        }
        return false; 
    }

    public static bool CheckRoomPassword(this Room room, string inputPassword)
    {
        if (room.GetIsPasswordProtected())  
        {
            string roomPassword = room.GetRoomPassword();
            return roomPassword == inputPassword;
        }
        return true;  
    }
}

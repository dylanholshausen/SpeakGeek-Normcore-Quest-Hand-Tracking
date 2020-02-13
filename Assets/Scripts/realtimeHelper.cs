using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using System;

public class realtimeHelper : MonoBehaviour
{
    private Realtime _Realtime;
    public string playerPrefabName;

    private void Start()
    {
        _Realtime = GetComponent<Realtime>();
        //_Realtime.Connect(randomString());
        _Realtime.Connect("hand-tracking");

        _Realtime.didConnectToRoom += _Realtime_didConnectToRoom;
    }

    private void _Realtime_didConnectToRoom(Realtime realtime)
    {
        GameObject newPlayer = Realtime.Instantiate(playerPrefabName);
        newPlayer.GetComponent<RealtimeView>().RequestOwnership();
    }

    private string randomString()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new String(stringChars);

        return finalString;
    }

}

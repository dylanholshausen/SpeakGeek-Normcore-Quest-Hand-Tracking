/************************************************************************************
Copyright : Copyright 2019 (c) Speak Geek (PTY), LTD and its affiliates. All rights reserved.

Developer : Dylan Holshausen

Script Description : Helper for Normcore Realtime Component

************************************************************************************/

using UnityEngine;
using Normal.Realtime;
using System;

public class realtimeHelper : MonoBehaviour
{
    private Realtime _Realtime;
    public string playerPrefabName;

    public string roomName;

    private void Start()
    {
        _Realtime = GetComponent<Realtime>();

        //Connect to Random Room Code
        //_Realtime.Connect(randomString(8));

        //Connect to Preset Code
        _Realtime.Connect(roomName);

        _Realtime.didConnectToRoom += _Realtime_didConnectToRoom;
    }

    //Realtime Event when Connecting to a Room
    private void _Realtime_didConnectToRoom(Realtime realtime)
    {
        //Instantiate a New Player
        GameObject newPlayer = Realtime.Instantiate(playerPrefabName);
        newPlayer.GetComponent<RealtimeView>().RequestOwnership();
    }

    //Generate Random String
    private string randomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new String(stringChars);

        return finalString;
    }

}

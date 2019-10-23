using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MoneyMessage : UnityEvent<System.Object>
{
}


[System.Serializable]
public class MoneyButtonMessage : UnityEvent<System.Object>
{
    public GameObject button;
}

public static class GameHandlerData
{
    public static string SceneStartLoadingHandler = "00";
    public static string SceneInLoadingHandler = "01";
    public static string SceneLoadedHandler = "02";
    public static string GameStartingHandler = "05";
    public static string PlayerJoinedServerHandler = "06";

}
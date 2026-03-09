using BepInEx;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using Debug = UnityEngine.Debug;

class XLOG
{

    public static void warning(string title, string msg)
    {
        Debug.LogWarning($"{title}:{msg}");
    }
    public static void error(string msg, string class_name)
    {
        Debug.LogError("ERROR: " + msg);
        Debug.LogError("POSITION: "+ class_name);
    }
    public static void message(string msg, string class_name = "")
    {
        Debug.LogWarning("MESSAGE: " + msg);
        if(class_name != "")
        {
            Debug.LogWarning("POSITION: " + class_name);
        }
    }
    public static void local_chat_message(string msg)
    {
        Chat.instance.AddString("", Localization.instance.Localize(msg), Talker.Type.Normal);
        Traverse.Create(Chat.instance).Field("m_hideTimer").SetValue(0);
    }
}



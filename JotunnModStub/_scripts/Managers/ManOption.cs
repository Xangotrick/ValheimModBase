using BepInEx;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using static ItemDrop;
using System.Text;





public class ManOption : JsonObject
{
    public static ManOption Instance;

    public string astring;
    //Example
    //public bool XPBAR_LEVEL = true;
    //public bool XPBAR_JOB1 = true;
    //public bool XPBAR_JOB2 = true;
    //public bool IS_ADMIN = false;

    public ManOption()
    {

    }

    public static void Load()
    {
        string text = rw.load_save_textfile("r", "options.dat", "", (new ManOption().serialize()));
        Instance = JsonObject.deserialize<ManOption>(text);
    }
    public static void Save() => rw.load_save_textfile("w", "options.dat", ManOption.Instance.serialize());

    public override string ToString()
    {
        string r = "";

        r += astring;
        //r += $"LEVEL:{XPBAR_LEVEL}";
        //r += $"JOB1:{XPBAR_JOB1}";
        //r += $"JOB2:{XPBAR_JOB2}";

        return r;
    }
}


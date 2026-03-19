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
using System.Text;

using System.Reflection;

public class ManApp
{
    public static ManApp Instance;
    public List<App> apps;

    Dictionary<KeyCode, List<Type>> KeyDictionary;
    MethodInfo MethodInfoAdd;

    public App.AppToolTip MyTooltip;
    public KERNEL.AppIGConsole MyIGConsole;



    public ManApp()
    {
        Instance = this;
        

        apps = new List<App>();


        KeyDictionary = new Dictionary<KeyCode, List<Type>>();

        List<Type> AppSubClasses = new List<Type>();
        Assembly asm = Assembly.Load(Assembly.GetAssembly(typeof(App)).GetName());
        foreach (Type type in asm.GetTypes())
        {
            if (type.IsSubclassOf(typeof(App)))
            {
                AppSubClasses.Add(type);
            }
        }
        foreach (Type type in AppSubClasses)
        {
            FieldInfo keyinfo = type.GetField("KeyPressOn", BindingFlags.Static | BindingFlags.Public);
            if (keyinfo != null)
            {
                KeyCode[] KeyArray = (KeyCode[])keyinfo.GetValue(null);
                foreach (KeyCode key in KeyArray)
                {
                    if (!KeyDictionary.ContainsKey(key))
                    {
                        KeyDictionary[key] = new List<Type>();
                    }
                    KeyDictionary[key].Add(type);
                }
            }
        }

        MethodInfoAdd = typeof(ManApp).GetMethod(nameof(Add));

    }


    public bool stop_control
    {
        get
        {
            foreach (App app in apps)
            {
                if (app.stop_control)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public T Add<T>(int EventID = -1) where T : App, new()
    {
        App new_app = new T() as App;

        if (new_app.is_unique)
        {
            foreach (App app in apps)
            {
                if (app is T)
                {
                    if (new_app.Switch_OnOff)
                    {
                        app.QuitApp();
                    }
                    XLOG.error("Only one of this App at a time!", "UI");
                    return null;
                }
            }
        }
        if (new_app.IG_only)
        {
            if (!Player.m_localPlayer)
            {
                XLOG.error("This App can only be created IG", "UI");
                return null;
            }
        }

        apps.Add(new_app);
        return new_app as T;
    }

    public bool IsOpen<T>()
    {
        if (apps.Where(x => x is T).ToList().Count > 0) { return true; }
        return false;
    }
    public T Get<T>() where T : class
    {
        return apps.Where(x => x is T).First() as T;
    }
    //Apps Segment

    public void UpdateApps()
    {
        foreach (App app in apps)
        {
            app.Update();
        }
    }

    public void GUIUpdateApps()
    {
        //Draw each app
        foreach (App app in apps)
        {
            app.Draw();
        }
        foreach (App app in apps)
        {
            app.DrawPriority();
        }


    //Remove each app marked for removal
    tryagain:
        foreach (App app in apps)
        {
            if (app.ToBeRemoved)
            {
                apps.Remove(app);
                goto tryagain;
            }
        }

        //Manage Inputs
        if (stop_control)
        {
            GUIManager.BlockInput(true);
        }
        else
        {
            GUIManager.BlockInput(false);

            typeof(GUIManager).GetMethod("ResetInputBlock", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { });
        }

        UpdateAppBehavior();
    }

    public void UpdateAppBehavior()
    {
        if (MOD_OPTIONS.IS_IG_CONSOLE)
        {
            if (MyIGConsole == null) { MyIGConsole = Add<KERNEL.AppIGConsole>(); }
        }
    }
    

    public void HandleInputs()
    {
        foreach (KeyCode key in KeyDictionary.Keys)
        {
            if (UnityInput.Current.GetKeyDown(key))
            {
                foreach (Type type in KeyDictionary[key])
                {
                    MethodInfoAdd.MakeGenericMethod(type).Invoke(this, new object[] { -1 });
                }
            }
        }
    }
    public void KillAll()
    {
        foreach (App anapp in apps)
        {
            if (anapp is PermanentApp || anapp.is_immune_escape) { continue; }
            anapp.ToBeRemoved = true;
        }
    }


    // Permanent Apps Segment

    public T PermanentAppFromT<T>() where T : PermanentApp, new()
    {

        return default;
    }
    public void Show<T>(params object[] args) where T : PermanentApp, new()
    {
        PermanentApp app = PermanentAppFromT<T>();
        if (app == null) { app = Add<T>() as PermanentApp; }

        app.Show();

    }
    public void Hide<T>(params object[] args) where T : PermanentApp, new()
    {
        PermanentApp app = PermanentAppFromT<T>();

        app.Hide();
    }
    public void LoadPermanentApp()
    {

    }

}

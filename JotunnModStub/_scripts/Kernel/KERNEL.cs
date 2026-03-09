using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Managers;
using PlayFab.Internal;
using System.IO;
using System.Reflection;

using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;




[BepInPlugin(pluginGUID, pluginName, pluginVersion)]
[BepInDependency(Jotunn.Main.ModGuid)]
public class KERNEL : BaseUnityPlugin
{
    #region build info
    public const string pluginGUID = "a";
    public const string pluginName = "a";
    public const string pluginVersion = "1.0.0";
    public const string foldername = "TemplateName";

    public static string filename { get { return foldername + "_"; } }
    public static string path_modfolder { get { return Application.dataPath.Replace("valheim_Data", "BepInEx") + "/plugins/" + foldername + "/"; } }

    public static bool IsGameRunning
    {
        get
        {
            if (ZNet.instance)
            {
                if (ZNet.instance.IsDedicated())
                {
                    return true;
                }
                else
                {
                    if (Player.m_localPlayer)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }


    private readonly Harmony _HarmonyInstance = new Harmony(pluginGUID);
    public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(pluginName);
    private void _BuildInfo()
    {
        XLOG.message("building the mod: " + pluginName);
        Debug.Log("te2st");
        Assembly assembly = Assembly.GetExecutingAssembly();
        _HarmonyInstance.PatchAll(assembly);



    }
    #endregion


    private float _sync_request_timestamp = -10;
    private bool _has_synced_message = false;
    private bool _game_is_on = false;

    private void Awake()
    {
        _BuildInfo();   ///Handles Mod Version/Name

        ManOption.Load();
        new ManApp();

        Debug.Log("testc");

        InvokeRepeating("Update1000", 1, 1);

        mod_data.U.awake();

        //LOAD RESSOURCES
        PrefabManager.OnVanillaPrefabsAvailable += load_assets;

        Main.Awake();
    }
    private void load_assets()
    {
        XLOG.warning("ASSETS", "Start of load_assets function.");
        mod_data.Data.load_assets();
        XLOG.message("end load assets");
        PrefabManager.OnVanillaPrefabsAvailable -= load_assets;

        FejdStartup.instance.transform.Find("Menu").Find("Logo").Find("LOGO").GetComponent<Image>().color = new Color(1, 0, 1);
        Transform clone = Instantiate(FejdStartup.instance.transform.Find("Menu").Find("Logo").Find("LOGO"), FejdStartup.instance.transform.Find("Menu").Find("Logo"));
        clone.GetComponent<RectTransform>().position += new Vector3(5,5,0);
        clone.GetComponent<Image>().color = new Color(0.3f, 0, 0);
        XLOG.warning("ASSETS", "End of load_assets function.");
    }


   
    private void Update()
    {

        Main.Update();


        if (IsGameRunning)
        {
            if(!_game_is_on)
            {
                _game_is_on = true;
                Main.OnGameStart();
            }
            GameUpdate();
        }
        else
        {
            if(_game_is_on)
            {
                _game_is_on = false;
                Main.OnGameEnd();
            }
        }
    }

    private void FixedUpdate()
    {
        Main.FixedUpdate();
    }

    private void Update1000()
    {
        Main.Update1000();

        if (IsGameRunning) { GameUpdate1000(); }
        {
            if(MOD_OPTIONS.IS_ONLINE_SYNC)
            {
            }
        }
        //Debug.Log("testc");
        //Debug.Log(Time.time);
        //Debug.Log(EnvMan.instance.m_dirLight.transform.localEulerAngles.ToString());
        //Debug.Log(EnvMan.instance.m_dirLight.transform.eulerAngles.ToString());
    }

    private void GameUpdate()
    {

        ManApp.Instance.HandleInputs();

        ManApp.Instance.UpdateApps();

        if (!MOD_OPTIONS.IS_ONLINE_SYNC | _has_synced_message)
        {
            Main.GameUpdate();
        }
    }

 

    private void GameUpdate1000()
    {


        if (MOD_OPTIONS.IS_ONLINE_SYNC)
        {
            if(!IsGameRunning)
            {
                if(_has_synced_message)
                {
                    _has_synced_message = false;

                }
                if(NetClass.instance != null)
                {
                    NetClass.instance = null;
                }
                return;
            }

            if (NetClass.instance == null)
            {
                if (NetClass.is_server)
                {

                    NetClass.instance = new NetServer();
                }
                if (!NetClass.is_server)
                {
                    NetClass.instance = new NetClient();
                }
                return;
            }
            if (!NetClass.instance.is_synced)
            {
                if(Time.time - _sync_request_timestamp > 10f)
                {
                    XLOG.message("try sync");
                    _sync_request_timestamp = Time.time;
                    NetClass.instance.RequestSync();
                }
                return;
            }

            if (!_has_synced_message)
            {
                _has_synced_message = true;
                XLOG.message("synced!");
                foreach (NetObj obj in NetClass.instance.ListNetObj)
                {
                    Debug.Log(obj.serialize());
                }
                XLOG.message("item end list","KERNEL");

                Main.OnServerSync();
            }

        }
        if(!MOD_OPTIONS.IS_ONLINE_SYNC | _has_synced_message)
        {
            Main.GameUpdate1000();
        }
    }

    private void OnGUI()
    {
        UI.update_screen();

        ManApp.Instance.GUIUpdateApps();
    }
    // More Code Here!
}
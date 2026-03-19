using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Managers;
using PlayFab.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static Heightmap;
using static System.Net.Mime.MediaTypeNames;
using Application = UnityEngine.Application;
using Debug = UnityEngine.Debug;
using Image = UnityEngine.UI.Image;




[BepInPlugin(MOD_OPTIONS.MODNAME, MOD_OPTIONS.MODNAME, pluginVersion)]
[BepInDependency(Jotunn.Main.ModGuid)]
public class KERNEL : BaseUnityPlugin
{
    public static KERNEL instance;
    #region build info
    public const string pluginVersion = "1.0.0";
    public static string foldername { get { return MOD_OPTIONS.MODNAME; } }

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


    private readonly Harmony _HarmonyInstance = new Harmony(MOD_OPTIONS.MODNAME);
    public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(MOD_OPTIONS.MODNAME);
    private void _BuildInfo()
    {
        XLOG.message("building the mod: " + MOD_OPTIONS.MODNAME);
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

        instance = this;

        ManOption.Load();
        new ManApp();

        Debug.Log("testc");

        InvokeRepeating("Update1000", 1, 1);

        mod_data.U.awake();

        //LOAD RESSOURCES
        PrefabManager.OnVanillaPrefabsAvailable += load_assets;



        CustomStatusEffects.U.awake();
        CallMethods.Awake();
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

        Transform clone2 = Instantiate(FejdStartup.instance.transform.Find("Menu").Find("Logo").Find("LOGO"), FejdStartup.instance.transform.Find("Menu").Find("Logo"));
        clone2.GetComponent<RectTransform>().position += new Vector3(5, 5, 0);
        Texture2D tex = mod_data.Data.TEX("vivaldi");
        Sprite icon = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        clone2.GetComponent<Image>().sprite = icon;
        XLOG.warning("ASSETS", "End of load_assets function.");
    }


   
    private void Update()
    {

        CallMethods.Update();


        if (IsGameRunning)
        {
            if(!_game_is_on)
            {
                _game_is_on = true;
                CallMethods.OnGameStart();
            }
            GameUpdate();
        }
        else
        {
            if(_game_is_on)
            {
                _game_is_on = false;
                CallMethods.OnGameEnd();
            }
        }
    }

    private void FixedUpdate()
    {
        CallMethods.FixedUpdate();
    }

    private void Update1000()
    {
        CallMethods.Update1000();

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
            CallMethods.GameUpdate();
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

                CallMethods.OnServerSync();
            }

        }
        if(!MOD_OPTIONS.IS_ONLINE_SYNC | _has_synced_message)
        {
            CallMethods.GameUpdate1000();
        }
    }

    private void OnGUI()
    {
        UI.update_screen();

        ManApp.Instance.GUIUpdateApps();
    }
    // More Code Here!

    private class CallMethods
    {
        static List<Action> DelAwake;
        static List<Action> DelUpdate;
        static List<Action> DelFixedUpdate;
        static List<Action> DelUpdate1000;
        static List<Action> DelOnServerSync;
        static List<Action> DelOnGameStart;
        static List<Action> DelGameUpdate;
        static List<Action> DelGameUpdate1000;
        static List<Action> DelOnGameEnd;
        
        private static void InitMethodCalls(string methodname,ref List<Action> del)
        {
            del = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(CDG)) && !t.IsAbstract)
                .Select(t => t.GetMethod(methodname, BindingFlags.Public | BindingFlags.Static))
                .Where(m => m != null)
                .Select(m => (Action)Delegate.CreateDelegate(typeof(Action), m))
                .ToList();
        }


        internal static void Awake()
        {
            // Initialize delegates at startup

            InitMethodCalls("awake", ref DelAwake);
            InitMethodCalls("update", ref DelUpdate);
            InitMethodCalls("fixedUpdate", ref DelFixedUpdate);
            InitMethodCalls("update1000", ref DelUpdate1000);
            InitMethodCalls("onServerSync", ref DelOnServerSync);
            InitMethodCalls("onGameStart", ref DelOnGameStart);
            InitMethodCalls("gameUdpate", ref DelGameUpdate);
            InitMethodCalls("gameUdpate1000", ref DelGameUpdate1000);
            InitMethodCalls("onGameEnd", ref DelOnGameEnd);

            /*Debug.Log(DelAwake.Count);
            Debug.Log(DelUpdate.Count);
            Debug.Log(DelFixedUpdate.Count);
            Debug.Log(DelUpdate1000.Count);
            Debug.Log(DelOnServerSync.Count);
            Debug.Log(DelOnGameStart.Count);
            Debug.Log(DelGameUpdate.Count);
            Debug.Log(DelGameUpdate1000.Count);
            Debug.Log(DelOnGameEnd.Count);*/


            ///Handle CDG Classes
            var assembly = Assembly.GetExecutingAssembly();
            var childtypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CDG)));
            foreach(var type in childtypes)
            {
                Debug.Log(type.Name);
            }

            Main.Awake();

            foreach (var action in DelAwake) { action(); }
        }

        /// Lancé chaque update du mod
        internal static void Update()
        {
            Main.Update();

            foreach (var action in DelUpdate) { action(); }
        }

        /// Lancé chauqe udpate du mod fréquence fixe (Time.deltatime constant)
        internal static void FixedUpdate()
        {
            Main.FixedUpdate();
            foreach (var action in DelFixedUpdate) { action(); }
        }

        /// Lancé chaque seconde
        internal static void Update1000()
        {
            Main.Update1000();
            foreach (var action in DelUpdate1000) { action(); }

        }

        /// Lancé à la syncronisation du serveur, si option réseau utilisée
        internal static void OnServerSync()
        {
            Main.OnServerSync();
            foreach (var action in DelOnServerSync) { action(); }
        }

        /// Lancé au lancement du réseau/une fois que le monde et le personnage sont existant
        internal static void OnGameStart()
        {
            if(Player.m_localPlayer)
            {
                Player.m_localPlayer.SetGodMode(MOD_OPTIONS.IS_GOD_MODE);
            }
            Main.OnGameStart();
            foreach (var action in DelOnGameStart) { action(); }
        }

        /// Lancé chaque update du mod une fois que le monde est chargé
        internal static void GameUpdate()
        {
            Main.GameUpdate();
            foreach (var action in DelGameUpdate) { action(); }
        }

        /// Lancé chaque seconde du jeu une fois que le monde est chargé
        internal static void GameUpdate1000()
        {
            Main.GameUpdate1000();
            foreach (var action in DelGameUpdate1000) { action(); }
        }

        /// Lancé une fois que le monde est déchargé
        internal static void OnGameEnd()
        {
            Main.OnGameEnd();
            foreach (var action in DelOnGameEnd) { action(); }
        }
    }


    public class AppIGConsole : PermanentApp
    {
        [HarmonyPatch(typeof(Debug), nameof(Debug.Log))]
        [HarmonyPatch(new Type[] { typeof(object) })]
        internal static class HPCatchLog
        {
            static void Prefix(object message)
            {
                if (ManApp.Instance.MyIGConsole != null && MOD_OPTIONS.IS_IG_CONSOLE)
                {
                    ManApp.Instance.MyIGConsole.stringlist.Insert(0,message.ToString());
                }
            }
        }
        [HarmonyPatch(typeof(Debug), nameof(Debug.LogWarning))]
        [HarmonyPatch(new Type[] { typeof(object) })]
        internal static class HPCatchLogWarning
        {
            static void Prefix(object message)
            {
                if (ManApp.Instance.MyIGConsole != null && MOD_OPTIONS.IS_IG_CONSOLE)
                {
                    ManApp.Instance.MyIGConsole.stringlist.Insert(0, message.ToString());
                }
            }
        }

        public List<string> stringlist;
        public AppIGConsole()
        {
            IG_only = false;
            stop_control = false;
            stringlist = new List<string>();
            Frame = UI.RfromR(0.7f, 0f, 0.3f, 0.3f, UI.ScreenFrame);
            StyleLabel.EX_LabelTransparent();
            StyleLabel.EX_SetBackGround("transparent");
            StyleLabel.normal.textColor = Color.green;
        }
            

        public override void Draw()
        {

            Rect[] listofrects = UI.RArrayFromR(true, 16, Frame);
            for (int i = 0; i < Mathf.Min(stringlist.Count, 16); i++)
            {
                GUI.Label(listofrects[i], stringlist[i], StyleLabel);
            }
        }
        public override void Update()
        {
            if (stringlist.Count > 100)
            {
                stringlist.RemoveAt(98);
            }
        }
    }

}

[HarmonyPatch(typeof(Terminal), nameof(Terminal.IsCheatsEnabled))]
internal static class HPAllowCheats
{
    static bool Prefix(ref bool __result)
    {
        if(MOD_OPTIONS.IS_CHEAT_MODE)
        {
            __result = true;
            return false;
        }
        return true;
    }
}






public class CDG
{
    ///Classe De Guidage
    public static void awake()
    {
    }

    /// Lancé chaque update du mod
    public static void update()
    {
    }

    /// Lancé chauqe udpate du mod fréquence fixe (Time.deltatime constant)
    public static void fixedUpdate()
    {
    }

    /// Lancé chaque seconde
    public static void update1000()
    {
    }

    /// Lancé à la syncronisation du serveur, si option réseau utilisée
    public static void onServerSync()
    {
    }

    /// Lancé au lancement du réseau/une fois que le monde et le personnage sont existant
    public static void onGameStart()
    {
    }

    /// Lancé chaque update du mod une fois que le monde est chargé
    public static void gameUpdate()
    {
    }

    /// Lancé chaque seconde du jeu une fois que le monde est chargé
    public static void gameUpdate1000()
    {
    }

    /// Lancé une fois que le monde est déchargé
    public static void onGameEnd()
    {
    }
}

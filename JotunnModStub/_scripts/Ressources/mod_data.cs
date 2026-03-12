using BepInEx;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using UnityEngine;
using static Character;
using Debug = UnityEngine.Debug;


using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = Jotunn.Logger;

namespace mod_data
{
    public class U
    {
        public static void awake()
        {
            Data.data_awake();
        }
        public static void update()
        {

        }
    }


    class Data
    {
        public enum DataDictionary
        {
            All,
            Icons,
            PNG
        }
        private static Dictionary<string, Texture2D> TEX_Icons = new Dictionary<string, Texture2D>();
        private static Dictionary<string, Texture2D> TEX_MyPNG = new Dictionary<string, Texture2D>();


        public static Texture2D TEX(string name, string subfolder ="", DataDictionary dict = DataDictionary.All)
        {
            string truename = name;
            if( subfolder != "") { truename = subfolder + "/" + truename; }

            if (dict == DataDictionary.All || dict == DataDictionary.PNG)
            {
                if (TEX_MyPNG.ContainsKey(truename)) { return TEX_MyPNG[truename]; }
            }

            if (subfolder == "" || dict == DataDictionary.All || dict== DataDictionary.Icons)
            {
                if(TEX_Icons.ContainsKey(truename)) { return TEX_Icons[truename]; }
            }

            return TEX_MyPNG["placeholder"];
        }


        //public static Dictionary<string, food> FOOD = new Dictionary<string, food>();

        private static string[] bundle_names = new string[]
        {
            "bundle_fx",
            "bundle_ui" 
        };


        public static AssetBundle bundle_fx;
        public static AssetBundle bundle_ui;


        public static void data_awake()
        {
            foreach (string astring in bundle_names)
            {
                bool found_match = false;
                foreach (AssetBundle bundle in AssetBundle.GetAllLoadedAssetBundles())
                {
                    Debug.Log(bundle.name);
                    if (bundle.name == astring)
                        typeof(Data).GetField(astring).SetValue(null, bundle);
                        found_match = true;
                        continue;
                }
                if (!found_match)
                {
                    load_bundle(astring);
                }
            }
            XLOG.message("lang");
            load_language_pack();
            XLOG.message("lang2");
            load_UI();
            XLOG.message("lang3");
        }

        public static void load_bundle(string bundlename)
        {
            Debug.Log(KERNEL.path_modfolder + bundlename);
            typeof(Data).GetField(bundlename).SetValue(null, AssetUtils.LoadAssetBundle(KERNEL.path_modfolder + bundlename));
            AssetBundle bundle = typeof(Data).GetField(bundlename).GetValue(null) as AssetBundle;
            //bundle = AssetUtils.LoadAssetBundle("SifModAssets/" + bundlename);
            /*string[] namecodes = new string[] { "English", "French" };

            foreach (string astring in namecodes)
            {
                TextAsset text = bundle.LoadAsset<TextAsset>(astring);
                if (text != null)
                {
                    LocalizationManager.Instance.AddJson(astring, text.ToString());
                }
            }*/
        }

        public static void load_language_pack()
        {
            return;

            string path = "BepInEx/plugins/Localization/French/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DirectoryInfo Dir_French = new DirectoryInfo(path);
            FileInfo[] info = Dir_French.GetFiles("*.json");
            foreach (FileInfo infofile in info)
            {
                string filedata = File.ReadAllText(infofile.FullName);
                LocalizationManager.Instance.AddJson("French", filedata) ;
            }
        }
        


        public static void load_UI()
        {
            string path = "BepInEx/plugins/" + KERNEL.foldername + "/raw_data/icons/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string path2 = "BepInEx/plugins/"+ KERNEL.foldername + "/raw_data/custom_icons/";
            if (!Directory.Exists(path2))
            {
                Directory.CreateDirectory(path2);
            }
            string path3 = "BepInEx/plugins/" + KERNEL.foldername + "/PNG/";
            if (!Directory.Exists(path3))
            {
                Directory.CreateDirectory(path3);
            }

            //FILL ICON DICTIONARY
            DirectoryInfo icondir = new DirectoryInfo(path);
            FileInfo[] info = icondir.GetFiles("*.png");
            foreach (FileInfo infofile in info)
            {
                TEX_Icons.Add(infofile.Name.ToLower().Replace(".png", ""), AssetUtils.LoadTexture(infofile.FullName));
            }


            DirectoryInfo customicondir = new DirectoryInfo(path2);
            info = customicondir.GetFiles("*.png");
            foreach (FileInfo infofile in info)
            {
                
                Texture2D tex = AssetUtils.LoadTexture(infofile.FullName);

                string key = infofile.Name.ToLower().Replace(".png", "");
                if (!TEX_Icons.ContainsKey(key)) { TEX_Icons.Add( key, tex); }
            }



            //FILL MyPNG DICTIONARY

            DirectoryInfo mypngdir = new DirectoryInfo(path3);
            info = mypngdir.GetFiles("*.png");
            DirectoryInfo[] dirinfos = mypngdir.GetDirectories();
            foreach (FileInfo infofile in info)
            {
                TEX_MyPNG.Add(infofile.Name.ToLower().Replace(".png", ""), AssetUtils.LoadTexture(infofile.FullName));
            }


            foreach(DirectoryInfo dirinfo in dirinfos)
            {
                DirectoryInfo mysubpngdir = new DirectoryInfo(dirinfo.FullName+"/");
                info = mysubpngdir.GetFiles("*.png");
                foreach (FileInfo infofile in info)
                {
                    string name = infofile.Name.ToLower().Replace(".png", "");
                    if(name.Contains(",")) { name = name.Split(',')[3]; }
                    TEX_MyPNG.Add(dirinfo.Name.ToLower()+"/"+name, AssetUtils.LoadTexture(infofile.FullName));
                }
            }

            UI.Load_UI();
        }







        public static void load_assets()
        {
            XLOG.warning("ASSETS", "Loading FX...");
            load_FX_prefabs();
            XLOG.warning("ASSETS", "Loading Music...");
            //load_Extra_Music();

        }


        /*public static void load_Extra_Music()
        {
            string[] listofclips = FX.namedmusiclist;
            foreach(string clipname in listofclips)
            {
                AudioClip clip = bundle_fx.LoadAsset<AudioClip>(clipname);

                MusicMan.NamedMusic _music = new MusicMan.NamedMusic();

                _music.m_name = clipname;
                _music.m_clips = new AudioClip[1] { clip };
                _music.m_alwaysFadeout = true;
                _music.m_loop = true;
                _music.m_ambientMusic = true;
                MusicMan.instance.m_music.Add(_music );
            }
        }*/
        public static void load_FX_prefabs()
        {
            GameObject[] prefabs = bundle_fx.LoadAllAssets<GameObject>();

            foreach(GameObject obj in prefabs)
            {
                XLOG.message(obj.name);
                PrefabManager.Instance.AddPrefab(obj);
            }
        }

        //PascalCase or camelCase string into lowercase snake_case.
        public static string SmallifyName(string astring)
        {
            string r = "";
            string[] words = Regex.Split(astring, @"(?<!^)(?=[A-Z])");
            for(int k = 0; k < words.Length; k++)
            {
                r += words[k].ToLower();
                if(k != words.Length - 1) { r += "_"; }
            }
            return r;
        }

        


        public static Texture2D AddWatermark(Texture2D background, Texture2D watermark, Color tcolor)
        {
            Texture2D returner = new Texture2D(background.width,background.height);

            int startX = 0;
            int startY = background.height - watermark.height;

            for (int x = startX; x < background.width; x++)
            {

                for (int y = startY; y < background.height; y++)
                {
                    Color bgColor = background.GetPixel(x, y);
                    Color wmColor = watermark.GetPixel(x - startX, y - startY);
                    wmColor = new Color(wmColor.r * tcolor.r, wmColor.g * tcolor.g, wmColor.b * tcolor.b, wmColor.a * tcolor.a);
                    wmColor = new Color(wmColor.r * wmColor.a, wmColor.g * wmColor.a, wmColor.b * wmColor.a, wmColor.a);

                    Color final_color = bgColor + wmColor;

                    returner.SetPixel(x, y, final_color);
                }
            }

            returner.Apply();
            return returner;
        }
        public static Texture2D AddWatermark(Texture2D background, Texture2D watermark)
        {
            Texture2D returner = new Texture2D(background.width, background.height);

            int startX = 0;
            int startY = background.height - watermark.height;

            for (int x = startX; x < background.width; x++)
            {

                for (int y = startY; y < background.height; y++)
                {
                    Color bgColor = background.GetPixel(x, y);
                    Color wmColor = watermark.GetPixel(x - startX, y - startY);
                    wmColor = new Color(wmColor.r * wmColor.a, wmColor.g * wmColor.a, wmColor.b * wmColor.a, wmColor.a);

                    Color final_color = bgColor + wmColor;

                    returner.SetPixel(x, y, final_color);
                }
            }

            returner.Apply();
            return returner;
        }
        public static Texture2D AddWatermark(Texture2D background, Texture2D watermark, Rect inputrect)
        {
            Rect arect = new Rect(inputrect.x, background.height -inputrect.y - inputrect.height, inputrect.width, inputrect.height);

            Texture2D copyTexture = new Texture2D(watermark.width, watermark.height);
            copyTexture.SetPixels(watermark.GetPixels());
            copyTexture.Apply();
            copyTexture = Resizer(copyTexture, (int)arect.width, (int)arect.height);

            Texture2D returner = new Texture2D(background.width, background.height);

            int startX = 0 ;
            int startY = 0;

            for (int x = startX; x < background.width; x++)
            {

                for (int y = startY; y < background.height; y++)
                {

                    Color bgColor;
                    Color wmColor;
                    Color final_color;
                    if (x < arect.x || x >= (arect.x + arect.width) || y < arect.y || y >= (arect.y + arect.height))
                    {
                        bgColor = background.GetPixel(x, y);

                        final_color = bgColor;
                    }
                    else
                    {
                        int relx = x - (int)arect.x;
                        int rely = y - (int)arect.y;
                        bgColor = background.GetPixel(x, y);
                        wmColor = copyTexture.GetPixel(relx, rely);
                        if (wmColor.a < 0.1f)
                        {
                            final_color = bgColor;
                        }
                        else
                        {
                            final_color = wmColor;
                        }

                        //final_color = wmColor;
                        //final_color = bgColor + wmColor;
                        //final_color = Color.black;
                    }

                    returner.SetPixel(x, y, final_color);
                }
            }


            returner.Apply();
            return returner;
        }
        static Texture2D Resizer(Texture2D texture2D, int targetX, int targetY)
        {
            RenderTexture rt = new RenderTexture(targetX, targetY, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);
            Texture2D result = new Texture2D(targetX, targetY);
            result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
            result.Apply();
            return result;
        }
    }



}
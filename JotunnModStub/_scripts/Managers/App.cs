using BepInEx;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using Steamworks;
using UnityEngine;

using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Reflection;
using mod_data;
using System.CodeDom;
using static TextViewer;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using System.ComponentModel;
using Unity.Profiling;
using Data = mod_data.Data;
using static mod_data.Data;
using static UI;
using System.Xml.Linq;
using UnityEngine.Experimental.GlobalIllumination;
using System.Security.Policy;
using System.IO;
using JetBrains.Annotations;
using Unity.Jobs;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class App
{
    #region AppDefinition

    Texture2D TEX(string name, string subfolder = "", DataDictionary dict = DataDictionary.All)
    {
        return mod_data.Data.TEX(name, subfolder, dict);
    }
    public virtual string loc_name { get; set; } = "";
    public virtual bool is_env_dependant { get; set; } = false;
    public virtual bool is_unique { get; set; } = true;
    public virtual bool stop_control { get; set; } = true;
    public virtual bool IG_only { get; set; } = true;
    public virtual bool Switch_OnOff { get; set; } = false;
    public virtual bool is_immune_escape { get; set; } = false;

    

    protected virtual Rect Frame { get; set; } = UI.ScreenFrame;
    protected virtual Rect TopHalf { get { return UI.RfromR(0, 0, 1, 0.5f, Frame); } }
    protected virtual Rect BottomHalf { get { return UI.RfromR(0, 0.5f, 1, 0.5f, Frame); } }
    protected virtual Rect LeftHalf { get { return UI.RfromR(0, 0, 0.5f, 1, Frame); } }
    protected virtual Rect RightHalf { get { return UI.RfromR(0.5f, 0, 0.5f, 1, Frame); } }
    protected virtual Rect DEBUG_FRAME { get; set; } = new Rect(0, 0, UI.X / 4f, UI.Y / 4f);
    protected virtual GUIStyle StyleLabel { get; set; } = new GUIStyle().EX_DefaultLabel();
    protected virtual GUIStyle StyleButton { get; set; } = new GUIStyle().EX_DefaultButton();
    protected virtual GUIStyle StyleScroll { get; set; } = UI.DefaultSkin.GetStyle("verticalscrollbar");

    internal bool ToBeRemoved = false;

    public App()
    {
        loc_name = "$" + this.GetType().Name.ToLower() + "_";
    }

    public string localize_keyword(string keyword) { return Localization.instance.Localize("$event_" + (this.GetType().Name).ToLower() + "_" + keyword); }
    public virtual void Draw()
    {
        if (is_env_dependant)
        {
            UpdateEnvColor();
        }
    }
    public virtual void DrawPriority()
    {
        if (is_env_dependant)
        {
            UpdateEnvColor();
        }
    }

    public virtual void DrawBackground(string folder, params string[] pngs)
    {
        Color _old = GUI.backgroundColor;
        GUIStyle astyle = new GUIStyle().EX_DefaultLabel();
        if (is_env_dependant)
        {
            UpdateEnvColor();
        }
        Rect Screen = new Rect(0, 0, UI.X, UI.Y);
        foreach (string astring in pngs)
        {
            astyle.normal.background = TEX(astring, folder);
            GUI.Label(Screen, "", astyle);
        }

        GUI.backgroundColor = _old;
    }
    public void UpdateEnvColor()
    {
        GUI.backgroundColor = Color.Lerp(EnvMan.instance.GetCurrentEnvironment().m_ambColorDay, EnvMan.instance.GetCurrentEnvironment().m_ambColorNight, 2 * Math.Abs(EnvMan.instance.GetDayFraction() - 0.5f));
    }

    public virtual void Update()
    {

    }

    public virtual void QuitApp()
    {
        ToBeRemoved = true;
    }


    public void LABEL(Rect arect, string astring, GUIStyle ASTYLE, bool noloc = false)
    {
        if(noloc)
        {
            GUI.Label(arect, Localization.instance.Localize(astring), ASTYLE);
        }
        else
        {
            GUI.Label(arect, Localization.instance.Localize(loc_name + astring), ASTYLE);
        }
    }
    public bool BUTTON(Rect arect, string astring, GUIStyle ASTYLE, bool noloc = false)
    {
        if (noloc)
        {
            return GUI.Button(arect, Localization.instance.Localize(astring), ASTYLE);
        }
        else
        {
            return GUI.Button(arect, Localization.instance.Localize(loc_name + astring), ASTYLE);
        }
    }


    #endregion







    #region AppToolTip

    public class AppToolTip : PermanentApp
    {
        public static string m_text = "";
        public static string m_title = "";
        public AppToolTip()
        {
            stop_control = false;
            is_env_dependant = false;
            StyleLabel.EX_LabelTransparent();
            StyleLabel.normal.background = TEX("dark");
        }

        public override void Update()
        {
            base.Update();



        }
        public override void DrawPriority()
        {
            base.DrawPriority();
            if (m_text == "") { return; }


            string loc_title = Localization.instance.Localize(m_title);
            string loc_text = Localization.instance.Localize(m_text);


            int numoflines = loc_text.Split('\n').Length - 1 + 4;

            int titlesize = loc_title.Length*2;
            if (numoflines > 0)
            {
                titlesize = Math.Max(titlesize, loc_text.Split('\n').OrderByDescending(x => x.Length).ToList()[0].Length);
            }
            else
            {
                titlesize = Math.Max(titlesize, loc_text.Length);
            }

            float width =titlesize / 45f * 318 * UI.X / 1920f * 1.2f;

            float height = 400 / 22f * numoflines;
            float titleheight = 400 / 22f * numoflines;

            Vector2 mousepos = UI.Mpos();
            Rect therect = new Rect(mousepos.x, mousepos.y + 25, width, height);

            StyleLabel.EX_LabelTransparent();
            StyleLabel.normal.background = TEX("dark");
            GUI.Label(therect, "", StyleLabel);

            StyleLabel.EX_LabelTransparent();
            StyleLabel.fontSize = Font(24);
            StyleLabel.normal.textColor = GUIManager.Instance.ValheimOrange;
            StyleLabel.alignment = TextAnchor.UpperCenter;
            GUI.Label(therect, loc_title, StyleLabel);
            StyleLabel.fontSize = Font(16);
            StyleLabel.normal.textColor = Color.white;
            StyleLabel.alignment = TextAnchor.UpperLeft;
            GUI.Label(therect, Localization.instance.Localize("\n\n" + loc_text), StyleLabel);
        }
    }

    #endregion


}

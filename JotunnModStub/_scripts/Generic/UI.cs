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
using System.Diagnostics.Eventing.Reader;
using TMPro;
using System.Runtime.CompilerServices;
using static UnityEngine.UI.Image;
using static App;
using System.Dynamic;

//UI HARMONY PATCHES



//STOCKAGE DES FONCTIONS ET VARIABLES GENERIQUES POUR L'UI
public class UI
{   


    public static int Font(int font1080)
    {
        return (int)Math.Round(font1080 * (Y / 1080f));
    }
    public static string money_string(float val)
    {
        int int_share = (int)Math.Floor(val);
        int float_share =  (int)Math.Round((val - int_share) * 100);

        if(float_share == 100) { int_share += 1; float_share = 0; }

        string string_int_value = int_share.ToString();
        string string_float_value = float_share.ToString();
        if (string_float_value.ToString().Length == 1 ) { string_float_value = "0" + string_float_value; }

        List<char>  rev_list_int = (from a in string_int_value select a).ToList();
        var IndexList = Enumerable.Range(0, rev_list_int.Count).Where(x => (x - rev_list_int.Count) % 3 == 0 && x != 0);
        IndexList.OrderBy(x => -x).ToList().ForEach(x => rev_list_int.Insert(x, ','));

        if (float_share == 0) { return string.Concat(rev_list_int); }
        else { return string.Concat(rev_list_int) + "." + float_share; }
        
        
    }
    public static string money_string_full(float val) => money_string(val) + " " + currency;   

    public static string currency = "₱";

    public static float X = Screen.width;
    public static float Y = Screen.height;
    public static Rect ScreenFrame { get { return new Rect(0,0,X,Y); } }

    public static GUISkin DefaultSkin;


    public class ColorPalette
    {
        public static Color PaleBrown = Color.black.EX_RGB(255, 225, 140);
        public static Color Brown = Color.black.EX_RGB(100, 70, 30);
        public static Color Transparant = Color.black.EX_RGB(255, 255, 255, 0);
        public static Color DarkBlue = Color.black.EX_RGB(0, 25, 75);
        public static Color Gold = Color.black.EX_RGB(150, 120, 30);
        public static Color Silver = Color.black.EX_RGB(180, 180, 180);
    }
    //private static bool _market = false;
    /*public static bool market
    {
        get { return _market; }
        set
        {
            _market = value;
            UI.update_input_lock();
        }
    }*/

    public static Rect[] RArrayFromR(bool isrows, int num_divide, Rect arect)
    {
        float div_val = 1f / (num_divide * 1f);
        Rect[] returner = new Rect[] {};
        if(isrows)
        {
            return (from a in Enumerable.Range(0, num_divide) select RfromR(0,a * div_val, 1, div_val, arect)).ToArray();
        }
        else
        {
            return (from a in Enumerable.Range(0, num_divide) select RfromR(a * div_val,0,div_val,1, arect)).ToArray();
        }
        
    }
    public static Rect[,] RGridFromR(int rows, int columns, Rect arect)
    {
        Rect[,] returner = new Rect[rows, columns];

        float col_size = arect.width / (1f * columns);
        float row_size = arect.height / (1f * rows);

        for(int y = 0; y < rows; y++)
        {
            for(int x = 0; x < columns; x++)
            {
                returner[y,x] = new Rect(arect.x+ x *col_size,arect.y+ y * row_size,col_size,row_size);
            }
        }

        return returner;
    }
    public static Rect RfromCenter(float x, float y, float w, float h)
    {
        int halfw = (int)Math.Round(w / 2f);
        int halfh = (int)Math.Round(h / 2f);

        return new Rect(x - halfw, y - halfh, halfw * 2, halfh * 2);
    }
    public static Rect RfromR(float rx, float ry, float rw, float rh, Rect arect)
    {
        return new Rect(arect.x + rx * arect.width, arect.y + ry * arect.height, arect.width * rw, arect.height * rh);
    }
    public static Rect RRatio(float ratio, Rect arect)
    {
        return RfromCenter(arect.x + arect.width / 2f, arect.y + arect.height / 2f, arect.width * ratio, arect.height * ratio);
    }
    public static Rect Rfrom1080p(float x, float y, float w, float h)
    {
        float rx = X / (1920f);
        float ry = Y / (1080f);
        return new Rect(x * rx, y * ry, w * rx, h * ry);
    }
    public static Rect Rfrom1440p(float x, float y, float w, float h)
    {
        float rx = X / (2560f);
        float ry = Y / (1440f);
        return new Rect(x * rx, y * ry, w * rx, h * ry);
    }
    public static Rect RfromRGrid(int x_index, int y_index, int n_col, int n_row, Rect[,] Rgrid)
    {
        if (y_index + n_row >  Rgrid.GetLength(0)) { XLOG.error("Incompatible Rect Grid Size", "UI"); return default; }
        if (x_index + n_col > Rgrid.GetLength(1)) { XLOG.error("Incompatible Rect Grid Size", "UI"); return default; }
        return new Rect(Rgrid[y_index, x_index].x, Rgrid[y_index, x_index].y, Rgrid[0, 0].width * n_col, Rgrid[0, 0].height * n_row);
    }
    public static (Vector2, Vector2) PixelToCenterDelta(Rect arect)
    {
        (Vector2,Vector2) returner = (Vector2.zero, Vector2.zero);

        Vector2 REF_FRAME = new Vector2(1620, 912);

        Vector2 CenterPixelPos = new Vector2(arect.x + arect.width / 2f, arect.y + arect.height / 2f);
        Vector2 PixelVectFromCenter = CenterPixelPos - new Vector2(UI.X / 2f, UI.Y / 2f);

        returner.Item1 = new Vector2(PixelVectFromCenter.x / (1f * UI.X) * REF_FRAME.x, -(PixelVectFromCenter.y / (1f * UI.Y) * REF_FRAME.y));

        returner.Item2 = new Vector2(arect.width / (1f * UI.X) * REF_FRAME.x, (arect.height / (1f * UI.Y) * REF_FRAME.y));

        return returner;
    }
    
    public static string ratiotoP(float ratio)
    {
        string returner = (ratio * 100).ToString();
        if (returner.Length > 4)
        {
            returner = returner.Substring(0, 4);
        }
        return returner + "%";

    }
    public static bool isMinR(Rect arect)
    {
        return arect.Contains(Mpos());
    }
    public static bool isInRect(Vector2 pos, Rect arect)
    {
        return (arect.x < pos.x && pos.x < arect.x + arect.width) && (arect.y < pos.y && pos.y < arect.y + arect.height);
    }
    public static bool isRectInScreen(Rect arect)
    {
        Rect screen = new Rect(0,0,X,Y);
        Vector2 A = new Vector2(arect.x, arect.y);
        Vector2 B = new Vector2(arect.x + arect.width, arect.y);
        Vector2 C = new Vector2(arect.x, arect.y + arect.height);
        Vector2 D = new Vector2(arect.x + arect.width, arect.y + arect.height);
        return isInRect(A, screen) || isInRect(B, screen) || isInRect(C, screen) || isInRect(D, screen);
    }

    public static Vector2 Mpos()
    {
        return new Vector2(Input.mousePosition.x, Y - Input.mousePosition.y);
    }
    public static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }



    public static void update_screen()
    {
        UI.X = Screen.width;
        UI.Y = Screen.height;
        
    }
    /*public static void update_input_lock()
    {
        inmenu = money | market | cheat | job | bonus | cook_package;//| compass;
        if (inmenu)
        {
            GUIManager.BlockInput(inmenu);
        }
        else
        {
            GUIManager.BlockInput(inmenu);

            typeof(GUIManager).GetMethod("ResetInputBlock", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { });
        }
    }*/

    public static void Load_UI()
    {
        DefaultSkin = new GUISkin();
        //DefaultSkin = Data.bundle_ui.LoadAsset<GUISkin>("DefaultSkin");

    }
    
}

public class PermanentApp : App
{
    protected bool show { get; set; }

    public bool Getter_show => show;


    public override void Draw()
    {
        if(!show) { return; }

        base.Draw();
    }
    public virtual void Show()
    {
        show = true;
    }
    public virtual void Hide()
    {
        show = false;
    }


    public override void QuitApp()
    {

    }
}

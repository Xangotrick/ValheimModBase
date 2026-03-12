using BepInEx;
using HarmonyLib;
using Jotunn.Managers;
using Steamworks;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Jotunn.Entities;
using System.Security.Cryptography;




public static class StringExtension
{
    public static string EX_ReplaceAt(this string input, int index, char newChar)
    {
        char[] chars = input.ToCharArray();
        chars[index] = newChar;
        return new string(chars);
    }

    public static string EX_KeepNumeric(this string input)
    {
        char[] charsallowed = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };
        string returner = new string(input.Where(x => charsallowed.Contains(x)).ToArray());
        if (returner.Split('.').Length > 1)
        {
            string[] split = returner.Split('.');

            string fixer = split[1];
            if (split[1].Length > 2)
            {
                fixer = split[1][0].ToString() + split[1][1].ToString();
            }
            returner = split[0] + "." + fixer;
        }
        return returner;
    }

}
public static class FloatExtension
{
    public static string EX_asTimeString(this float input)
    {
        int fixed_time = Mathf.FloorToInt(input);
        if (input < 60)
        {
            return (fixed_time).ToString() + "s";
        }
        else if (input < 3600)
        {
            int seconds = fixed_time % 60;
            int minutes = (fixed_time - seconds) / 60;
            return (minutes.ToString() + "m" + seconds.ToString() + "s");
        }
        else
        {
            int seconds = fixed_time % 60;
            int minutes_tot = (fixed_time - seconds) / 60;
            int minutes = minutes_tot % 60;
            int hours = (minutes_tot - minutes) / 60;

            return (hours.ToString() + "h" + minutes.ToString() + "m" + seconds.ToString() + "s");
        }
    }

}
public static class IntExtension
{
    public static string EX_asTimeString(this int input)
    {
        int fixed_time = Mathf.FloorToInt(input);
        if (input < 60)
        {
            return (fixed_time).ToString() + "s";
        }
        else if (input < 3600)
        {
            int seconds = fixed_time % 60;
            int minutes = (fixed_time - seconds) / 60;
            return (minutes.ToString() + "m" + seconds.ToString() + "s");
        }
        else
        {
            int seconds = fixed_time % 60;
            int minutes_tot = (fixed_time - seconds) / 60;
            int minutes = minutes_tot % 60;
            int hours = (minutes_tot - minutes) / 60;

            return (hours.ToString() + "h" + minutes.ToString() + "m" + seconds.ToString() + "s");
        }
    }

    public static string EX_asRomanString(this int input)
    {
        switch (input)
        {
            case 1: return "I";
            case 2: return "II";
            case 3: return "III";
            case 4: return "IV";
            case 5: return "V";
            case 6: return "VI";
            case 7: return "VII";
            case 8: return "VIII";
            case 9: return "IX";
            case 10: return "X";
            case 11: return "XI";
            case 12: return "XII";
            case 13: return "XIII";
            case 14: return "XIV";
            case 15: return "XV";
            case 16: return "XVI";
            case 17: return "XVII";
            case 18: return "XVIII";
            case 19: return "XIX";
            case 20: return "XX";
            case 21: return "XXI";
        }
        return "N";
    }

}
public static class PrefabManagerExtension
{
    public static T GetComp<T>(this PrefabManager man, string item_name) where T : Component
    {
        GameObject prefab = man.GetPrefab(item_name);
        if (prefab == null) { return null; }
        return prefab.GetComponent<T>();
    }
}
public static class RectExtension
{
    public static bool EX_IntersectsRect(this Rect this_rect, Rect otherRect)
    {
        Vector2 A = new Vector2(this_rect.xMin, this_rect.yMin);
        Vector2 B = new Vector2(this_rect.xMax, this_rect.yMin);
        Vector2 C = new Vector2(this_rect.xMin, this_rect.yMax);
        Vector2 D = new Vector2(this_rect.xMax, this_rect.yMax);

        if (UI.isInRect(A, otherRect)) { return true; }
        if (UI.isInRect(B, otherRect)) { return true; }
        if (UI.isInRect(C, otherRect)) { return true; }
        if (UI.isInRect(D, otherRect)) { return true; }

        return false;
    }

}
public static class ExtendedVector3
{
    public static Vector3 EX_xzplane(this Vector3 vect)
    {
        return new Vector3(vect.x, 0, vect.z);
    }
}
public static class ExtendedColor
{
    /// <summary>
    /// Modifies the color through RGB 0 - 255 
    /// </summary>
    /// <param name="COLOR"></param>
    /// <param name="R"></param>
    /// <param name="G"></param>
    /// <param name="B"></param>
    /// <param name="A"></param>
    /// <returns>Returns the modified color</returns>
    public static Color Lerp3(Color color1, Color color2, Color color3, float amount)
    {
        if (amount < 0.5f)
        {
            return Color.Lerp(color1, color2, 2 * amount);
        }
        else
        {
            return Color.Lerp(color2, color3, 2 * amount - 1);
        }
    }

    public static Color EX_RGB(this Color COLOR, int R, int G, int B, int A = 255)
    {
        COLOR.r = R / 255f;
        COLOR.g = G / 255f;
        COLOR.b = B / 255f;
        COLOR.a = A / 255f;
        return COLOR;
    }

    /// <summary>
    /// Modifies the color and makes it translucent with a modified alpha value
    /// </summary>
    /// <param name="COLOR"></param>
    /// <param name="R"></param>
    /// <param name="G"></param>
    /// <param name="B"></param>
    /// <param name="A"></param>
    /// <returns>Returns the modified color</returns>
    public static Color EX_Alpha(this Color COLOR, int alpha)
    {


        COLOR.a = alpha / 255f;
        return COLOR;
    }

}
public static class Extended_StylePalette
{
    public static GUIStyle EX_DefaultLabel(this GUIStyle astyle)
    {
        astyle = new GUIStyle(UI.DefaultSkin.label);
        astyle.normal.background = UI.MakeTex(16, 16, new Color().EX_RGB(255, 255, 255));
        astyle.fontSize = UI.Font(20);
        astyle.font = UI.DefaultSkin.font;
        return astyle;
    }
    public static GUIStyle EX_DefaultButton(this GUIStyle astyle)
    {
        astyle = new GUIStyle(UI.DefaultSkin.button);
        astyle.normal.background = UI.MakeTex(16, 16, new Color().EX_RGB(200, 200, 200));
        astyle.hover.background = UI.MakeTex(16, 16, new Color().EX_RGB(150, 150, 150));
        astyle.active.background = UI.MakeTex(16, 16, new Color().EX_RGB(230, 230, 230));
        astyle.fontSize = 20;
        astyle.font = UI.DefaultSkin.font;
        return astyle;
    }

    public static GUIStyle EX_CobaltLabel(this GUIStyle astyle)
    {
        astyle.normal.background = UI.MakeTex(16, 16, new Color().EX_RGB(72, 74, 88));
        astyle.fontSize = UI.Font(20);
        astyle.font = UI.DefaultSkin.font;
        return astyle;
    }
    public static GUIStyle EX_CobaltButton(this GUIStyle astyle)
    {
        astyle.normal.background = UI.MakeTex(16, 16, new Color().EX_RGB(110, 115, 170));
        astyle.hover.background = UI.MakeTex(16, 16, new Color().EX_RGB(115, 120, 175));
        astyle.active.background = UI.MakeTex(16, 16, new Color().EX_RGB(100, 105, 160));
        astyle.fontSize = UI.Font(20);
        astyle.font = UI.DefaultSkin.font;
        return astyle;
    }


    public static GUIStyle EX_Button01(this GUIStyle astyle, bool frozen = false)
    {
        astyle.normal.background = mod_data.Data.TEX("button01n");
        astyle.hover.background = mod_data.Data.TEX("button01h");
        astyle.active.background = mod_data.Data.TEX("button01c");
        if (frozen)
        {
            astyle.normal.background = mod_data.Data.TEX("button01n");
            astyle.hover.background = mod_data.Data.TEX("button01n");
            astyle.active.background = mod_data.Data.TEX("button01n");
        }
        astyle.fontSize = UI.Font(20);
        astyle.font = UI.DefaultSkin.font;
        return astyle;
    }
    public static GUIStyle EX_Button02(this GUIStyle astyle, bool frozen = false)
    {
        astyle.normal.background = mod_data.Data.TEX("button02n");
        astyle.hover.background = mod_data.Data.TEX("button02h");
        astyle.active.background = mod_data.Data.TEX("button02c");
        if (frozen)
        {
            astyle.normal.background = mod_data.Data.TEX("button02n");
            astyle.hover.background = mod_data.Data.TEX("button02n");
            astyle.active.background = mod_data.Data.TEX("button02n");
        }
        astyle.fontSize = UI.Font(20);
        astyle.font = UI.DefaultSkin.font;
        return astyle;
    }
    public static GUIStyle EX_Button(this GUIStyle astyle,int num, bool frozen = false)
    {
        string folder_name = "button" + num.ToString();
        astyle.normal.background = mod_data.Data.TEX("n",folder_name);
        astyle.hover.background = mod_data.Data.TEX("h", folder_name);
        astyle.active.background = mod_data.Data.TEX("c", folder_name);
        if (frozen)
        {
            astyle.normal.background = mod_data.Data.TEX("n", folder_name);
            astyle.hover.background = mod_data.Data.TEX("n", folder_name);
            astyle.active.background = mod_data.Data.TEX("n", folder_name);
        }
        astyle.fontSize = UI.Font(20);
        astyle.font = UI.DefaultSkin.font;
        return astyle;
    }

    public static GUIStyle EX_Button01wide(this GUIStyle astyle, bool frozen = false)
    {
        astyle.normal.background = mod_data.Data.TEX("button01nwide");
        astyle.hover.background = mod_data.Data.TEX("button01hwide");
        astyle.active.background = mod_data.Data.TEX("button01cwide");
        if (frozen)
        {
            astyle.normal.background = mod_data.Data.TEX("button01nwide");
            astyle.normal.background = mod_data.Data.TEX("button01nwide");
            astyle.normal.background = mod_data.Data.TEX("button01nwide");
        }
        astyle.fontSize = UI.Font(20);
        astyle.font = UI.DefaultSkin.font;
        return astyle;
    }
    public static GUIStyle EX_ButtonTrans(this GUIStyle astyle, bool frozen = false)
    {
        astyle.normal.background = mod_data.Data.TEX("transparent");
        astyle.hover.background = mod_data.Data.TEX("button_trans_h");
        astyle.active.background = mod_data.Data.TEX("button_trans_c");
        if (frozen)
        {
            astyle.normal.background = mod_data.Data.TEX("transparent");
            astyle.hover.background = mod_data.Data.TEX("transparent");
            astyle.hover.background = mod_data.Data.TEX("transparent");
        }
        astyle.fontSize = UI.Font(20);
        astyle.font = UI.DefaultSkin.font;
        return astyle;
    }

    public static GUIStyle EX_LabelWood(this GUIStyle astyle)
    {
        astyle.normal.background = mod_data.Data.TEX("wood_panel");
        astyle.fontSize = UI.Font(20);
        astyle.font = UI.DefaultSkin.font;
        return astyle;
    }
    public static GUIStyle EX_SetBackGround(this GUIStyle astyle, string pngfile)
    {
        astyle.normal.background = mod_data.Data.TEX(pngfile);
        astyle.hover.background = mod_data.Data.TEX(pngfile);
        astyle.active.background = mod_data.Data.TEX(pngfile);
        return astyle;
    }
    public static GUIStyle EX_LabelTransparent(this GUIStyle astyle)
    {
        //astyle = new GUIStyle(UI.DefaultSkin.label);
        astyle.normal.background = mod_data.Data.TEX("transparent");
        astyle.fontSize = UI.Font(20);
        astyle.font = UI.DefaultSkin.font;
        return astyle;
    }
}

public static class CharacterExtension
{
    /// <summary>
    /// Force l'animation d'un humanoide, par un "trigger".
    /// </summary>
    /// <param name="humanoid"></param>
    /// <param name="trigger"></param>
    public static void EX_SetTrigger(this Character humanoid, string trigger)
    {
        ZSyncAnimation m_zanim = (ZSyncAnimation)Traverse.Create(humanoid).Field("m_zanim").GetValue();

        m_zanim.SetTrigger(trigger);
    }
}
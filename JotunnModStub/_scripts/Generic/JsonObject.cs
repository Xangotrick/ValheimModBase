using BepInEx;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using Steamworks;
using System;
using UnityEngine;

[Serializable]
public class JsonObject
{

    public string serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public static string serialize(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    public static T deserialize<T>(string astring)
    {
        return (JsonUtility.FromJson<T>(astring));
    }


}


//I don't know what this is for?
/*

[Serializable]
public struct Vector2S
{
    public float x;
    public float y;

    public Vector2S(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Vector2S))
        {
            return false;
        }

        var s = (Vector2S)obj;
        return x == s.x &&
               y == s.y;
    }


    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }

    public static bool operator ==(Vector2S a, Vector2S b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Vector2S a, Vector2S b)
    {
        return a.x != b.x && a.y != b.y;
    }

    public static implicit operator Vector2(Vector2S x)
    {
        return new Vector2(x.x, x.y);
    }

    public static implicit operator Vector2S(Vector2 x)
    {
        return new Vector2S(x.x, x.y);
    }

    public override string ToString()
    {
        return $"({x},{y})";
    }
}

*/


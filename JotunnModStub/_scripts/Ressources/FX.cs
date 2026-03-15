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

class FX
{
    
    /// <summary>
    /// Array of names of custom musics we want to add
    /// </summary>
    
    //public static string[] namedmusiclist = new string[] { "bg1_sewers", "kaldeim" };

    /// <summary>
    /// Instantiates an effect prefab with the given name at a given position
    /// </summary>
    /// <param name="sfx_name"></param>
    /// <param name="pos"></param>
    /// <returns>Returns the GameObject of the instantiated effect</returns>
    /// 
    public static GameObject Play(RefSFX REF, Vector3? pos = null)
    {
        return Play(REF.ToString(), pos);
    }
    public static GameObject Play(RefVFX REF, Vector3? pos = null)
    {
        return Play(REF.ToString(), pos);
    }
    public static GameObject Play(string sfx_name, Vector3? pos = null)
    {
        if (pos == null) { pos = Player.m_localPlayer.transform.position; }
        GameObject fx = PrefabManager.Instance.GetPrefab(sfx_name);
        if (fx)
        {
            return UnityEngine.Object.Instantiate(fx, (Vector3)pos, Quaternion.identity);
        }
        else
        {
            XLOG.error("FX: '" + sfx_name + "' was not found.", "FX");
        }
        return null;
    }


    private static EffectList.EffectData GetFXData(string fx_name)
    {
        EffectList.EffectData returner = new EffectList.EffectData();

        returner.m_prefab = PrefabManager.Instance.GetPrefab(fx_name);
        returner.m_enabled = true;

        return returner;
    }

    /// <summary>
    /// Creates an EffectList from a list of Effect Prefab names
    /// </summary>
    /// <param name="fx_names"></param>
    /// <returns>Returns the resulting EffectList</returns>
    public static EffectList GetFXList(params string[] fx_names)
    {
        EffectList returner = new EffectList();
        returner.m_effectPrefabs = new EffectList.EffectData[fx_names.Length];

        for (int index = 0; index < fx_names.Length; index++)
        {
            returner.m_effectPrefabs[index] = GetFXData(fx_names[index]);
        }

        return returner;
    }


    public static FootStep.StepEffect GetFootFXList(string name, FootStep.MotionType type_motion, FootStep.GroundMaterial ground_mat, params string[] fx_names)
    {
        FootStep.StepEffect returner = new FootStep.StepEffect();
        returner.m_name = name;
        returner.m_motionType = type_motion;
        returner.m_material = ground_mat;
        returner.m_effectPrefabs = new GameObject[fx_names.Length];

        for (int index = 0; index < fx_names.Length; index++)
        {
            returner.m_effectPrefabs[index] = PrefabManager.Instance.GetPrefab(fx_names[index]);
        }

        return returner;
    }

    public static void AddFXList(EffectList an_effect_list, params string[] names)
    {

        if (an_effect_list.m_effectPrefabs == null) { an_effect_list.m_effectPrefabs = new EffectList.EffectData[0]; }

        List<EffectList.EffectData> data_list = an_effect_list.m_effectPrefabs.ToList();

        foreach (string astring in names) { data_list.Add(GetFXData(astring)); }

        an_effect_list.m_effectPrefabs = data_list.ToArray();

    }

    public enum RefSFX
    {

        //Example
        //sfx_rune_drop,
        //sfx_transaction_complete,
        //sfx_coin_transfer,
        //wav_button_click01,
        //sfx_event_complete,
        //sfx_destruction_01,
        //sfx_magical_poof_01,
        //sfx_quest_complete,
        sfx_zhonya_in,
        //sfx_zhonya_out,
        //NULL

    }
    public enum RefVFX
    {
        //Example
        //vfx_interact_lifmunk,
        //vfx_interact_fireworks01,
        //vfx_strike_01,
        //vfx_zhonya,
        //vfx_piece_chest_reward01_destruction
    }
}



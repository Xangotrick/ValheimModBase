using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomStatusEffects
{
    public class U
    {
        public static void awake()
        {
            CustomStatusEffect IEffect = null;

            /*IEffect = Status.default_creator<Status_Equ_HelmetWizardLeather>("Equ_HelmetWizardLeather", -1);
            ItemManager.Instance.AddStatusEffect(IEffect);
            Status.Equ_HelmetWizardLeather = IEffect;*/

            /*IEffect = Status.default_creator<Status_Zhonya>("Zhonya", 1);
            ItemManager.Instance.AddStatusEffect(IEffect);
            Status.Zhonya = IEffect;*/
        }
        public static void update()
        {
        }
    }
    public class Status
    {
        //public static CustomStatusEffect Equ_HelmetWizardLeather;
        //public static CustomStatusEffect Zhonya;
        
        public static CustomStatusEffect default_creator<T>(string Name, float time) where T : StatusEffect
        {
            string dollarname = "$status_" + Name.ToLower();

            StatusEffect returner = ScriptableObject.CreateInstance<T>();
            returner.name = Name;
            returner.m_name = dollarname;

            string dir = "BepInEx/plugins/SifModAssets/images_status/";
            string filename = Name.ToLower() + ".png";
            FileInfo status_icon = new FileInfo(dir + filename);
            FileInfo status_icon_default = new FileInfo(dir + "placeholder.png");
            Sprite sprite;
            if (status_icon.Exists) { sprite = AssetUtils.LoadSprite(status_icon.FullName); }
            else { sprite = AssetUtils.LoadSprite(status_icon_default.FullName); }
            returner.m_icon = sprite;
            returner.m_ttl = time;
            returner.m_startMessageType = MessageHud.MessageType.Center;
            returner.m_startMessage = dollarname + "_start";
            returner.m_stopMessageType = MessageHud.MessageType.Center;
            returner.m_stopMessage = dollarname + "_stop";
            returner.m_tooltip = dollarname + "_tooltip";
            return new CustomStatusEffect(returner, fixReference: false);
        }
    }
    /*public class Status_Equ_HelmetWizardLeather : BetterStatus
    {
        public override void Setup(Character character)
        {
            base.Setup(character);

        }

        public override void OnStatManagerBuild()
        {
            base.OnStatManagerBuild();

            if (m_character.transform.root == Player.m_localPlayer.transform.root)
            {
                BonusMan.instance.Add(new StatMod("max_eitr", 20));
            }
        }

        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);
            /*Debug.Log("oupsw");
            if (!(m_character is Player)) { ; return; }

            float max_eitre = m_character.GetMaxEitr();
            if(max_eitre < 20)
            {
                typeof(Player).GetMethod("SetMaxEitr", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(m_character as Player, new object[] { 20, false });
            }*/
        }

        public override void Stop()
        {
            base.Stop();
        }
    }*/

    /*public class Status_Zhonya : BetterStatus
    {
        public Status_Zhonya()
        {
            invulnerable = true;
        }
        public override void Setup(Character character)
        {
            base.Setup(character);
            m_character.transform.GetComponentInChildren<Animator>().enabled = false;
            m_character.transform.GetComponent<Rigidbody>().isKinematic = true;
            FX.Play(FX.RefSFX.sfx_zhonya_in, m_character.transform.position);
            GameObject zhonya_part = FX.Play(FX.RefVFX.vfx_zhonya, m_character.transform.position);
            ParticleSystem ps = zhonya_part.GetComponent<ParticleSystem>();

            ps.Stop();
            var main = ps.main;
            var shape = ps.shape;
            shape.skinnedMeshRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>();
            main.duration = m_ttl + 0.5f;
            ps.Play();
        }

        public override void Stop()
        {
            m_character.transform.GetComponentInChildren<Animator>().enabled = true;
            m_character.transform.GetComponent<Rigidbody>().isKinematic = false;
            FX.Play(FX.RefSFX.sfx_zhonya_out, m_character.transform.position);
            base.Stop();
        }
    }*/


}

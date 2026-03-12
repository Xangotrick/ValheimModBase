using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class BetterStatus : StatusEffect
{

    public bool is_quitting = false;
    public override void Setup(Character character)
    {
        base.Setup(character);
        if (m_character.transform.root == Player.m_localPlayer.transform.root)
        {
        }
    }

    public override void Stop()
    {
        is_quitting = true;
        if (m_character.transform.root == Player.m_localPlayer.transform.root)
        {
        }
        base.Stop();
    }

}
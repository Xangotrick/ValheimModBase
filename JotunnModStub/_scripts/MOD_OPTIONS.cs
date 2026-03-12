using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class MOD_OPTIONS
{
    /// Est ce que le mod utilise les classes de synchronisation des données? 
    /// Si oui, implique que le mod ne fonctionne qu'avec un serveur dédié et un client
    public static bool IS_ONLINE_SYNC = true;

    public const string MODNAME = "SpellMod";

    /// Certaines fonctions ne doivent pas être exécutées plusieurs fois à travers les différents mods. Normalement il ne faut pas toucher à cette valeur
    public static bool MASTER_MOD = false;
}

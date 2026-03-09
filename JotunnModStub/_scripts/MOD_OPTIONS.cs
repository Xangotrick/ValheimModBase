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
    public static string MODNAME = "TemplateName";
}

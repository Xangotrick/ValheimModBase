using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public  class NetMethods
{
    #region Core Messages
    ///////////////////////////////////////
    ///Do not touch
    public static void CTS_RequestTuning()
    {
        ///CTS Core
        if (!ZNet.instance) { return; }
        if (!ZNet.instance.IsServer()) { NetClass.instance.ClientRequestHandler(MethodBase.GetCurrentMethod().Name); return; }
        ///

        foreach (NetObj OBJ in NetClass.instance.ListNetObj)
        {
            NetClass.instance.SendNetObjDataClient(OBJ);
        }
    }
    ///////////////////////////////////////
    #endregion


    /// <summary>
    /// Example de methode STC (Server To Client)
    /// </summary>
    public static void STC_DebugAll(string msg)
    {
        ///STC Core
        if (!ZNet.instance) { return; }
        if (ZNet.instance.IsServer()) { NetClass.instance.ClientSendHandler(MethodBase.GetCurrentMethod().Name, msg); return; }
        ///


        Debug.Log(msg);
    }

    /// <summary>
    /// Example de classe CTS (Client To Server)
    /// </summary>
    public static void CTS_MessageToDebugServer(string msg)
    {
        ///CTS Core
        if (!ZNet.instance) { return; }
        if (!ZNet.instance.IsServer()) { NetClass.instance.ClientRequestHandler(MethodBase.GetCurrentMethod().Name,msg); return; }
        ///

        Debug.Log(msg);

    }
}

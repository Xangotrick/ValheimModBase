using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using PlayFab.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Mono.Security.X509.X520;

public class NetClass
{
    public static string path_serverfolder = Application.dataPath + "/" + KERNEL.foldername + "_server_sync_files/";
    public static string path_serverfolder_fromroot = "server_sync_files/";  //MOD NAME ADDED ALREADY
    public static NetClass instance;


    public static CustomRPC RPC_ClientRequest;
    public static CustomRPC RPC_SendNetObjData;



    public NetClass()
    {
        instance = this;
        RPC_REGISTRY();
    }
    public void RPC_REGISTRY()
    {
        if(MOD_OPTIONS.MASTER_MOD)
        {
            ZRoutedRpc.instance.Register(MOD_OPTIONS.MODNAME + "_ClientRequest", new Action<long, ZPackage>(RPC_ClientRequestServer)); // Our Server RPC for CTS
            ZRoutedRpc.instance.Register(MOD_OPTIONS.MODNAME + "_ClientSendServer", new Action<long, ZPackage>(RPC_ClientSendServer)); // Our Server RPC for STC
            ZRoutedRpc.instance.Register(MOD_OPTIONS.MODNAME + "_SendNetObjData", new Action<long, ZPackage>(RPC_SendNetObjDataClient)); // Our Mock Client Function
        }

    }
    public static bool is_server
    {
        get
        {
            if (ZNet.instance)
            {
                return ZNet.instance.IsServer();
            }
            return false;
        }
    }


    public long PeerServerUID
    {
        get 
        {
            return ZNet.instance.GetPeers().FirstOrDefault(p => p.m_server)?.m_uid ?? 0;
        }
    }
    protected List<NetObj> _listNetObj;

    public List<NetObj> ListNetObj { get { return _listNetObj; } }

    public bool is_synced { get { return _listNetObj != null; } }

    //Runs on a Update 1000 only when game is active

    public virtual void RequestSync()
    {

    }
    public NetObj GetNetObjFromId(int id)
    {
        foreach (NetObj obj in NetClass.instance._listNetObj)
        {
            if (obj.ID == id) { return obj; }
        }
        return null;
    }


    
    ///Les methodes "Handler" sont internes à l'envoi de demandes de methodes CTS/STC (voir plus bas)
    ///CTS
    public void ClientRequestHandler(string methodname, params object[] args)
    {
        if (args.Length == 0)
        {
            ClientRequestServer(methodname);
        }
        else
        {
            ClientRequestServer(methodname, args);
        }
    }
    /// STC
    public void ClientSendHandler(string methodname, params object[] args)
    {
        if (args.Length == 0)
        {
            ClientSendServer(methodname);
        }
        else
        {
            ClientSendServer(methodname, args);
        }
    }

    /// RPC CALLS
    /// Par paires de fonctions, avec une nomenclature
    /// La fonction nommée sans "RPC" est la fonction lancée par le demandeur, et prépare le message à envoyer.
    /// La fonction nommée "RPC" est la fonction executée suite à l'appel préparé
    ///
    /// ClientRequestServer:
    /// Prépare une fonction nommée à être lancée par le serveur avec ses arguments
    /// 
    /// SendNetObjDataClient
    /// Permet la syncronisation depuis le serveur, d'une donnée NetObj
    /// 
    /// CTS
    private void ClientRequestServer(string methodname, params object[] args)
    {
        if (!ZNet.instance) { XLOG.error("No server to send a message to!", "SyncDataManager"); return; }
        if (ZNet.instance.IsServer()) { XLOG.error("ClientServerRequest should be called from a client!", "SyncDataManager"); return; }

        string data = "";
        string arg_encoding = "##$$##";
        if (args.Length > 0)
        {

            for (int i = 0; i < args.Length; i++)
            {
                object currentarg = args[i];
                if (currentarg is string) { arg_encoding += "s"; }
                if (currentarg is int) { arg_encoding += "i"; }
                if (currentarg is float) { arg_encoding += "f"; }
                if (currentarg is bool) { arg_encoding += "b"; }

                data += "##$$##";
                data += currentarg.ToString();
            }
        }
        else
        {
            arg_encoding += "null";
        }
        ZPackage package = new ZPackage();
        package.Write(methodname + arg_encoding + data);

        ZNetPeer peer = ZNet.instance.GetServerPeer();

        if (peer != null)
        {
            ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, MOD_OPTIONS.MODNAME + "_ClientRequest", package);
        }
    }
    private static void RPC_ClientRequestServer(long sender, ZPackage package)
    {
        string message = package.ReadString();

        string[] splitmessage = message.Split(new string[] { "##$$##" }, StringSplitOptions.None);

        string methodname = splitmessage[0];
        string arg_encoding = splitmessage[1];

        if (arg_encoding != "null")
        {
            List<string> data = new List<string>();
            for (int i = 1; i < splitmessage.Length; i++)
            {
                data.Add(splitmessage[i]);
            }

            object[] args = new object[arg_encoding.Length];

            for (int i = 0; i < arg_encoding.Length; i++)
            {
                switch (arg_encoding[i])
                {
                    case 's':
                        args[i] = splitmessage[i + 2];
                        break;
                    case 'i':
                        args[i] = int.Parse(splitmessage[i + 2]);
                        break;
                    case 'f':
                        args[i] = float.Parse(splitmessage[i + 2]);
                        break;
                    case 'b':
                        args[i] = bool.Parse(splitmessage[i + 2]);
                        break;
                }
            }
            typeof(NetMethods).GetMethod(methodname, BindingFlags.Public | BindingFlags.Static).Invoke(null, args);
        }
        else
        {
            typeof(NetMethods).GetMethod(methodname, BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { });
        }


    }
    /// STC
    public void ClientSendServer(string methodname, params object[] args)
    {
        if (!ZNet.instance) { XLOG.error("No server to send a message to!", "SyncDataManager"); return; }
        if (!ZNet.instance.IsServer()) { XLOG.error("ClientSendServer should be called from a server!", "SyncDataManager"); return; }

        string data = "";
        string arg_encoding = "##$$##";
        if (args.Length > 0)
        {

            for (int i = 0; i < args.Length; i++)
            {
                object currentarg = args[i];
                if (currentarg is string) { arg_encoding += "s"; }
                if (currentarg is int) { arg_encoding += "i"; }
                if (currentarg is float) { arg_encoding += "f"; }
                if (currentarg is bool) { arg_encoding += "b"; }

                data += "##$$##";
                data += currentarg.ToString();
            }
        }
        else
        {
            arg_encoding += "null";
        }


        ZPackage package = new ZPackage();
        package.Write(methodname + arg_encoding + data);
        if (ZNet.instance.GetPeers() != null)
        {
            foreach (ZNetPeer peer in ZNet.instance.GetPeers())
            {
                if (peer == null) { continue; }
                Debug.Log(peer.m_uid);
                if (peer.m_uid == PeerServerUID) { continue; }
                ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, MOD_OPTIONS.MODNAME + "_ClientSendServer", package);
            }
        }
    }
    private static void RPC_ClientSendServer(long sender, ZPackage package)
    {
        string message = package.ReadString();

        string[] splitmessage = message.Split(new string[] { "##$$##" }, StringSplitOptions.None);

        string methodname = splitmessage[0];
        string arg_encoding = splitmessage[1];


        if (arg_encoding != "null")
        {
            List<string> data = new List<string>();
            for (int i = 1; i < splitmessage.Length; i++)
            {
                data.Add(splitmessage[i]);
            }

            object[] args = new object[arg_encoding.Length];

            for (int i = 0; i < arg_encoding.Length; i++)
            {
                switch (arg_encoding[i])
                {
                    case 's':
                        args[i] = splitmessage[i + 2];
                        break;
                    case 'i':
                        args[i] = int.Parse(splitmessage[i + 2]);
                        break;
                    case 'f':
                        args[i] = float.Parse(splitmessage[i + 2]);
                        break;
                    case 'b':
                        args[i] = bool.Parse(splitmessage[i + 2]);
                        break;
                }
            }
            typeof(NetMethods).GetMethod(methodname, BindingFlags.Public | BindingFlags.Static).Invoke(null, args);
        }
        else
        {
            typeof(NetMethods).GetMethod(methodname, BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { });
        }


    }





    /// OBJ SYNC
    public void SendNetObjDataClient(NetObj obj)
    {
        ZPackage package = new ZPackage();
        package.Write(obj.TYPE.ToString() + obj.serialize());
        //RPC_SendNetObjData.SendPackage(ZNet.instance.GetPeers(), package);
        ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, MOD_OPTIONS.MODNAME + "_SendNetObjData", package);
    }
    private static void RPC_SendNetObjDataClient(long sender, ZPackage package)
    {
        string data = package.ReadString();

        string typestring = "";
        string truedata = "";

        foreach (NetObj.Type type in Enum.GetValues(typeof(NetObj.Type)))
        {
            string typebeingchecked = type.ToString();
            if (data.Substring(0, typebeingchecked.Length) == typebeingchecked)
            {
                typestring = typebeingchecked;
                truedata = data.Replace(typestring, "");
            }
        }


        if (!NetClass.instance.is_synced)
        {
            NetClass.instance._listNetObj = new List<NetObj>();
        }

        NetObj sync_obj = NetObj.SyncObject_deserialize(truedata, typestring);
        NetObj current_obj = NetClass.instance.GetNetObjFromId(sync_obj.ID);
        if (current_obj != null)
        {
            for (int k = 0; k < NetClass.instance._listNetObj.Count; k++)
            {
                if (NetClass.instance._listNetObj[k].ID == current_obj.ID)
                {
                    NetClass.instance._listNetObj[k] = sync_obj;
                }
            }
        }
        else
        {
            NetClass.instance._listNetObj.Add(sync_obj);
        }

    }














}

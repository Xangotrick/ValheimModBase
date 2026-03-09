using PlayFab.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Random = UnityEngine.Random;


public class NetServer : NetClass
{
    public NetServer() : base()
    {
    }

    public override void RequestSync()
    {
        _listNetObj = new List<NetObj>();
        if (!Directory.Exists(path_serverfolder))
        {
            Directory.CreateDirectory(path_serverfolder);
        }

        DirectoryInfo ServerFolderFiles = new DirectoryInfo(path_serverfolder);
        FileInfo[] info = ServerFolderFiles.GetFiles("*.txt");

        foreach (FileInfo infofile in info)
        {
            string filename = infofile.Name.Replace(".txt", "");
            string typestring = filename.Split('_')[0];
            string data = rw.load_save_textfile("r", path_serverfolder_fromroot + infofile.Name);
            _listNetObj.Add(NetObj.SyncObject_deserialize(data, typestring));
        }
        foreach (NetObj obj in _listNetObj)
        {
            XLOG.message(obj.ToString());
        }


        XLOG.message("Server has been tuned!", "SyncDataManager");


        if (_listNetObj.Count == 0)
        {
            
            NetObj obj = new NetObj();
            obj._string_list.Add("hello");
            XLOG.message("created : " + obj.serialize());
            OnCreateNetObj(obj);
        }
    }


    public void OnCreateNetObj(NetObj obj)
    {
        while (obj.ID == -1)
        {
            int rand_id = Random.Range(1, 999999999);
            if (GetNetObjFromId(rand_id) != null) { continue; }
            obj.ID = rand_id;
        }
        string stringvalue = obj.serialize();
        rw.load_save_textfile("w", path_serverfolder_fromroot + obj.TYPE + "_" + obj.ID.ToString() + ".txt", stringvalue);

        _listNetObj.Add(obj);

        ///SendSyncObjDataClient(obj);
    }
}
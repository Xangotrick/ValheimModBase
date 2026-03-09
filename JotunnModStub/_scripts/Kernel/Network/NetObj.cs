using PlayFab.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class NetObj :JsonObject
{
    public enum Type
    {
        None,
        NetObj,
    }

    public int ID = -1;
    public Type TYPE = Type.None;


    [SerializeField]
    public List<string> _string_list = new List<string>();
    [SerializeField]
    private List<int> _int_list = new List<int>();
    [SerializeField]
    private List<float> _float_list = new List<float>();
    [SerializeField]
    private List<bool> _bool_list = new List<bool>();




    public NetObj()
    {

        TYPE = (NetObj.Type)Enum.Parse(typeof(NetObj.Type), this.GetType().ToString());
    }


    public override string ToString()
    {
        return "NetObj: ID:" + ID.ToString() + " Type:" + TYPE.ToString();
    }

    public static NetObj SyncObject_deserialize(string json, string atypestring)
    {
        NetObj.Type atype = (NetObj.Type)Enum.Parse(typeof(NetObj.Type), atypestring);

        switch (atype)
        {
            case Type.NetObj:
                return JsonObject.deserialize<NetObj>(json);
        }
        return null;
    }
}

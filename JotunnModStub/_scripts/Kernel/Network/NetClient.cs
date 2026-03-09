using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class NetClient : NetClass
{
    public NetClient():base()
    {
    }

    public override void RequestSync()
    {
        NetMethods.CTS_RequestTuning();
    }
}

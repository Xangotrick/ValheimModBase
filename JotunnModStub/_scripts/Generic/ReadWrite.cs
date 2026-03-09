using BepInEx;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;



public class CYPHER
{
    private static int n = 5;
    public static string rail_cypher(string input)
    {
        if (input == "") { return ""; }
        string returner = "";


        string[] rails = new string[n];
        for (int k = 0; k < input.Length; k++)
        {
            char c = input[k];

            rails[rail_index(k)] += c;
        }

        foreach (string astring in rails.ToList())
        {
            returner += astring;
        }


        return returner;
    }
    public static string rail_read(string input)
    {
        if (input == "") { return ""; }

        string[] returnarray = new string[input.Length];
        string wierdstring = "";

        for (int k = 0; k < input.Length; k++)
        {
            wierdstring += Convert.ToChar(k);
        }

        string mutatedstring = rail_cypher(wierdstring);


        for (int k = 0; k < input.Length; k++)
        {
            char c = input[k];
            int pos = Convert.ToInt16(mutatedstring[k]);
            returnarray[pos] += c;
        }

        string returner = "";

        for (int k = 0; k < returnarray.Length; k++)
        {
            returner += returnarray[k];
        }

        return returner;
    }
    private static int rail_index(int index)
    {
        int subrank = 2 * (n - 1);
        int subindex = index % subrank;
        if (subindex < n)
        {
            return subindex;
        }
        else
        {
            return (n - 1) - (subindex % (n - 1));
        }
    }
}
public class rw
{

    public static string load_save_textfile(string rw, string filename, string data = "", string def = "")
    {
        string filepath = Application.dataPath + "/" + KERNEL.filename +filename;
        if (rw == "w")
        {
            if (data == "") { return ""; }
            File.WriteAllText(filepath, CYPHER.rail_cypher(data));
        }
        if (rw == "r")
        {
            if (!System.IO.File.Exists(filepath))
            {
                string defaultdata = def;
                string cdefaultdata = CYPHER.rail_cypher(defaultdata);
                File.WriteAllText(filepath, cdefaultdata);

            }

            string readdata = File.ReadAllText(filepath);
            return CYPHER.rail_read(readdata);
        }
        return "";
    }
    public static void delete_file(string filename)
    {
        string filepath = Application.dataPath + "/" + KERNEL.filename + filename;
        if(System.IO.File.Exists(filepath))
        {
            File.Delete(filepath);
        }
    }

    public static string dir_data = Application.dataPath + "/";

    public static void writefile(string data, string filename, string dir = "")
    {
        if (dir == "") { dir = dir_data; }

        File.WriteAllText(dir + KERNEL.filename + filename, data);
    }

    public static string readfile(string filename, bool isroot = true)
    {
        string returner = "";

        string path = Application.dataPath + "/" + KERNEL.filename + filename;
        if (!isroot) { path = KERNEL.filename+filename; }

        try
        {
            returner = File.ReadAllText(path);
        }
        catch (Exception e)
        {
            XLOG.error("Could not read file. does it exist ?", "rw");

        }

        return returner;
    }

    public static List<XMLDATA> ReadXML(string path)
    {

        List<XMLDATA> r = new List<XMLDATA> ();

        string filedata = File.ReadAllText(path);
        filedata = filedata.Replace(" ", "").Replace("\t", "").Replace("\r", "");
        string[] lines_text = filedata.Split('\n');
        //List<string> list_lines = lines_text.Select(x => Regex.Replace(x, @"\s+", "")).ToList();
        List<string> list_lines = lines_text.Where(x => x != "" && x != null).ToList();

        list_lines.Remove("<root>");
        list_lines.Remove("</root>");

        List<(int, int, string)> start_end_index = new List<(int, int, string)>();
        string KEY = "";
        (int, int, string) trouple = (0, 0, "");
        for (int i = 0; i < list_lines.Count; i++)
        {
            string line = list_lines[i];
            //Opening
            if (line.Contains("<") && !line.Contains("/"))
            {
                if (KEY != "") { XLOG.error("Wrong formatting on config file", path); }
                KEY = line.Substring(1, line.Length - 2);
                trouple.Item1 = i+1;
                trouple.Item3 = KEY;
            }
            //Closing
            if (line.Contains("<") && line.Contains("/"))
            {
                if (KEY == "") { XLOG.error("Wrong formatting on config file", path); }
                KEY = "";
                trouple.Item2 = i-1;
                start_end_index.Add(trouple);
                trouple = (0, 0, "");
            }
        }
        foreach ((int, int, string) troup in start_end_index)
        {
            XMLDATA data = new XMLDATA(troup.Item3);
            for(int i = troup.Item1; i <= troup.Item2; i++)
            {
                string astring = list_lines[i];
                if(astring.Contains("="))
                {
                    string[] split = astring.Split('=');
                    data.data.Add((split[0], split[1]));
                }
            }
            r.Add(data);
        }


        return r;
    }


    public class XMLDATA
    {
        public string key;
        public List<(string,  string)> data = new List<(string, string)>();

        public XMLDATA(string key)
        {
            this.key = key;
        }

        public override string ToString()
        {
            string r = "Key "+ key;

            foreach((string, string) data in  data) { r += $"\n {data.Item1} : {data.Item2}"; }

            return r;

        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSONManager
{
    public static JSONModel ReadURL(string path)
    {
        if (File.Exists(path) == false) return null;
        string data = File.ReadAllText(path);
        return JsonUtility.FromJson<JSONModel>(data);
    }

    public static void SaveURL(string _url)
    {
        JSONModel model = new JSONModel();
        model.url = _url;
        string data = JsonUtility.ToJson(model);
        File.WriteAllText(Application.persistentDataPath + "/URL.json", data);
    }
}

[Serializable]
public class JSONModel 
{
    public string url;
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataSerializationUtility
{
    public static T Load<T>(string pathEnd)
    {
        string path = Application.persistentDataPath + pathEnd;

        if (File.Exists(path))
        {
            try
            {
                string data = File.ReadAllText(path);
                //      Debug.Log("LOADED "+data);
                return JsonUtility.FromJson<T>(data);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
         
        }
        else
        {

            CreateNewEmptyFile(path);

            string data = File.ReadAllText(path);
            //      Debug.Log("LOADED "+data);
            return JsonUtility.FromJson<T>(data);
        }
    }

    public static void Save<T>(T toSave,string pathEnd)
    {
        string path = Application.persistentDataPath + pathEnd;
        string json = JsonUtility.ToJson(toSave, true);
      //  Debug.Log("SAVED "+json);
        try
        {
          File.WriteAllText(path, json);

        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }

    private static void CreateNewEmptyFile(string path)
    {
        File.WriteAllText(path,"");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;   

public static class SaveSystem
{
    public static void SaveData(FileData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + data.GetPath();
        
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static FileData LoadData(string pathEnd, FileData defaultData)
    {
        string path = Application.persistentDataPath + pathEnd;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            FileData data = formatter.Deserialize(stream) as FileData;
            stream.Close();
            return data;
        }
        else
        {
            SaveData(defaultData);
            return defaultData;
        }
    }
}

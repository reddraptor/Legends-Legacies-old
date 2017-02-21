using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Assets.Scripts;

public static class WorldSave
{
    public static string saveFilePath = Application.persistentDataPath + "/WorldSaves/";
    public static string saveFileExtension = ".sav";

    public static void Save(string name, GameWorld gameWorld)
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (!Directory.Exists(saveFilePath))
        {
            Directory.CreateDirectory(saveFilePath);
        }
        FileStream file = File.Create(saveFilePath + name + saveFileExtension);
        //bf.Serialize(file, gameWorld.Serializable);
        file.Close();
        Debug.Log("Saved at " + saveFilePath + name + saveFileExtension);
    }

    public static string[] GetSaveFileNames()
    {
        string[] saveFileNames = Directory.GetFiles(saveFilePath);
        for (int i = 0; i < saveFileNames.Length; i++)
        {
            saveFileNames[i] = saveFileNames[i].Remove(0, saveFilePath.Length);
            saveFileNames[i] = saveFileNames[i].Remove(saveFileNames[i].Length - saveFileExtension.Length);
        }

        return saveFileNames;
    }

    public static void Load(string name, GameWorld gameWorld)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(saveFilePath + name + saveFileExtension, FileMode.Open);
        //gameWorld.Serializable = (GameWorld.WorldSerializable)bf.Deserialize(file);
        file.Close();
    }

    public static void Delete(string name)
    {
        File.Delete(saveFilePath + name + saveFileExtension);
    }
}

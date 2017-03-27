using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.Data_Types.Serialization;

namespace Assets.Scripts.Managers
{
    class PersistanceManager : MonoBehaviour
    {
        public string saveFileFolder = "WorldSaves";
        public string saveFileExtension = ".sav";


        internal WorldManager worldManager;
        internal string saveFilePath;

        private void Start()
        {
            worldManager = GetComponent<WorldManager>();
            saveFilePath = Application.persistentDataPath + "/" + saveFileFolder + "/";
        }


        public void Save(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            if (!Directory.Exists(saveFilePath))
            {
                Directory.CreateDirectory(saveFilePath);
            }
            FileStream file = File.Create(saveFilePath + fileName + saveFileExtension);

            WorldData worldData = new WorldData(worldManager);
            bf.Serialize(file, worldData);

            file.Close();
            Debug.Log("Saved at " + saveFilePath + fileName + saveFileExtension);
        }

        public string[] GetSaveFileNames()
        {
            string[] saveFileNames = Directory.GetFiles(saveFilePath);
            for (int i = 0; i < saveFileNames.Length; i++)
            {
                saveFileNames[i] = saveFileNames[i].Remove(0, saveFilePath.Length);
                saveFileNames[i] = saveFileNames[i].Remove(saveFileNames[i].Length - saveFileExtension.Length);
            }

            return saveFileNames;
        }

        public void Load(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveFilePath + fileName + saveFileExtension, FileMode.Open);
            WorldData worldData = (WorldData)bf.Deserialize(file);
            worldData.SetData(worldManager);
            file.Close();
        }

        public void Delete(string name)
        {
            File.Delete(saveFilePath + name + saveFileExtension);
        }


    }
}

using UnityEngine;
using UnityEditor;
using NUnit.Framework;
//using Assets.Scripts.Behaviors;
//using Assets.Scripts.Serialization;
//using Assets.Scripts;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class Serialization_Test {

	[Test]
	public void SaveLoadEntity() {
        //Arrange

        //Entity entity1 = ScriptableObject.CreateInstance<Entity>();
        //PlayerScript player1 = ScriptableObject.CreateInstance<PlayerScript>();
        //player1.playerId = "Bob";
        //entity1.behaviorScript = player1;
        //entity1.name = "Bob";

        //Entity entity2 = ScriptableObject.CreateInstance<Entity>();
        //PlayerScript player2 = ScriptableObject.CreateInstance<PlayerScript>();
        //entity2.behaviorScript = player2;

        //EntitySerializer entitySerializer = new EntitySerializer();

        string saveFilePath = Application.persistentDataPath + "/TestSaves/";
        string saveFileName = "Save_Entity_Test.save";
        BinaryFormatter bf = new BinaryFormatter();

        if (!Directory.Exists(saveFilePath))
        {
            Directory.CreateDirectory(saveFilePath);
        }
        FileStream file = File.Create(saveFilePath + saveFileName);

        //Act
        //Save entity data to file
       // bf.Serialize(file, entitySerializer.GetSerializableData(entity1));
        file.Close();
        Debug.Log("Saved at " + saveFilePath + saveFileName);

        //Load entity data from file
        file = File.Open(saveFilePath + saveFileName, FileMode.Open);
        //entitySerializer.SetDeserializedData(entity2, (EntitySerializableData)bf.Deserialize(file));
        file.Close();
 
        //Assert
        //The object has a new name
        //Assert.AreEqual(entity1.name, entity2.name);
	}
}

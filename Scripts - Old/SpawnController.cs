using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Behaviors;


// Handles the spawning and despawning of mobile objects from a list of prefabs
public class SpawnController : MonoBehaviour
{
    public List<Entity> entityList;
    public List<GameObject> prefabList;

    public int playerPrefabIndex;
    public int playerEntityIndex;
    public PlayerScript playerScript;
    
    TileMap tileMap;
    GameController gameController;
    LocationList masterLocationList;

    // Use this for initialization
    void Start()
    {
        gameController = gameObject.GetComponent<GameController>();
        //tileMap = gameController.tileMap;
        masterLocationList = gameController.gameWorld.MasterLocationList;    
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public PlayerScript SpawnPlayer(Coordinates coords)
    {
        PlayerScript player = Instantiate(playerScript);
        player.Entity = Spawn(playerEntityIndex, coords);
        return player;
    }
    
    public void Spawn(Entity entity, Coordinates coords)
    {
        GameObject mobileInstance = Instantiate(prefabList[entity.prefabIndex], GetPositionFromFocus(coords), Quaternion.identity, tileMap.transform);
        //entity.Mobile = mobileInstance.GetComponent<Mobile>();
        if (!entity.Mobile)
        {
            throw new System.ArgumentException("GameObject prefab has no mobile component.", "index");
        }
        else
        {
            masterLocationList.ChangeAsset(entity, coords);
        }
    }

    public Entity Spawn(int entityIndex, Coordinates coords)
    {
        Entity entity = Instantiate(entityList[entityIndex]);
        Spawn(entity, coords);
        return entity;
    }

    public void Despawn(Entity entity)
    {
        masterLocationList.RemoveAsset<Entity>(entity.Coordinates);
        Destroy(entity.Mobile);
    }

    Vector3 GetPositionFromFocus(Coordinates coords)
    {
        Vector3 position = Vector3.zero;
        //position.x = position.x + (coords.World.X - tileMap.focusLocation.World.X);
        //position.y = position.y + (coords.World.Y - tileMap.focusLocation.World.Y);
        return position;
    }
}

using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;
using System;
using System.Collections;

public class EntityManager : MonoBehaviour
{
    /* EDITOR FIELDS */
    public GameObject[] prefabs;

    /* PUBLIC ENUMERATORS */
    public enum EntityType : byte { Undefined, Player, Mob, Npc, Terrain, Prop }

    /* PUBLIC FIELDS */
    public Dictionary<Coordinates, Entity> players;
    public Dictionary<Coordinates, Entity> mobs;
    //static public Dictionary<Coordinates, Location> npcs;
    //static public Dictionary<Coordinates, Location> newTerrain;
    //static public Dictionary<Coordinates, Location> props;
    //...


    /* PRIVATE FIELDS */
    Dictionary<string, GameObject> prefabDictionary;
    TileMapManager tileMapManager;
    MovementManager movementManager;

    /*UNITY MESSAGES */
    // Use this for initialization
    void Start()
    {
        tileMapManager = GetComponent<TileMapManager>();
        movementManager = GetComponent<MovementManager>();

        //Load prefabs into dictionary for easy retrieval by name
        prefabDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in prefabs)
        {
            prefabDictionary.Add(prefab.name, prefab);
        }

        players = new Dictionary<Coordinates, Entity>();
        mobs = new Dictionary<Coordinates, Entity>();
        //npcs = new Dictionary<Coordinates, Entity>();
        //newTerrain = new Dictionary<Coordinates, Entity>();
        //props = new Dictionary<Coordinates, Entity>();
        //...
    }

    // Update is called once per frame
    void Update()
    {

    }

    /* METHODS */
    /// <summary>
    /// Creates instance of a prefab with a given name, at the given coordinates on the map and adds it to world data
    /// </summary>
    /// <param name="name">Prefab name</param>
    /// <param name="coordinates">Map coordinates</param>
    /// <returns>A game object with the Entity component that is an instance of a prefab.</returns>
    public Entity Spawn(string name, Coordinates coordinates)
    {
        GameObject prefab = prefabDictionary[name];
        GameObject gOEntity;

        gOEntity = Instantiate(prefab, tileMapManager.GetScreenPositionAt(coordinates), Quaternion.identity);
        Entity entity = gOEntity.GetComponent<Entity>();
        if (entity)
        {
            entity.entityManager = this;
            if (!Place(entity, coordinates))
            {
                Destroy(entity.gameObject);
                return null;
            }
            return entity;
        }
        else return null;
    }

    public void Despawn(Entity entity)
    {
        if (entity.type == EntityType.Player)
        {
            players.Remove(entity.coordinates);
        }
        else if (entity.type == EntityType.Mob)
        {
            mobs.Remove(entity.coordinates);
        }
        // ... else if other entity types

        Destroy(entity.gameObject);
    }

    public void DespawnAll()
    {
        foreach (Entity entity in players.Values)
        {
            Destroy(entity.gameObject);
        }
        players.Clear();

        foreach (Entity entity in mobs.Values)
        {
            Destroy(entity.gameObject);
        }
        mobs.Clear();

        // ... other entity types
    }

    public bool IsOccupied(Coordinates coordinates, EntityType entityType)
    {
        if (entityType == EntityType.Player || entityType == EntityType.Mob) //Mobs and Players can't share the same space
        {
            if (players.ContainsKey(coordinates)) return true;
            else if (mobs.ContainsKey(coordinates)) return true;
            else return false;
        }
        else throw new System.ArgumentException("IsFree(Coordinates, Entity.Entity_Type): Given entity type undefined.");
    }

    public bool Place(Entity entity, Coordinates coordinates)
    {
        if (!entity) return false;

        if (!IsOccupied(coordinates, entity.type))
        {
            if (entity.type == EntityType.Player)
            {
                if (entity.placed) players.Remove(entity.coordinates);
                players.Add(coordinates, entity);
            }
            else if (entity.type == EntityType.Mob)
            {
                if (entity.placed) mobs.Remove(entity.coordinates);
                mobs.Add(coordinates, entity);
            }
            entity.coordinates = coordinates;
            entity.placed = true;
            return true;
        }
        else return false;
    }

    public void Center(Entity entity)
    {
        entity.transform.position = Vector3.zero;

        tileMapManager.ChangeFocus(entity.coordinates);
        tileMapManager.Refresh();
    }

    public Player GetPlayer(string name)
    {
        Player player;

        foreach (Entity playerEntity in players.Values)
        {
            player = playerEntity.GetComponent<Player>();
            if (player.name == name) return player;
        }
        return null;
    }

    internal void SetMoveAll(int horizontal, int vertical, float speed)
    {
        foreach (Entity entity in mobs.Values)
        {
            movementManager.Add(entity.GetComponent<Movement>(), horizontal, vertical, speed);
        }
    }
}

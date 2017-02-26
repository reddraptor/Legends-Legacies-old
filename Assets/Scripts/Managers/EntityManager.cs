using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;
using System;
using System.Collections;

public class EntityManager : MonoBehaviour
{
    /* INSPECTOR FIELDS */
    public GameObject[] prefabs;

    /* PUBLIC ENUMERATORS */
    public enum EntityType : byte { Undefined, Map, Player, Mob, Npc, Terrain, Prop }

    /* PUBLIC PROPERTIES */
    public Dictionary<Coordinates, Entity>.ValueCollection playerCollection
    {
        get { return players.Values; }
    }

    public Dictionary<Coordinates, Entity>.ValueCollection mobCollection
    {
        get { return mobs.Values; }
    }

    //...

    /* PRIVATE FIELDS */
    Dictionary<string, GameObject> prefabDictionary;
    TileMapManager tileMapManager;
    MovementManager movementManager;

    Dictionary<Coordinates, Entity> players;
    Dictionary<Coordinates, Entity> mobs;
    //static public Dictionary<Coordinates, Location> npcs;
    //static public Dictionary<Coordinates, Location> newTerrain;
    //static public Dictionary<Coordinates, Location> props;
    //...


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
    /// Creates instance of a prefab with a given name, at the given coordinates on the map and adds it to
    /// the approbriate entity dictionary
    /// </summary>
    /// <param name="name">Prefab name</param>
    /// <param name="coordinates">Map coordinates</param>
    /// <returns>The entity component of new gameobject instance. If the prefab has no entity component or
    /// entity can not be placed at those coordinates, null is returned.</returns>
    public Entity Spawn(string name, Coordinates coordinates)
    {
        GameObject prefab = prefabDictionary[name];
        GameObject gOEntity;

        gOEntity = Instantiate(prefab, tileMapManager.GetScreenPositionAt(coordinates), Quaternion.identity);
        Entity entity = gOEntity.GetComponent<Entity>();
        if (entity)
        {
            if (!Place(entity, coordinates))
            {
                Destroy(entity.gameObject);
                return null;
            }
            return entity;
        }
        else return null;
    }

    /// <summary>
    /// Removes entity from data. Destroys gameobject entity is attached to.
    /// </summary>
    /// <param name="entity">An entity component</param>
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

    /// <summary>
    /// All entity data is cleared and attached gameobjects destroyed. Typically called when one game session ends before another begins.
    /// </summary>
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

    /// <summary>
    /// Checks if coordinates is occupied by an entity type, or another type that can not share the same tile as the entity type.
    /// </summary>
    /// <param name="coordinates">Map coordinates</param>
    /// <param name="entityType">An entity type</param>
    /// <returns>True if occupied, else false.</returns>
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

    /// <summary>
    /// Attempts to place an entity at given coordinates. 
    /// </summary>
    /// <param name="entity">An entity component</param>
    /// <param name="coordinates">Map Coordinates</param>
    /// <returns>True if successful, false if entity is null, or coordinates are occupied.</returns>
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
            Debug.Log("Placed: " + entity.name + ", " + entity.GetInstanceID() + ", " + entity.coordinates.World.X + " " + entity.coordinates.World.Y);
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Moves camera focus with entity in the center.
    /// </summary>
    /// <param name="focusEntity">An entity component</param>
    public void Center(Entity focusEntity)
    {
        tileMapManager.focus = focusEntity.coordinates;

        foreach (Entity entity in players.Values)
        {
            entity.transform.position = tileMapManager.GetScreenPositionAt(entity.coordinates);
        }
        foreach (Entity entity in mobs.Values)
        {
            entity.transform.position = tileMapManager.GetScreenPositionAt(entity.coordinates);
        }

        // ...

        tileMapManager.Refresh();
    }

    /// <summary>
    /// Returns the client's player component
    /// </summary>
    /// <param name="name">Name of the entity</param>
    /// <returns>Player component with given name</returns>
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

    /// <summary>
    /// Moves all entities in given horizontal and vertical tile spaces at given speed. Typically called by tile map scroller.
    /// </summary>
    /// <param name="horizontal">horizontal vectorin tile spaces</param>
    /// <param name="vertical">vertical vector in tile spaces</param>
    /// <param name="speed">Speed to move entities</param>
    internal void SetMoveAll(int horizontal, int vertical, float speed)
    {
        foreach (Entity entity in mobs.Values)
        {
            movementManager.Add(entity.GetComponent<Movement>(), horizontal, vertical, speed);
        }
    }


}

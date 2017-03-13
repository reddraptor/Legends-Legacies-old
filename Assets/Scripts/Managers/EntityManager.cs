using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;
using System;

public class EntityManager : MonoBehaviour
{
    /* INSPECTOR FIELDS */
    public GameObject[] prefabs;

    /* PUBLIC ENUMERATORS */
    public enum EntityType : byte { Undefined, Map, Player, Mob, Npc, Terrain, Prop }

    /* PUBLIC PROPERTIES */
    internal Dictionary<Coordinates, Entity>.ValueCollection playerCollection
    {
        get { return players.Values; }
    }

    internal Dictionary<Coordinates, Entity>.ValueCollection mobCollection
    {
        get { return mobs.Values; }
    }

    //...

    internal WorldManager worldManager
    {
        get { return GetComponent<WorldManager>(); }
    }
    
    internal GameManager gameManager
    {
        get { return GetComponent<GameManager>(); }
    }

    /* PRIVATE FIELDS */
    System.Random random = new System.Random();
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

    private void Awake()
    {
        //Load prefabs into dictionary for easy retrieval by name
        prefabDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in prefabs)
        {
            prefabDictionary.Add(prefab.name, prefab);
        }
        tileMapManager = GetComponent<TileMapManager>();
        movementManager = GetComponent<MovementManager>();

        players = new Dictionary<Coordinates, Entity>();
        mobs = new Dictionary<Coordinates, Entity>();
        //npcs = new Dictionary<Coordinates, Entity>();
        //newTerrain = new Dictionary<Coordinates, Entity>();
        //props = new Dictionary<Coordinates, Entity>();
        //...

    }

    // Use this for initialization
    void Start()
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
    internal Entity Spawn(string name, Coordinates coordinates)
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
    internal void Despawn(Entity entity)
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

        entity.chunk.entitySet.Remove(entity);
        Destroy(entity.gameObject);
    }

    /// <summary>
    /// All entity data is cleared and attached gameobjects destroyed. Typically called when one game session ends before another begins.
    /// </summary>
    internal void CleanUpSpawns()
    {
        foreach (Entity entity in players.Values)
        {
            Despawn(entity);
        }
        players.Clear();

        foreach (Entity entity in mobs.Values)
        {
            Despawn(entity);
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
    internal bool IsOccupied(Coordinates coordinates, EntityType entityType)
    {
        if (entityType == EntityType.Player || entityType == EntityType.Mob) //Mobs and Players can't share the same space
        {
            if (players.ContainsKey(coordinates)) return true;
            else if (mobs.ContainsKey(coordinates)) return true;
            else return false;
        }
        else throw new ArgumentException("IsFree(Coordinates, Entity.Entity_Type): Given entity type undefined.");
    }

    
    internal void Populate(Chunk chunk)
    {
        if (chunk == null) return;

        int population = random.Next(10);

        for (int i = 0; i < population; i++)
        {
            int randomX = random.Next(Chunk.chunkTileWidth);
            int randomY = random.Next(Chunk.chunkTileWidth);

            Entity entity = Spawn("Cow", new Coordinates(chunk.lowerLeft.inWorld.x + randomX, chunk.lowerLeft.inWorld.y + randomY));
        }

        // START DEBUG CODE
        //if (mobCollection.Count == 0)
        //{
        //    Spawn("Cow", new Coordinates(chunk.lowerLeft.inWorld.x + randomX, chunk.lowerLeft.inWorld.y + randomY));
        //}
        // END DEBUG CODE
    }

    internal void Depopulate(Chunk chunk)
    {
        if (chunk == null) return;

        Entity[] entityArray = new Entity[chunk.entitySet.Count];

        chunk.entitySet.CopyTo(entityArray); // Since Despawn(Entity) modifies chunk.entitySet, have to iterate over a copy of the set
        foreach (Entity entity in entityArray)
        {
            Despawn(entity);
        }
    }

    /// <summary>
    /// Attempts to set movement of an entity. If it's the client player, sets the map to scroll.
    /// </summary>
    /// <param name="entity">Entity to set movement on.</param>
    /// <param name="horizontal">Number of tiles to move along the horizontal.</param>
    /// <param name="vertical">Number of tiles to move along the vertical.</param>
    /// <param name="locomotion">Method of movement by terrain type</param>
    /// <returns>Returns true if movement set successfully.</returns>
    internal bool Move(Entity entity, Vector2 vector, TerrainTile.TerrainType locomotion = TerrainTile.TerrainType.Land)
    {
        float distance = vector.magnitude;// Vector2.Distance(entity.transform.position, (Vector2)entity.transform.position + direction);

        // If obstacles in our way, ignore move
        if (IsObstacle(entity, vector, distance))
        {
            return false;
        }
        
        // If we can place the entity at new coordinates set movements
        if (Place(entity, entity.coordinates.AtVector(vector)))
        {
            Attributes attributes = entity.GetComponent<Attributes>();

            if (attributes & tileMapManager & worldManager)
            {
                float speed = worldManager.GetSpeed(entity.coordinates, attributes, locomotion);

                // If entity is the player, scroll the map,
                if (gameManager.player.entity == entity)
                {
                    tileMapManager.Scroll(vector, speed);
                    return true;
                }
                // else set movement of the entity
                else
                {
                    Movement movement = entity.GetComponent<Movement>();
                    if (movement)
                    {
                        movementManager.Add(movement, vector, speed);
                        return true;
                    }
                }
            }
        }
        return false;

    }

    internal bool IsObstacle(Entity entity, Vector2 direction, float distance)
    {
        Collider2D collider = entity.GetComponent<Collider2D>();
        RaycastHit2D[] hits = new RaycastHit2D[1];
        if (collider.Raycast(direction, hits, distance) > 0)
            return true;
        else return false;
    }
    

    /// <summary>
    /// Attempts to place an entity at given coordinates. 
    /// </summary>
    /// <param name="entity">An entity component</param>
    /// <param name="coordinates">Map Coordinates</param> 
    /// <returns>True if successful, false if entity is null, or coordinates are occupied.</returns>
    internal bool Place(Entity entity, Coordinates coordinates)
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
            // else if other entity types

            entity.coordinates = coordinates;

            Chunk chunk = worldManager.GetChunk(coordinates);

            if (entity.chunk != chunk && chunk != null)
            {
                if (entity.chunk != null) entity.chunk.entitySet.Remove(entity);
                entity.chunk = chunk;
                chunk.entitySet.Add(entity);
            }

            entity.placed = true;
 
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Moves camera focus with entity in the center.
    /// </summary>
    /// <param name="focusEntity">An entity component</param>
    internal void Center(Entity focusEntity)
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
    internal Player GetPlayer(string name)
    {
        Player player;

        foreach (Entity playerEntity in players.Values)
        {
            player = playerEntity.GetComponent<Player>();
            if (player.name == name) return player;
        }
        return null;
    }
}

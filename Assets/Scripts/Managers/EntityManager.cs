using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;
using System;

namespace Assets.Scripts.Managers
{
    public class EntityManager : MonoBehaviour
    {
        public GameObject[] prefabs;

        internal Dictionary<Coordinates, Entity>.ValueCollection PlayerCollection
        {
            get
            {
                if (players == null) return null;
                return players.Values;
            }
        }

        internal Dictionary<Coordinates, Entity>.ValueCollection MobCollection
        {
            get
            {
                if (mobs == null) return null;
                return mobs.Values;
            }
        }

        //...

        private WorldManager worldManager;
        private GameManager gameManager;
        private PhysicsManager physicsManager;
        private TileMapManager tileMapManager;
        private MovementManager movementManager;
        private ScreenManager screenManager;

        private System.Random randomizer = new System.Random();
        private Dictionary<string, GameObject> prefabDictionary;

        private Dictionary<Coordinates, Entity> players;
        private Dictionary<Coordinates, Entity> mobs;
        //private public Dictionary<Coordinates, Location> npcs;
        //private public Dictionary<Coordinates, Location> newTerrain;
        //private public Dictionary<Coordinates, Location> props;
        //...

        void Start()
        {
            //Load prefabs into dictionary for easy retrieval by name
            prefabDictionary = new Dictionary<string, GameObject>();
            foreach (GameObject prefab in prefabs)
            {
                prefabDictionary.Add(prefab.name, prefab);
            }
            tileMapManager = GetComponent<TileMapManager>();
            movementManager = GetComponent<MovementManager>();
            worldManager = GetComponent<WorldManager>();
            gameManager = GetComponent<GameManager>();
            physicsManager = GetComponent<PhysicsManager>();
            screenManager = GetComponent<ScreenManager>();

            players = new Dictionary<Coordinates, Entity>();
            mobs = new Dictionary<Coordinates, Entity>();
            //npcs = new Dictionary<Coordinates, Entity>();
            //newTerrain = new Dictionary<Coordinates, Entity>();
            //props = new Dictionary<Coordinates, Entity>();
            //...
        }

        /* METHODS */
        /// <summary>
        /// Creates instance of an entity, at the given coordinates on the map and adds it to entity data.
        /// </summary>
        /// <param name="name">Prefab name</param>
        /// <param name="coordinates">Map coordinates</param>
        /// <returns>Entity component of new instance.</returns>
        internal Entity Spawn(string name, Coordinates coordinates)
        {
            if (!worldManager) return null;
            
            Chunk chunk = worldManager.GetLoadedChunk(coordinates);
            return Spawn(name, chunk, new IntegerPair(coordinates.InChunks.I, coordinates.InChunks.J));
        }

        /// <summary>places it.
        /// Creates instance of an entity, attached to the given chunk, and at the given chunk indices and adds it to entity data.
        /// </summary>
        /// <param name="name">Name of prefab to instance for entity</param>
        /// <param name="chunk">Map chunk to spawn entity</param>
        /// <param name="tileIndices">Tile indices in the chunk to spawn entity</param>
        /// <returns>Entity component of new instance.</returns>
        internal Entity Spawn(string name, Chunk chunk, IntegerPair tileIndices)
        {
            GameObject prefab = prefabDictionary[name];
            GameObject gOEntity;

            gOEntity = Instantiate(
                prefab, 
                screenManager.GetScreenPositionAt(new Coordinates(chunk.lowerLeft.InWorld.X + tileIndices.I, chunk.lowerLeft.InWorld.Y + tileIndices.J)),
                Quaternion.identity
                );

            Entity entity = gOEntity.GetComponent<Entity>();

            if (entity)
            {
                if (!Place(entity, chunk, tileIndices))
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
            if (!entity) return;

            if (entity.Type == "Player")
            {
                players.Remove(entity.Coordinates);
            }
            else if (entity.Type == "Mob")
            {
                mobs.Remove(entity.Coordinates);
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
            Entity[] entityArray;

            // Despawn player entities from the player collection and then clear the collection
            if (players != null)
            {
                entityArray = new Entity[players.Values.Count];
                players.Values.CopyTo(entityArray, 0);
                foreach (Entity player in entityArray)
                {
                    Despawn(player);
                }
                players.Clear();
            }

            // Despawn mob entities from the mob colleaciton and clear the collection
            if (mobs != null)
            {
                entityArray = new Entity[mobs.Values.Count];
                mobs.Values.CopyTo(entityArray, 0);
                foreach (Entity mob in entityArray)
                {
                    Despawn(mob);
                }
                mobs.Clear();

            }
            // ... other entity types
        }

        /// <summary>
        /// Checks entity data if an entity of a similar given type is at given coordinates.
        /// </summary>
        /// <param name="coordinates">Map coordinates</param>
        /// <param name="entityType">An entity type</param>
        /// <returns>True if occupied, else false.</returns>
        internal bool IsOccupied(Coordinates coordinates, string entityType)
        {
            if (entityType == "Player" || entityType == "Mob") //Mobs and Players can't share the same location
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

            int population = randomizer.Next(10);

            //for (int i = 0; i < population; i++)
            //{
            //    IntegerPair indices = new IntegerPair(randomizer.Next(Chunk.chunkTileWidth), randomizer.Next(Chunk.chunkTileWidth));

            //    Spawn("Cow", chunk, indices);
            //}

            // START DEBUG CODE
            if (chunk.lowerLeft.InChunks == new Coordinates(0, 0).InChunks)
            {
                Spawn("Cow", chunk, new IntegerPair(randomizer.Next(Chunk.chunkTileWidth), randomizer.Next(Chunk.chunkTileWidth)));
            }
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
        /// <param name="vector">Vector of direction and distance.</param>
        /// <param name="locomotion">Method of movement by terrain type</param>
        /// <returns>Returns true if movement set successfully.</returns>
        internal bool Move(Entity entity, Vector2 vector, TerrainTile.TerrainType locomotion = TerrainTile.TerrainType.Land)
        {
            // If obstacles in the way, ignore move
            if (physicsManager)
            {
                float distance = vector.magnitude;

                if (physicsManager.GetTargetRay(entity, vector, distance)) return false;
            }

            // If we can place the entity at new coordinates set movements
            if (Place(entity, entity.Coordinates.AtVector(vector)))
            {
                Attributes attributes = entity.GetComponent<Attributes>();

                if (attributes & tileMapManager & worldManager)
                {
                    float speed = worldManager.GetSpeed(entity.Coordinates, attributes, locomotion);

                    // If entity is the player, scroll the map,
                    if (gameManager.Player == entity)
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

        /// <summary>
        /// Updates the entity location data, to the given chunk and it's tile indices
        /// </summary>
        /// <param name="entity">An entity component.</param>
        /// <param name="chunk">Chunk to place entity.</param>
        /// <param name="tileIndices">Tile indices in chunk to place entity.</param>
        /// <returns></returns>
        internal bool Place(Entity entity, Chunk chunk, IntegerPair tileIndices)
        {
            if (!entity) return false;

            Coordinates coordinates = new Coordinates(chunk, tileIndices);

            //Entity obstacle = physicsManager.GetObstacle(coordinates);

            if (!IsOccupied(coordinates, entity.Type))
            {
                if (entity.Type == "Player")
                {
                    if (entity.Placed) players.Remove(entity.Coordinates);
                    players.Add(coordinates, entity);
                }
                else if (entity.Type == "Mob")
                {
                    if (entity.Placed) mobs.Remove(entity.Coordinates);
                    mobs.Add(coordinates, entity);
                }
                // else if other entity types

                if (entity.chunk != chunk && chunk != null)
                {
                    if (entity.chunk != null) entity.chunk.entitySet.Remove(entity);
                    entity.chunk = chunk;
                    chunk.entitySet.Add(entity);
                }
                entity.tileIndices = tileIndices;
                entity.Placed = true;

                return true;
            }
            else return false;
        }



        /// <summary>
        /// Updates the entity coordinates, as long as the location isn't occupied
        /// </summary>
        /// <param name="entity">An entity component</param>
        /// <param name="coordinates">Map coordinates</param> 
        /// <returns>True if successful, false if entity is null, or coordinates are occupied.</returns>
        internal bool Place(Entity entity, Coordinates coordinates)
        {
            Chunk chunk = worldManager.GetLoadedChunk(coordinates);

            if (chunk != null)
                return Place(entity, chunk, new IntegerPair(coordinates.InChunks.I, coordinates.InChunks.J));
            else
                return false;

        }

        /// <summary>
        /// Returns the client's player component
        /// </summary>
        /// <param name="name">Name of the entity</param>
        /// <returns>Player component with given name</returns>
        internal Player GetPlayer(string name)
        {
            if (PlayerCollection == null) return null;

            Player player;
            
            foreach (Entity playerEntity in PlayerCollection)
            {
                player = playerEntity.GetComponent<Player>();
                if (player.name == name) return player;
            }
            return null;
        }
    }
}
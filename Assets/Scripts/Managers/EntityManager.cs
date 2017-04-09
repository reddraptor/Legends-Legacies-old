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
        public EntityCollection playerCollection;
        public EntityCollection mobCollection;
        //private public Dictionary<Coordinates, Location> npcs;
        //private public Dictionary<Coordinates, Location> newTerrain;
        //private public Dictionary<Coordinates, Location> props;
        //...


        //...

        private WorldManager worldManager;
        private GameManager gameManager;
        private PhysicsManager physicsManager;
        private TileMapManager tileMapManager;
        private MovementManager movementManager;
        private ScreenManager screenManager;

        private System.Random randomizer = new System.Random();
        private Dictionary<string, GameObject> prefabDictionary;

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

        }

        /* METHODS */
        /// <summary>
        /// Creates instance of an entity, at the given coordinates on the map and adds it to entity data.
        /// </summary>
        /// <param name="prefabName">Prefab name</param>
        /// <param name="coordinates">Map coordinates</param>
        /// <returns>Entity component of new instance.</returns>
        internal Entity Spawn(string prefabName, Coordinates coordinates)
        {
            if (!worldManager) return null;
            
            Chunk chunk = worldManager.GetLoadedChunk(coordinates);
            return Spawn(prefabName, chunk, new IntegerPair(coordinates.InChunks.I, coordinates.InChunks.J));
        }

        /// <summary>places it.
        /// Creates instance of an entity, attached to the given chunk, and at the given chunk indices and adds it to entity data.
        /// </summary>
        /// <param name="prefabName">Name of prefab to instance for entity</param>
        /// <param name="chunk">Map chunk to spawn entity</param>
        /// <param name="tileIndices">Tile indices in the chunk to spawn entity</param>
        /// <returns>Entity component of new instance.</returns>
        internal Entity Spawn(string prefabName, Chunk chunk, IntegerPair tileIndices)
        {
            GameObject prefab = prefabDictionary[prefabName];
            GameObject gOEntity;

            gOEntity = Instantiate(
                prefab, 
                screenManager.GetScreenPositionAt(new Coordinates(chunk.lowerLeft.InWorld.X + tileIndices.I, chunk.lowerLeft.InWorld.Y + tileIndices.J)),
                Quaternion.identity
                );

            EntityMember entityMember = gOEntity.GetComponent<EntityMember>();

            if (entityMember)
            {
                if (!Place(entityMember, chunk, tileIndices))
                {
                    Destroy(entityMember.gameObject);
                    return null;
                }
                return entityMember;
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
                playerCollection.Remove(entity);
            }
            else if (entity.Type == "Mob")
            {
                mobCollection.Remove(entity);
            }
            // ... else if other entity types

            entity.Chunk.entitySet.Remove(entity);
            Destroy(entity.gameObject);
        }

        /// <summary>
        /// All entity data is cleared and attached gameobjects destroyed. Typically called when one game session ends before another begins.
        /// </summary>
        internal void CleanUpSpawns()
        {
            // Despawn player entities from the player collection and then clear the collection
            if (playerCollection != null)
            {
                foreach (EntityMember player in playerCollection.Members)
                {
                    Despawn(player);
                }
                playerCollection.Clear();
            }

            // Despawn mob entities from the mob colleaciton and clear the collection
            if (mobCollection != null)
            {
                foreach (EntityMember mob in mobCollection.Members)
                {
                    Despawn(mob);
                }
                mobCollection.Clear();

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
                if (playerCollection.MemberAt(coordinates)) return true;
                else if (mobCollection.MemberAt(coordinates)) return true;
                else return false;
            }
            else throw new ArgumentException("IsFree(Coordinates, Entity.Entity_Type): Given entity type undefined.");
        }


        internal void Populate(Chunk chunk)
        {
            if (chunk == null) return;

            int population = randomizer.Next(10);

            for (int i = 0; i < population; i++)
            {
                IntegerPair indices = new IntegerPair(randomizer.Next(Chunk.chunkTileWidth), randomizer.Next(Chunk.chunkTileWidth));

                Spawn("Cow", chunk, indices);
            }

            // START DEBUG CODE
            //if (chunk.lowerLeft.InChunks == new Coordinates(0, 0).InChunks)
            //{
            //    Spawn("Cow", chunk, new IntegerPair(randomizer.Next(Chunk.chunkTileWidth), randomizer.Next(Chunk.chunkTileWidth)));
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
                Despawn((EntityMember)entity);
            }
        }

        /// <summary>
        /// Attempts to set movement of an entity. If it's the client player, sets the map to scroll.
        /// </summary>
        /// <param name="entity">Entity to set movement on.</param>
        /// <param name="vector">Vector of direction and distance.</param>
        /// <param name="locomotion">Method of movement by terrain type</param>
        /// <returns>Returns true if movement set successfully.</returns>
        internal bool Move(Entity entity, Vector2 vector, TerrainTileEntity.TerrainType locomotion = TerrainTileEntity.TerrainType.Land)
        {
            // If obstacles in the way, ignore move
            if (physicsManager)
            {
                float distance = vector.magnitude;

                if (physicsManager.GetTargetRay(entity, vector, distance)) return false;
            }

            // If we can place the entity at new coordinates set movements
            if (Place((EntityMember)entity, entity.Coordinates.AtVector(vector)))
            {
                Attributes attributes = entity.GetComponent<Attributes>();

                if (attributes & tileMapManager & worldManager)
                {
                    float speed = worldManager.GetSpeed(entity.Coordinates, attributes, locomotion);

                    // If entity is the player, scroll the map,
                    if (gameManager.client.controlledEntity == entity)
                    {
                        tileMapManager.Scroll(vector, speed);
                        return true;
                    }
                    // else set movement of the entity
                    else
                    {
                        movementManager.Assign(entity, vector, speed);
                        return true;
                    }
                }
            }
            return false;

        }

        /// <summary>
        /// Updates the entity location data, to the given chunk and it's tile indices
        /// </summary>
        /// <param name="entityMember">An entity component.</param>
        /// <param name="chunk">Chunk to place entity.</param>
        /// <param name="tileIndices">Tile indices in chunk to place entity.</param>
        /// <returns></returns>
        internal bool Place(EntityMember entityMember, Chunk chunk, IntegerPair tileIndices)
        {
            if (entityMember && chunk != null)
            {
                if (!IsOccupied(new Coordinates(chunk, tileIndices), entityMember.Type))
                {
                    entityMember.SetLocation(chunk, tileIndices);

                    if (entityMember.Type == "Player" && !playerCollection.Contains(entityMember))
                        playerCollection.Add(entityMember);
                    else if (entityMember.Type == "Mob" && !mobCollection.Contains(entityMember))
                        mobCollection.Add(entityMember);
                    //else if other types
                    
                    entityMember.Placed = true;
                    return true;
                }
            }

            return false;
        }



        /// <summary>
        /// Updates the entity coordinates, as long as the location isn't occupied
        /// </summary>
        /// <param name="entityMember">An entity component</param>
        /// <param name="coordinates">Map coordinates</param> 
        /// <returns>True if successful, false if entity is null, or coordinates are occupied.</returns>
        internal bool Place(EntityMember entityMember, Coordinates coordinates)
        {
            Chunk chunk = worldManager.GetLoadedChunk(coordinates);

            if (chunk != null)
                return Place(entityMember, chunk, new IntegerPair(coordinates.InChunks.I, coordinates.InChunks.J));
            else
                return false;

        }

        /// <summary>
        /// Returns the client's player component
        /// </summary>
        /// <param name="name">Name of the entity</param>
        /// <returns>Player component with given name</returns>
        internal PlayerEntity GetPlayer(string name)
        {
            if (playerCollection == null) return null;

            PlayerEntity player;
            
            foreach (EntityMember playerMember in playerCollection.Members)
            {
                player = playerMember.GetComponent<PlayerEntity>();
                if (player.name == name) return player;
            }
            return null;
        }

        internal PlayerEntity SpawnPlayer(string name)
        {
            Coordinates spawnCoords = worldManager.world.PlayerSpawnCoordinates;
            EntityMember otherPlayer = playerCollection.MemberAt(spawnCoords);
            EntityMember otherMob = mobCollection.MemberAt(spawnCoords);

            if (otherPlayer) 
            {
                Despawn(otherPlayer); // temporary solution
            }
            else if (otherMob)
            {
                Despawn(otherMob); // temporary solution
            }

            PlayerEntity player = (PlayerEntity)Spawn("Player", spawnCoords);
            player.name = name;
            return player;
        }
    }
}
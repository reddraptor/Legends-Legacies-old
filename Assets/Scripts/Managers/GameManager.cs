using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;
using Client = Assets.Scripts.Components.Client;

namespace Assets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public Client client;

        private WorldManager worldManager;
        private TileMapManager tileMapManager;
        private MenuManager menuManager;
        private EntityManager entityManager;

        void Start()
        {
            worldManager = GetComponent<WorldManager>();
            tileMapManager = GetComponent<TileMapManager>();
            menuManager = GetComponent<MenuManager>();
            entityManager = GetComponent<EntityManager>();
            
            if (menuManager)
            {
                //Load a random map as a background
                if (tileMapManager & worldManager)
                {
                    worldManager.Seed = new System.Random().Next(100000);
                    worldManager.LoadChunksAt(new Coordinates(0, 0));
                    tileMapManager.SetFocus(new Coordinates(0, 0));
                    tileMapManager.ShowMap(true);
                }
                menuManager.CloseAll();
                menuManager.Open("Main Menu");
            }
            else
                BeginGame();
        }


        internal void BeginGame()
        {
            ClearGame();
            if (tileMapManager)
            {
                tileMapManager.ShowMap(false);

                if (entityManager & worldManager)
                {
                    if (!(client.controlledEntity = entityManager.GetPlayer("Player One")))
                    {
                        worldManager.LoadChunksAt(worldManager.world.PlayerSpawnCoordinates);
                        tileMapManager.SetFocus(worldManager.world.PlayerSpawnCoordinates);
                        client.controlledEntity = entityManager.SpawnPlayer("Player One");
                    }
                }
                tileMapManager.ShowMap(true);
            }
        }

        void ClearGame()
        {
            if (entityManager) entityManager.CleanUpSpawns();
        }

    }

}
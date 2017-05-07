using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;
using Client = Assets.Scripts.Components.Client;

namespace Assets.Scripts.Managers
{
    public class GameManager : Manager
    {
        public Client client;

        protected override void Start()
        {
            base.Start();
            
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

                if (entitiesManager & worldManager)
                {
                    if (!(client.controlledEntity = entitiesManager.GetPlayer("Player One")))
                    {
                        worldManager.LoadChunksAt(worldManager.world.PlayerSpawnCoordinates);
                        tileMapManager.SetFocus(worldManager.world.PlayerSpawnCoordinates);
                        client.controlledEntity = entitiesManager.SpawnPlayer("Player One");
                    }
                }
                tileMapManager.ShowMap(true);
            }
        }

        void ClearGame()
        {
            if (entitiesManager) entitiesManager.CleanUpSpawns();
        }

    }

}
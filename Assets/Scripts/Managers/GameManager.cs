using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;
using System;

namespace Assets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public Player Player
        {
            get { return player; }
        }

        private Player player;
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
                    player = entityManager.GetPlayer("Player One");
                    if (!player)
                    {
                        worldManager.LoadChunksAt(worldManager.world.PlayerSpawnCoordinates);
                        tileMapManager.SetFocus(worldManager.world.PlayerSpawnCoordinates);
                        player = (Player)entityManager.Spawn("Player", worldManager.world.PlayerSpawnCoordinates);
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
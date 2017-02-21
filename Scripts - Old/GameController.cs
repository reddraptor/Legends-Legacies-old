using UnityEngine;
using Assets.Scripts.UI_Scripts.Menus;
using Assets.Scripts.Behaviors;

namespace Assets.OldScripts
{
    [RequireComponent(typeof(MenuController))]
    [RequireComponent(typeof(InputController))]
    [RequireComponent(typeof(SpawnController))]
    [RequireComponent(typeof(BehaviorController))]

    public class GameController : MonoBehaviour
    {
        [Header("UI Objects")]

        [Header("Game Objects")]
        public GameObject tileMapObject;
        public GameWorld gameWorld;
        public GameObject mainCamera;

        [Header("Input Settings")]
        [Range(0.05f, 0.001f)]
        public float mouseScrollEdgePercentage = 0.05f; //Percent of the total screen size at edge where mouse scroll is active.

        GameObject playerMobile;
        Coordinates spawnLocation;
        MainMenuController mainMenuController;
        InputController inputController;
        SpawnController spawnController;
        BehaviorController behaviorController;
        bool start = true;


        /* PROPERTIES */
        public TileMap tileMap
        {
            get
            {
                return tileMapObject.GetComponent<TileMap>();
            }
        }

        public Vector2 OrthographicSize
        {
            get
            {
                Vector2 orthoSize = new Vector2();
                orthoSize.y = mainCamera.GetComponent<Camera>().orthographicSize;
                orthoSize.x = (orthoSize.y * mainCamera.GetComponent<Camera>().aspect);
                return orthoSize;
            }
        }

        public BehaviorController BehaviorController
        {
            get { return behaviorController; }
        }

        /* MONOBEHAVIOUR METHODS */

        // Use this for initialization
        void Start()
        {
            // Setup the game world with a random seed to use as a background for the main menu
            gameWorld.GameController = this;
            gameWorld.Seed = new System.Random().Next(100000);
            spawnLocation = new Coordinates(gameWorld.SPAWN_LOCATION_X, gameWorld.SPAWN_LOCATION_Y);

            gameWorld.LoadChunksAt(spawnLocation);

            // Create a player instance, but hide it for now. It will always reside center screen. Set it to predetermined spawn location.
            playerMobile = null; // Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                                   //playerInstance.SetActive(false);
                                   //gameWorld.Player = playerInstance.GetComponent<Player>();
                                   //gameWorld.MasterLocationList.ChangeAsset(gameWorld.Player, spawnLocation);

            // Setup the tile map
            tileMap.gameController = this;
            tileMapObject.SetActive(true);

            //Get the input controller
            inputController = GetComponent<InputController>();

            // Get the UI elements
            mainMenuController = GetComponent<MainMenuController>();

            // Get the spawn controller
            spawnController = GetComponent<SpawnController>();

            // Get the behavior controller
            behaviorController = GetComponent<BehaviorController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (start)
            {
                // Start with the main menu up. Save function inactive, as there is no game started yet
                mainMenuController.CloseAll();
                mainMenuController.Open("Main Menu");
                (mainMenuController.GetMenu("Main Menu") as MainMenu).saveEnabled = false;
                start = false;
            }
        }

        public void StartGame()
        {
            if (gameWorld.Player == null)
            {
                gameWorld.Player = spawnController.SpawnPlayer(spawnLocation);
            }
            
            StartMap();
            (mainMenuController.GetMenu("Main Menu") as MainMenu).saveEnabled = true;
        }

        public void CenterOnPlayer()
        {
            tileMap.ChangeFocus(gameWorld.Player.Entity.Coordinates);
            tileMap.CreateTileMap();
            gameWorld.Player.Mobile.Center();
        }

        //public void SpawnNewPlayer()
        //{
        //    gameWorld.Player = Instantiate(playerPrefab);
        //    gameWorld.Player.Entity = spawnController.Spawn(1, spawnLocation);   
        //}

        public void StartMap()
        {
            gameWorld.LoadChunksAt(gameWorld.Player.Entity.Coordinates);
            tileMap.ChangeFocus(gameWorld.Player.Entity.Coordinates);
            tileMap.CreateTileMap();
        }
    }

}
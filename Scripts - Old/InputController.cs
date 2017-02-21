using UnityEngine;
using System.Collections;
using Assets.Scripts.UI_Scripts.Menus;

namespace Assets.OldScripts
{
    [RequireComponent(typeof(GameController))]
    [RequireComponent(typeof(MenuController))]
    public class InputController : MonoBehaviour
    {

        public KeyCode mainMenuKey = KeyCode.Escape;
        public KeyCode mouseScrollKey = KeyCode.LeftControl;

        public float mouseScrollCenterPercent = 0.1f;

        public GameObject mouseScrollReminder;

        GameController gameController;
        MainMenuController mainMenuController;

        IntegerPair inputDirection;
        bool menuKeyUp;
        bool mouseScrollKeyDown;
        bool mouseScrollActive;
        bool mouseScrollKeyUp;
        Vector2 mousePosition;
        IntegerPair mouseScrollDirection;


        // Use this for initialization
        void Start()
        {
            gameController = GetComponent<GameController>();
            mainMenuController = GetComponent<MainMenuController>();
            mouseScrollReminder.GetComponent<Canvas>().enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

            inputDirection = new IntegerPair((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical"));
            menuKeyUp = Input.GetKeyUp(mainMenuKey);
            mouseScrollKeyDown = Input.GetKeyDown(mouseScrollKey);
            mouseScrollKeyUp = Input.GetKeyUp(mouseScrollKey);
            mousePosition = Input.mousePosition;
            mouseScrollDirection = GetMouseScrollDirection(mousePosition);


            if (menuKeyUp)
            {
                if (!mainMenuController.ownMenuOpen)
                    mainMenuController.Open("Main Menu");
                else
                    mainMenuController.Return();
            }

            else if (mouseScrollKeyDown)
            {
                if (mouseScrollActive == false) mouseScrollActive = true;
                mouseScrollReminder.GetComponent<Canvas>().enabled = true;
            }

            else if (mouseScrollKeyUp)
            {
                if (mouseScrollActive == true) mouseScrollActive = false;
                mouseScrollReminder.GetComponent<Canvas>().enabled = false;
            }

            else if (!gameController.tileMap.Mobile.isMoving && !mainMenuController.ownMenuOpen)
            {
                if ((inputDirection.x != 0 || inputDirection.y != 0) && gameController.gameWorld.Player != null)
                {
                    if (inputDirection.x < 0)
                    {
                        gameController.gameWorld.MasterLocationList.MoveAsset<Entity>(gameController.gameWorld.Player.Entity.Coordinates, gameController.gameWorld.Player.Entity.Coordinates.West(1));
                    }
                    else if (inputDirection.x > 0)
                    {
                        gameController.gameWorld.MasterLocationList.MoveAsset<Entity>(gameController.gameWorld.Player.Entity.Coordinates, gameController.gameWorld.Player.Entity.Coordinates.East(1));
                    }

                    if (inputDirection.y < 0)
                    {
                        gameController.gameWorld.MasterLocationList.MoveAsset<Entity>(gameController.gameWorld.Player.Entity.Coordinates, gameController.gameWorld.Player.Entity.Coordinates.South(1));
                    }
                    else if (inputDirection.y > 0)
                    {
                        gameController.gameWorld.MasterLocationList.MoveAsset<Entity>(gameController.gameWorld.Player.Entity.Coordinates, gameController.gameWorld.Player.Entity.Coordinates.North(1));
                    }

                    if (inputDirection.x != 0 || inputDirection.y != 0)
                    {
                        if (!gameController.gameWorld.Player.Mobile.IsCentered) gameController.CenterOnPlayer();
                        float playerSpeed = gameController.gameWorld.Player.Entity.baseSpeed * gameController.gameWorld.SpeedModifierAt(gameController.gameWorld.Player.Entity.Coordinates);
                        StartCoroutine(gameController.tileMap.Scroll(inputDirection.x, inputDirection.y, playerSpeed));
                    }
                }

                else if (mouseScrollActive)
                {
                    if ((mouseScrollDirection.x != 0 || mouseScrollDirection.y != 0))
                    {
                        // Ensure map doesn't scroll horizontally out of player's sight range.
                        Coordinates newFocusHorizontal = new Coordinates(gameController.tileMap.focusLocation.World.X + mouseScrollDirection.x, gameController.tileMap.focusLocation.World.Y);
                        if (mouseScrollDirection.x != 0
                            && gameController.gameWorld.Player.Entity.Coordinates.Range(newFocusHorizontal) > gameController.gameWorld.Player.Entity.sightRange - Chunk.tileWidth / 2)
                        {
                            mouseScrollDirection.x = 0;
                        }

                        // Ensure map doesn't scroll vertically out of player's sight range.
                        Coordinates newFocusVertical = new Coordinates(gameController.tileMap.focusLocation.World.X, gameController.tileMap.focusLocation.World.Y + mouseScrollDirection.y);
                        if (mouseScrollDirection.y != 0
                            && gameController.gameWorld.Player.Entity.Coordinates.Range(newFocusVertical) > gameController.gameWorld.Player.Entity.sightRange - Chunk.tileWidth / 2)
                        {
                            mouseScrollDirection.y = 0;
                        }

                        // If scrolling out of players sight range both directions then return.
                        if (mouseScrollDirection.x == 0 && mouseScrollDirection.y == 0) return;

                        Coordinates newFocusLocation
                            = new Coordinates(gameController.tileMap.focusLocation.World.X + mouseScrollDirection.x, gameController.tileMap.focusLocation.World.Y + mouseScrollDirection.y);

                        // Ensure the scrolling of both directions would not be out of player's sight. Not sure if this is necessesary?!?!
                        if (gameController.gameWorld.Player.Entity.Coordinates.Range(newFocusLocation) > gameController.gameWorld.Player.Entity.sightRange - Chunk.tileWidth / 2) return;

                        //if (!centerKeyReminder.activeInHierarchy) centerKeyReminder.SetActive(true);
                        StartCoroutine(gameController.gameWorld.Player.Mobile.Move(-mouseScrollDirection.x, -mouseScrollDirection.y, gameController.tileMap.scrollSpeed));
                        StartCoroutine(gameController.tileMap.Scroll(mouseScrollDirection.x, mouseScrollDirection.y));
                    }
                }

                else if (!mouseScrollActive)
                {
                    if (!gameController.gameWorld.Player.Mobile.IsCentered) gameController.CenterOnPlayer();
                }
            }
        }

        IntegerPair GetMouseScrollDirection(Vector2 mousePos)
        {
            IntegerPair direction = new IntegerPair(0, 0);
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 lowerLeftOfCenter = new Vector2(screenCenter.x - Screen.width * mouseScrollCenterPercent / 2, screenCenter.y - Screen.height * mouseScrollCenterPercent / 2);
            Vector2 upperRightOfCenter = new Vector2(screenCenter.x + Screen.width * mouseScrollCenterPercent / 2, screenCenter.y + Screen.height * mouseScrollCenterPercent / 2);

            // if mouse position is outside screen limits return zero direction
            if (mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height) return direction;

            // determine directions to scroll
            if (mousePos.x < lowerLeftOfCenter.x)
                direction.x = -1;

            else if (mousePos.x > upperRightOfCenter.x)
                direction.x = 1;

            if (mousePos.y < lowerLeftOfCenter.y)
                direction.y = -1;

            else if (mousePos.y > upperRightOfCenter.y)
                direction.y = 1;

            return direction;
        }

    }
}


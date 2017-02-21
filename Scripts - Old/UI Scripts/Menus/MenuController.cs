using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.UI_Scripts.Menus
{

    [RequireComponent(typeof(GameController))]
    public class MenuController : MonoBehaviour
    {
        public GameObject[] menuPrefabs;

        Dictionary<string, Menu> menuTaggedWith;
        GameController gameCtrlr;
        Menu lastMenu;

        public GameController gameController
        {
            get { return gameCtrlr; }
        }

        // Use this for initialization
        void Start()
        {
            gameCtrlr = GetComponent<GameController>();

            menuTaggedWith = new Dictionary<string, Menu>();

            foreach (GameObject prefab in menuPrefabs)
            {
                Menu menu = Instantiate<GameObject>(prefab).GetComponent<Menu>();
                if (menu.menuController == null)
                {
                    menuTaggedWith.Add(prefab.tag, menu);
                    menu.menuController = this;
                }
                else
                {
                    Debug.Log("Can not add menu object to menu controller. Object's menu controller already set: " + menu.ToString());
                }
            }
        }


        public void Close(string goTag)
        {
            if (menuTaggedWith == null)
            {
                Debug.Log("Close menu failed: No menus added to controller.");
                return;
            }

            Menu menu;
            if (menuTaggedWith.TryGetValue(goTag, out menu))
            {
                menu.open = false;
            }
            else
            {
                Debug.Log("Close menu failed: No menu with tag " + goTag);
            }
        }

        public void CloseAll()
        {
            foreach (Menu menu in menuTaggedWith.Values)
            {
                menu.open = false;
            }
        }

        public bool IsOpen(string goTag)
        {
            if (menuTaggedWith == null) return false;
            Menu menu;
            if (menuTaggedWith.TryGetValue(goTag, out menu))
                return menu.open;
            else return false;
        }

        public bool ownMenuOpen
        {
            get
            {
                if (menuTaggedWith == null) return false;

                foreach (string menuTag in menuTaggedWith.Keys)
                {
                    if (IsOpen(menuTag)) return true;
                }

                return false;
            }
        }

        public void Open(string goTag)
        {
            if (menuTaggedWith == null)
            {
                Debug.Log("Open menu failed: No menus added to controller");
                return;
            }

            Menu menu;
            if (menuTaggedWith.TryGetValue(goTag, out menu))
            {
                if (!menu.open)
                {
                    menu.open = true;
                    lastMenu = menu;
                }
                else
                {
                    Debug.Log("OpenFrom failed: Menu already open.");
                }
            }
            else
            {
                Debug.Log("OpenFrom failed: No menu with tag " + goTag);
            }
        }

        public Menu GetMenu(string goTag)
        {
            if (menuTaggedWith == null)
            {
                Debug.Log("GetMenuWithTag failed: No menus added to controller.");
                return null;
            }

            Menu menu;
            if (menuTaggedWith.TryGetValue(goTag, out menu))
            {
                return menu;
            }
            else
            {
                Debug.Log("GetMenuWithTag failed: No menu with tag " + goTag);
                return null;
            }
        }

        public void Return()
        {
            if (lastMenu.open)
            {
                lastMenu.Return();
            }
        }
    }

}
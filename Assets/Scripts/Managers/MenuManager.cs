using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.UI_Components.Menu;

public class MenuManager : MonoBehaviour
{ 
    public GameObject[] menuPrefabs;
    
    Dictionary<string, AMenu> menuDictionary;
    AMenu lastOpened;
    AMenu lastClosed;

    public bool anyOpen
    {
        get
        {
            foreach(AMenu menu in menuDictionary.Values)
            {
                if (menu.open) return true;
            }
            return false;
        }
    }

    /* UNITY MESSAGES */
    // Use this for initialization
    void Start()
    {
        menuDictionary = new Dictionary<string, AMenu>();

        foreach(GameObject prefab in menuPrefabs)
        {
            AMenu menu = Instantiate(prefab).GetComponent<AMenu>();
            menu.open = false;
            menuDictionary.Add(prefab.name, menu);
            menu.menuManager = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /* METHODS */

    public void Open(string menuName, AMenu callingMenu)
    {
        AMenu menu = GetMenu(menuName);

        if (menu)
        {
            if (callingMenu) menu.callingMenu = callingMenu;
            Open(menu);
        }
    }

    public void Open(string menuName)
    {
        Open(menuName, null);
    }

    public void Open(AMenu menu)
    {
        if (menu)
        {
            menu.OnOpen();
            menu.open = true;
            lastOpened = menu;
        }
    }


    public void Close(string menuName)
    {
        Close(GetMenu(menuName));
    }

    public void Close(AMenu menu)
    {
        if (menu)
        {
            menu.OnClose();
            menu.open = false;
            lastClosed = menu;
        }
    }

    public void CloseAll()
    {
        foreach (AMenu menu in menuDictionary.Values) Close(menu);
    }

    public void Return(string menuName)
    {
        AMenu menu = GetMenu(menuName);
        if (menu)
        {
            if (menu.callingMenu) Open(menu.callingMenu);
        }
        Close(menuName);
    }

    public void Return()
    {
        if (lastOpened)
        {
            Close(lastOpened);
            if (lastOpened.callingMenu) Open(lastOpened.callingMenu);
        }
    }

    public AMenu GetMenu(string menuName)
    {
        AMenu menu;

        if (menuDictionary.TryGetValue(menuName, out menu))
        {
            return menu;
        }
        else return null;
    }

    public bool IsOpen(string menuName)
    {
        AMenu menu = GetMenu(menuName);
        if (menu)
            return menu.open;
        else return false;
    }
}

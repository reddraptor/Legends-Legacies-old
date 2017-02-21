using UnityEngine;
using System.Collections;
using Assets.Scripts.UI_Components;
using Assets.Scripts.Managers;

namespace Assets.Scripts.UI_Components.Menu
{
    public class Menu_Load : AMenu
    {
        public SelectableList saveGameList;

        string[] saveFileNames;
        GameManager gameManager;
        PersistanceManager persistanceManager;
        Menu_Main mainMenu;

        public override void OnClose()
        {
            
        }

        public override void OnOpen()
        {
            gameManager = menuManager.GetComponent<GameManager>();
            persistanceManager = menuManager.GetComponent<PersistanceManager>();
            mainMenu = (Menu_Main)menuManager.GetMenu("Main Menu");
            if (persistanceManager)
            {
                saveFileNames = persistanceManager.GetSaveFileNames();
                saveGameList.Clear();
                for (int i = 0; i < saveFileNames.Length; i++)
                {
                    saveGameList.Add(saveFileNames[i]);
                }
            }
        }

        public void OnLoad()
        {
            string selectedText = saveGameList.GetSelectedText();
            if (persistanceManager && (selectedText != ""))
            {
                if (gameManager) gameManager.ClearGame();
                persistanceManager.Load(selectedText);
                if (gameManager) gameManager.BeginGame();
                mainMenu.saveEnabled = true;
                menuManager.Close(this);
            }
        }

        public void OnDelete()
        {
            if (persistanceManager) persistanceManager.Delete(saveGameList.GetSelectedText());
            saveGameList.RemoveSelected();
        }

    }

}
using System;
using UnityEngine.UI;
using Assets.Scripts.Managers;

namespace Assets.Scripts.UI_Components.Menu
{
    class Menu_Create : AMenu
    {
        public InputField seedField;

        WorldManager worldManager;
        GameManager gameManager;
        Menu_Main mainMenu;

        public override void OnClose()
        {
            //throw new NotImplementedException();
        }

        public override void OnOpen()
        {
            if (seedField) seedField.text = new Random().Next(100000).ToString();
            worldManager = menuManager.GetComponent<WorldManager>();
            gameManager = menuManager.GetComponent<GameManager>();
            mainMenu = (Menu_Main)menuManager.GetMenu("Main Menu");
        }

        public void OnCreateWorld()
        {
            if (seedField & worldManager) worldManager.Seed = ConvertTextToSeed(seedField.text);
            if (gameManager) gameManager.BeginGame();
            if (mainMenu) mainMenu.saveEnabled = true;
            if (menuManager) menuManager.Close(this);
        }

        private int ConvertTextToSeed(string text)
        {
            int seed = 0;
            char[] charArray = text.ToCharArray();

            for (int i = 0; i < charArray.Length; i++)
            {
                seed = seed + charArray[i];
            }

            return seed;
        }

    }
}

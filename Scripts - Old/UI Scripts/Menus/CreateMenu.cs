using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.UI_Scripts.Menus
{
    public class CreateMenu : Menu
    {

        public InputField seedField;
        //public GameController gameController;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {

        }

        protected override void OnOpened()
        {
            base.OnOpened();
            seedField.text = new System.Random().Next(100000).ToString();
        }

        public void OnCreateWorld()
        {
            menuController.gameController.gameWorld.Seed = ConvertTextToSeed(seedField.text);
            menuController.gameController.StartGame();
            menuController.Close(tag);
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
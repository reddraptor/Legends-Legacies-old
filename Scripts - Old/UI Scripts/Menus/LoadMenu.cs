using UnityEngine;
using System.Collections;

namespace Assets.Scripts.UI_Scripts.Menus
{
    public class LoadMenu : Menu
    {

        public GameObject saveListContent;

        string[] saveFileNames;
        SelectableList saveGameList;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            saveGameList = saveListContent.GetComponent<SelectableList>();
        }

        // Update is called once per frame
        protected override void Update()
        {

        }

        protected override void OnOpened()
        {
            base.OnOpened();
            saveFileNames = WorldSave.GetSaveFileNames();
            saveGameList.Clear();
            for (int i = 0; i < saveFileNames.Length; i++)
            {
                saveGameList.Add(saveFileNames[i]);
            }
        }

        public void OnLoad()
        {
            WorldSave.Load(saveGameList.GetSelectedText(), menuController.gameController.gameWorld);
            menuController.gameController.StartGame();
            Close();
        }

        public void OnDelete()
        {
            WorldSave.Delete(saveGameList.GetSelectedText());
            saveGameList.RemoveSelected();
        }
    }

}
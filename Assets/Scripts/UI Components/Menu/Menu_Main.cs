using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI_Components.Menu
{
    class Menu_Main : AMenu
    {
        /* EDITOR FIELDS */
        public Button saveButton;

        /* PUBLIC PROPERTIES */
        public bool saveEnabled
        {
            get { return saveButton.interactable; }
            set { saveButton.interactable = value; }
        }

        /* PRIVATE FIELDS */

        /* UNITY MESSAGES */
        protected override void Awake()
        {
            base.Awake();
            saveEnabled = false;
        }

        /* METHODS */
        public void OnQuit()
        {
            Debug.Log("Quitting.");
            Application.Quit();
        }

        public void OnNew()
        {
            menuManager.Open("Create Menu", this);
            menuManager.Close(this);
        }

        public void OnSave()
        {
            menuManager.Open("Save Menu", this);
            menuManager.Close(this);
        }

        public void OnLoad()
        {
            menuManager.Open("Load Menu", this);
            menuManager.Close(this);
        }

        public override void OnOpen()
        {
            //throw new NotImplementedException();
        }

        public override void OnClose()
        {
            //throw new NotImplementedException();
        }
    }
}

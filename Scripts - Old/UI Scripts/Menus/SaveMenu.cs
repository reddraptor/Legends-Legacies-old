using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.UI_Scripts.Menus

{
    public class SaveMenu : Menu
    {

        public InputField saveNameField;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {

        }

        public void OnSave()
        {
            WorldSave.Save(saveNameField.text, menuController.gameController.gameWorld);
            Close();
        }
    }

}
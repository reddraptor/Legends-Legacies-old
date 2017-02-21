using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.UI_Scripts.Menus
{
    public class MainMenu : Menu
    {

        public GameObject saveButtonObject;

        //public GameObject savePanel;
        //public GameObject loadPanel;
        //public GameObject createPanel;

        Button saveButton;

        //SaveMenu saveMenu;
        //LoadMenu loadMenu;
        //CreateMenu createMenu;

        public bool saveEnabled
        {
            get { return saveButton.interactable; }
            set { saveButton.interactable = value; }
        }

        protected override void Awake()
        {
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            saveButton = saveButtonObject.GetComponent<Button>();
            //saveMenu = savePanel.GetComponent<SaveMenu>();
            //loadMenu = loadPanel.GetComponent<LoadMenu>();
            //createMenu = createPanel.GetComponent<CreateMenu>();
        }

        // Update is called once per frame
        protected override void Update()
        {

        }

        public void OnQuit()
        {
            Debug.Log("Quitting.");
            Application.Quit();
            //menuController.Close(tag);

        }

        public void OnNew()
        {
            Open("Create Menu", this);
            Close();
        }

        public void OnSave()
        {
            Open("Save Menu", this);
            Close();

        }

        public void OnLoad()
        {
            Open("Load Menu", this);
            Close();

        }
        
    }

}
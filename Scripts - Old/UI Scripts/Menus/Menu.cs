using UnityEngine;
using System.Collections;

namespace Assets.Scripts.UI_Scripts.Menus
{
    public class Menu : MonoBehaviour
    {
        MenuController menuCtrler;
        Canvas canvas;

        string retTag;

        public MenuController menuController
        {
            get { return menuCtrler; }
            set
            {
                if (menuCtrler == null)
                {
                    menuCtrler = value;
                }
                else
                {
                    Debug.Log("Set menuController failed: menuController already set. " + this.ToString());
                    return;
                }
            }
        }

        public string returnTag
        {
            get { return retTag; }
            set { retTag = value; }
        }

        protected virtual void Awake() { }

        protected virtual void Start()
        {
            canvas = gameObject.GetComponent<Canvas>();
        }

        protected virtual void Update() { }


        protected virtual void Open(string menuObjectTag)
        {
            menuCtrler.GetMenu(menuObjectTag).returnTag = this.gameObject.tag;
            menuCtrler.Open(menuObjectTag);
        }

        protected virtual void Close()
        {
            menuCtrler.Close(tag);
        }


        public virtual void Return()
        {
            if (retTag != null) menuCtrler.Open(retTag);
            menuCtrler.Close(this.gameObject.tag);
        }

        public bool open
        {
            get { return canvas.enabled; }
            set
            {
                OnOpened();
                canvas.enabled = value;
            }
        }

        protected virtual void OnOpened() { }
    }

}

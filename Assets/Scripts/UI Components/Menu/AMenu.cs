using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.UI_Components.Menu
{
    [RequireComponent(typeof(Canvas))]
    public abstract class AMenu : MonoBehaviour
    {
        [HideInInspector]
        public MenuManager menuManager;
        [HideInInspector]
        public AMenu callingMenu = null;

        /* PUBLIC PROPERTIES */
        public bool open
        {
            get { return canvas.enabled; }
            set
            {
                canvas.enabled = value;
            }
        }

        Canvas canvas;

        protected virtual void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        public abstract void OnOpen();
        public abstract void OnClose();
    }
}

using UnityEngine.UI;
using Assets.Scripts.Managers;

namespace Assets.Scripts.UI_Components.Menu
{
    class Menu_Save : AMenu
    {
        public InputField saveNameField;

        PersistanceManager persistanceManager;

        public override void OnClose()
        {

        }
        
        public override void OnOpen()
        {
            persistanceManager = menuManager.GetComponent<PersistanceManager>();
        }

        public void OnSave()
        {
            persistanceManager.Save(saveNameField.text);
            menuManager.Close(this);
        }
    }
}

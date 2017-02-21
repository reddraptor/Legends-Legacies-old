using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.UI_Components
{
    public class SelectableList : MonoBehaviour
    {
        public GameObject listItemPrefab;

        List<GameObject> items;
        Button selected;

        // Use this for initialization
        void Start()
        {
            items = new List<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Add(string buttonText)
        {
            GameObject item = Instantiate(listItemPrefab, transform, false) as GameObject;
            item.GetComponentInChildren<Text>().text = buttonText;
            Button button = item.GetComponent<Button>();
            button.onClick.AddListener(delegate { OnNewSelected(button); });
            if (items.Count == 0)
            {
                button.interactable = false;
                selected = button;
            }
            items.Add(item);
        }

        public void Clear()
        {
            while (items.Count > 0)
            {
                Destroy(items[items.Count - 1]);
                items.Remove(items[items.Count - 1]);
            }
        }

        public void OnNewSelected(Button button)
        {
            button.interactable = false;
            selected.interactable = true;
            selected = button;
        }

        public string GetSelectedText()
        {
            if (selected)
                return selected.GetComponentInChildren<Text>().text;
            else return "";
        }

        public void RemoveSelected()
        {
            if (!selected) return;
            items.Remove(selected.gameObject);
            selected.gameObject.SetActive(false);
            Destroy(selected);
            if (items.Count > 0)
            {
                selected = items[0].GetComponent<Button>();
                selected.interactable = false;
            }
        }
    }

}
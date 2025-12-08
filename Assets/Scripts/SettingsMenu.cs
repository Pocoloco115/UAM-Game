using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    public List<KeyBindItem> menuItems;
    private int currentIndex = 0;

    private void OnEnable()
    {
        currentIndex = 0;
        foreach (var item in menuItems)
        {
            item.Initialize();  
        }
        UpdateSelection();
    }

    private void Update()
    {
        if (!menuItems[currentIndex].IsWaitingForKey())
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex = (currentIndex - 1 + menuItems.Count) % menuItems.Count;
                UpdateSelection();
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex = (currentIndex + 1) % menuItems.Count;
                UpdateSelection();
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                menuItems[currentIndex].StartRebind();
            }
        }
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].SetArrowActive(i == currentIndex);
        }
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuItems[currentIndex].gameObject);
    }
}







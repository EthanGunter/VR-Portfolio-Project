using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// A collection of button names and actions that should be performed when they are pressed
/// </summary>
[CreateAssetMenu(menuName = "Shared Data/Context Menu")]
public class ContextMenuData : ScriptableObject
{
    public event Action<string, Action, int> OnItemAdded;
    public event Action<string> OnItemRemoved;

    public string[] Items => _items.Select(x => x.displayText).ToArray();
    public int Count => _items.Count;

    private readonly List<MenuAction> _items = new();

    public void AddItem(string displayText, Action action, int index = -1)
    {
        if (index < 0)
        {
            _items.Add(new MenuAction { displayText = displayText, action = action });
        }
        else
        {

            Debug.Log($"Inserting menu item '{displayText}' at {index}", this);
            _items.Insert(index, new MenuAction { displayText = displayText, action = action });
        }
        OnItemAdded.Invoke(displayText, action, index > 0 ? index : _items.Count - 1);
    }
    public void RemoveItem(string displayText)
    {
        _items.RemoveAt(_items.FindIndex(x => x.displayText == displayText));
        OnItemRemoved.Invoke(displayText);
    }


    public class MenuAction
    {
        public string displayText;
        public Action action;
    }
}

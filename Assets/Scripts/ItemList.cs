using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "CreateData/ItemData", order = 0)]
public class ItemList:ScriptableObject
{
    public List<Item> items = new List<Item>();
}

[Serializable]
public class Item
{
    public int itemId;
    public string itemName;
    public ItemType type;
    public Sprite itemIcon;
    public int width;
    public int height;
}

[Serializable]
public enum ItemType
{
    Consume,
    Weapon,
    Shoe,
    Head
}
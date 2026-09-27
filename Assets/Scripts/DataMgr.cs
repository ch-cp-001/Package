using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataMgr
{
    private static DataMgr instance = new DataMgr();
    public static DataMgr Instance {  get { return instance; } }

    public ItemList items;

    public DataMgr()
    {
        items = Resources.Load<ItemList>("Data/ItemData");
    }

    // 外部获取Item
    public Item GetItem(int itemId)
    {
        return items.items[itemId];
    }
}

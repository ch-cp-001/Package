using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackagePanel : MonoBehaviour
{
    private static PackagePanel instance;
    public static PackagePanel Instance => instance;

    public Transform gridTran;
    public Transform itemTran;


    // 记录背包每个格子的加载状态（true表示已加载）
    private bool[] showState = new bool[32];
    // 记录所有加载的背包物品图片对象
    private Dictionary<int,PackageItemObj> packageItemDic = new Dictionary<int,PackageItemObj>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        LoadPackageGrids();
    }

    // 加载背包中的格子
    public void LoadPackageGrids()
    {
        for(int i = 0; i < 4; i++)
        {
            for(int  j = 0; j < 8; j++)
            {
                GameObject grid = Instantiate(Resources.Load<GameObject>("Model/Grid"));
                grid.transform.SetParent(gridTran, false);
                (grid.transform as RectTransform).anchoredPosition = new Vector2(10+51*j,-10-51*i);
                grid.GetComponent<PackageGridObj>().gridIndex = i*8+j;
            }
        }
    }

    // 加载显示当前背包中的所有物品
    public void ShowPackageItem()
    {
        Dictionary<int, PackageItem> itemDic = PackageDatagr.Instance.itemDic;
        foreach(int gridIndex in itemDic.Keys)
        {
            if (showState[gridIndex])
            {
                continue;
            }
            else
            {
                ShowPackageItem(itemDic[gridIndex]);
            }
        }
    }
    
    // 显示某个格子索引位置的物品
    public void ShowPackageItem(PackageItem packageItem)
    {
        Item item = DataMgr.Instance.GetItem(packageItem.itemId);
        GameObject obj = Instantiate(Resources.Load<GameObject>("Model/Item"));
        obj.transform.SetParent(itemTran, false);
        (obj.transform as RectTransform).anchoredPosition = new Vector2(11+(packageItem.gridIndex % 8)*51,-11-(packageItem.gridIndex / 8)*51);
        (obj.transform as RectTransform).sizeDelta = new Vector2(51* packageItem.width-3,51* packageItem.height-3);
        obj.GetComponent<Image>().sprite = item.itemIcon;
        obj.GetComponent<PackageItemObj>().SetItemId(packageItem.itemId);

        for(int i=0;i<packageItem.height; i++)
        {
            for(int j=0;j<packageItem.width; j++)
            {
                showState[packageItem.gridIndex + i * 8 + j] = true;
            }
        }
    }

    // 移动已创建的图片位置(移动前索引，新位置信息)
    public void MovePackageItem(int preIndex,PackageItem newItem,bool isRotate) 
    {
        if (packageItemDic.ContainsKey(preIndex))
        {
            PackageItemObj item = packageItemDic[preIndex];
            RectTransform rect = item.transform as RectTransform;
            rect.anchoredPosition = new Vector2(11 + (newItem.gridIndex % 8) * 51, -11 - (newItem.gridIndex / 8) * 51);
            if (isRotate)
            {
                rect.sizeDelta = new Vector2(rect.sizeDelta.y, rect.sizeDelta.x);
            }
            packageItemDic.Add(newItem.gridIndex, item);
            packageItemDic.Remove(preIndex);
        }

        if (isRotate)
        {
            // 修改state状态
            for (int i = 0; i < newItem.width; i++)
            {
                for (int j = 0; j < newItem.height; j++)
                {
                    showState[preIndex + i * 8 + j] = false;
                }
            }           
        }
        else
        {
            // 修改state状态
            for (int i = 0; i < newItem.height; i++)
            {
                for (int j = 0; j < newItem.width; j++)
                {
                    showState[preIndex + i * 8 + j] = false;
                }
            }
        }
        // 修改state状态
        for (int i = 0; i < newItem.height; i++)
        {
            for (int j = 0; j < newItem.width; j++)
            {
                showState[preIndex + i * 8 + j] = true;
            }
        }

    }

    // 删除创建好的图片
    public void DeletePackageItem(PackageItem packageItem)
    {
        if (packageItemDic.ContainsKey(packageItem.gridIndex))
        {
            Destroy(packageItemDic[packageItem.gridIndex]);
            packageItemDic.Remove(packageItem.gridIndex);
        }

        // 修改state状态
        for (int i = 0; i < packageItem.height; i++)
        {
            for (int j = 0; j < packageItem.width; j++)
            {
                showState[packageItem.gridIndex + i * 8 + j] = false;
            }
        }
    }
}

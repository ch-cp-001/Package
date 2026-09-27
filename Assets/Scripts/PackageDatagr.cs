using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageDatagr
{
    private static PackageDatagr instance = new PackageDatagr();
    public static PackageDatagr Instance {  get { return instance; } }

    private int packageLength = 32;
    // 背包格子索引，物品信息
    public Dictionary<int,PackageItem> itemDic = new Dictionary<int,PackageItem>();


    // 获得物品
    public void GetItem(int itemId)
    {
        Item item = DataMgr.Instance.GetItem(itemId);
        for(int i = 0; i < packageLength; i++)
        {
            if (CheckGridUseful(i, item.width, item.height))
            {
                StoreItem(i,item,false);
                break;
            }else if(CheckGridUseful(i, item.height, item.width))
            {
                StoreItem(i, item, true);
                break;
            }
            else
            {
                Debug.Log("装不下了");
            }
        }
    }

    // 检测可存放（空位直接放）后,向指定背包位置存放物品
    private void StoreItem(int index, Item item,bool isRotate)
    {
        PackageItem pItem = new PackageItem();
        pItem.itemId = item.itemId;
        pItem.gridIndex = index;
        if (isRotate)
        {
            pItem.width = item.height;
            pItem.height = item.width;
        }
        else
        {
            pItem.width = item.width;
            pItem.height = item.height;
        }
            
        for (int i = 0; i < pItem.height; i++)
        {
            for (int j = 0; j < pItem.width; j++)
            {
                itemDic.Add(index + i * 8 + j,pItem);
            }
        }
        PackagePanel.Instance.ShowPackageItem(pItem);
    }

    /// <summary>
    /// 移动已有物品
    /// </summary>
    /// <param name="index">目标索引位置</param>
    /// <param name="item">原物品信息</param>
    /// <param name="isRotate">是否旋转</param>
    private void StoreItem(int index, PackageItem item,bool isRotate)
    {
        item.gridIndex = index;
        if (isRotate)
        {
            int tmp = item.width;
            item.width = item.height;
            item.height = tmp;
        }

        for (int i = 0; i < item.height; i++)
        {
            for (int j = 0; j < item.width; j++)
            {
                itemDic.Add(index + i * 8 + j, item);
            }
        }
        
    }


    // 移动背包物品，原位置有物品，需要将原本位置的物品放到拖拽物品的位置或者重新插入
    private void MoveItem(int preIndex,int newIndex)
    {
        // 获取原位置物品信息
        PackageItem preItem = itemDic[preIndex];
        RemoveItem(preItem);
        preItem.gridIndex = newIndex;
        // 记录物品移动后受到影响的物品信息
        List<PackageItem> newItemList = new List<PackageItem>();
        // 根据原物品形状取出新位置会替代的物品
        for(int i = 0; i < preItem.height; i++)
        {
            for(int j = 0 ; j< preItem.width; j++)
            {
                int index = newIndex + i * 8 + j;
                if (itemDic.ContainsKey(newIndex + i * 8 + j))
                {
                    newItemList.Add(itemDic[index]);
                    RemoveItem(itemDic[index]);
                    

                }
                // 将原物品的位置放到目标位置
                itemDic[index] = preItem;
                
            }
        }
        // 移动原物品位置
        PackagePanel.Instance.MovePackageItem(preIndex, preItem, false);
        // 重新布局newItemList中的物品
        for (int i=0;i< newItemList.Count; i++)
        {
            int index = newItemList[i].gridIndex - (newIndex - preIndex);
            int preItemIndex = newItemList[i].gridIndex;
            if (CheckGridUseful(index, newItemList[i].width, newItemList[i].height))
            {
                StoreItem(index, newItemList[i], false);
                PackagePanel.Instance.MovePackageItem(preItemIndex, newItemList[i], false);
            }
            else if(CheckGridUseful(index, newItemList[i].height, newItemList[i].width))
            {
                StoreItem(index, newItemList[i], true);
                PackagePanel.Instance.MovePackageItem(preItemIndex, newItemList[i], true);
            }
            else
            {
                PackagePanel.Instance.DeletePackageItem(newItemList[i]);
                GetItem(newItemList[i].itemId);
                PackagePanel.Instance.ShowPackageItem();
            }
        }
    }

    // 移除某个位置相关物品
    private void RemoveItem(PackageItem packageItem)
    {
        for(int i = 0; i < packageItem.height; i++)
        {
            for (int j = 0; j < packageItem.width; j++)
            {
                itemDic.Remove(packageItem.gridIndex+i*8+j);
            }
        }
    }

    // 给一个背包位置和物品长宽，判断是否可存放（该位置为空位）
    public bool CheckGridUseful(int index,int width,int height)
    {
        for(int i = 0; i < height; i++)
        {
            for(int j=0;j< width; j++)
            {
                int pos = index + i * 8 + j;
                if (itemDic.ContainsKey(pos))
                {
                    return false;
                }
            }
        }
        return true;
    }

    // 查询某个背包位置的物品信息
    public PackageItem QueryItem(int index)
    {
        return itemDic[index];
    }
}

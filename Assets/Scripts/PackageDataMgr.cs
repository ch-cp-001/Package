using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageDataMgr
{
    private static PackageDataMgr instance = new PackageDataMgr();
    public static PackageDataMgr Instance {  get { return instance; } }

    private int packageLength = 32;
    // 背包格子索引，物品信息
    public Dictionary<int,PackageItem> itemDic = new Dictionary<int,PackageItem>();

    public PackageItem GetPackageItem(int gridIndex)
    {
        if(itemDic.ContainsKey(gridIndex))
            return itemDic[gridIndex];
        return null;
    }

    // 获得物品
    public void ReceiveItem(int itemId)
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
                Debug.Log(i+"位置不合适装不下了");
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
    public void MoveItem(int preIndex,int newIndex)
    {
        bool isRotate = false;
        // 获取原位置物品信息
        PackageItem preItem = itemDic[preIndex];

        // 判断原本的放置方式是否会超出背包范围
        if (newIndex<0 || newIndex + (preItem.height-1)*8+preItem.width-1 >= packageLength || newIndex%8 + preItem.width-1>=8)
        {
            if(newIndex < 0 || newIndex + (preItem.width-1) * 8 + preItem.height-1 >= packageLength || newIndex % 8 + preItem.height - 1 >=8)
            {
                Debug.Log("移动失败");
                return;
            }
            else
            {
                isRotate = true;
            }
        }

        PackageItemObj prePackageItemObj = PackagePanel.Instance.ReturnPackageItem(preItem);
        RemoveItem(preItem);
        preItem.gridIndex = newIndex;
        if (isRotate)
        {
            int tmp = preItem.width;
            preItem.width = preItem.height;
            preItem.height = tmp;
            // 顺便修改图片尺寸
            RectTransform rect = prePackageItemObj.transform as RectTransform;
            rect.sizeDelta = new Vector2(rect.sizeDelta.y, rect.sizeDelta.x);
        }
        // 记录物品移动后受到影响的物品信息
        List<PackageItem> newItemList = new List<PackageItem>();
        // 根据原物品形状取出新位置会替代的物品
        for(int i = 0; i < preItem.height; i++)
        {
            for(int j = 0 ; j< preItem.width; j++)
            {
                int index = newIndex + i * 8 + j;
                if (itemDic.ContainsKey(index) && itemDic[index] != preItem)
                {
                    newItemList.Add(itemDic[index]);
                    RemoveItem(itemDic[index]);
                }
                // 将原物品的位置放到目标位置
                itemDic[index] = preItem;                
            }
        }
        // 移动原物品位置(先临时记录对象，后续再存储,否则下面移动的时候会导致索引重复情况报错)
        //PackagePanel.Instance.MovePackageItem(preIndex, preItem, false);
        // 重新布局newItemList中的物品
        for (int i=0;i< newItemList.Count; i++)
        {
            int index = newItemList[i].gridIndex - (newIndex - preIndex);
            Debug.Log("想要放到"+index);
            int preItemIndex = newItemList[i].gridIndex;
            if (CheckGridUseful(index, newItemList[i].width, newItemList[i].height))
            {
                Debug.Log("不旋转");
                StoreItem(index, newItemList[i], false);
                PackagePanel.Instance.MovePackageItem(preItemIndex, newItemList[i], false);
            }
            else if(CheckGridUseful(index, newItemList[i].height, newItemList[i].width))
            {
                Debug.Log("旋转");
                StoreItem(index, newItemList[i], true);
                PackagePanel.Instance.MovePackageItem(preItemIndex, newItemList[i], true);
            }
            else
            {
                Debug.Log("其他");
                PackagePanel.Instance.DeletePackageItem(newItemList[i]);
                ReceiveItem(newItemList[i].itemId);
                //PackagePanel.Instance.ShowPackageItem(itemDic[itemNewIndex]);
            }
        }
        newItemList.Clear();
        PackagePanel.Instance.MovePackageItem(prePackageItemObj,preItem);
    }

    // 移动背包物品，原位置有物品，需要将原本位置的物品放到拖拽物品的位置或者重新插入
    public void SupplyMoveItem(int itemId,int newIndex)
    {
        bool isRotate = false;
        // 获取原位置物品信息
        Item item = DataMgr.Instance.GetItem(itemId);
        
        // 判断原本的放置方式是否会超出背包范围
        if (newIndex < 0 || newIndex + (item.height - 1) * 8 + item.width - 1 >= packageLength || newIndex % 8 + item.width - 1 >= 8)
        {
            if (newIndex < 0 || newIndex + (item.width - 1) * 8 + item.height - 1 >= packageLength || newIndex % 8 + item.height - 1 >= 8)
            {
                Debug.Log("移动失败");
                return;
            }
            else
            {
                isRotate = true;
            }
        }
        if (isRotate)
        {
            int tmp = item.width;
            item.width = item.height;
            item.height = tmp;
        }
        PackageItem packageItem = new PackageItem();
        packageItem.itemId = itemId;
        packageItem.gridIndex = newIndex;
        packageItem.height = item.height;
        packageItem.width = item.width;

        // 记录物品移动后受到影响的物品信息
        List<PackageItem> newItemList = new List<PackageItem>();
        // 根据原物品形状取出新位置会替代的物品
        for (int i = 0; i < item.height; i++)
        {
            for (int j = 0; j < item.width; j++)
            {
                int index = newIndex + i * 8 + j;
                if (itemDic.ContainsKey(index))
                {
                    newItemList.Add(itemDic[index]);
                    RemoveItem(itemDic[index]);
                }
                // 将原物品的位置放到目标位置
                itemDic[index] = packageItem;
            }
        }
        
        // 重新布局newItemList中的物品
        for (int i = 0; i < newItemList.Count; i++)
        {
            PackagePanel.Instance.DeletePackageItem(newItemList[i]);
            ReceiveItem(newItemList[i].itemId);
        }
        newItemList.Clear();
        PackagePanel.Instance.ShowPackageItem(packageItem);
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
        // 计算出来的索引超过上限或者存在右侧跨行情况，说明存储后会导致物品超出背包容量
        if (index<0 || index%8+width-1>=8 || index+(height-1)*8+width-1>=packageLength)
            return false;

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

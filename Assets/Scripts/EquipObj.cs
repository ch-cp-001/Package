using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 装备对象
public class EquipObj : MonoBehaviour,IPointerClickHandler
{
    public ItemType ItemType;
    private Image icon;
    private int itemId;

    private void Awake()
    {
        icon = GetComponent<Image>();
    }

    public void Init(Item item)
    {
        this.itemId = item.itemId;
        icon.sprite = item.itemIcon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            PackageDataMgr.Instance.ReceiveItem(itemId);
        }
    }
}

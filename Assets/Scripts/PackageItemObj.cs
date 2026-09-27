using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PackageItemObj : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    private int itemId;
    private Image img;
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    public void SetItemId(int itemId)
    {
        this.itemId = itemId;
    }

    // 偏移量
    private Vector2 offset;
    // 
    private RectTransform canvasRect;

    private Camera canvasCamera;
    private GameObject dragObj;
    // 计算鼠标点击位置与左上角的偏移
    public void OnBeginDrag(PointerEventData eventData)
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        canvasCamera = canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, canvasCamera, out Vector2 mouseLocal);
        offset = (transform as RectTransform).anchoredPosition - mouseLocal;
        //Debug.Log((transform as RectTransform).anchoredPosition +"--"+mouseLocal + "--"+offset);

        // 创建复制体
        dragObj = CreateDragItem();
    }

    

    public void OnDrag(PointerEventData eventData)
    {
        // 自身变成半透明后创建一个复制体跟随鼠标移动
        Transparent(false);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, canvasCamera, out Vector2 mouseLocal);
        (dragObj.transform as RectTransform).anchoredPosition = mouseLocal+offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log((dragObj.transform as RectTransform).anchoredPosition.x+" , "+ (dragObj.transform as RectTransform).anchoredPosition.y);
        if(OverStep((dragObj.transform as RectTransform).anchoredPosition))
        {
            Debug.Log("越界");
            Destroy(dragObj);
            return;
        }
        // 利用坐标计算，找到目标格子后判断是否可以移动（有物体则发生交换，没有就直接移动）删除复制体
        int dragX = (int)((dragObj.transform as RectTransform).anchoredPosition.x - 10) / 51;
        int dragY = (int)(-(dragObj.transform as RectTransform).anchoredPosition.y - 10) /51;
        int dragIndex = dragY * 8 + dragX;
        Debug.Log(dragIndex);
        
        //判断当前物体是否可以放在新位置（直接放，如果原来有物体就尝试将原本的物体替换到他的位置，替换失败就向后重新插入背包）


    }

    // 判断拖拽后是否越界
    private bool OverStep(Vector2 pos)
    {
        if(pos.x<10 || pos.x > 418)
        {
            return true;
        }
        if(pos.y > -10 || pos.y < -214)
        {
            return true;
        }
        return false;
    }

    // 调整透明度方法(true表示改为不透明状态)
    private void Transparent(bool flag)
    {
        Color color = img.color;
        color = new Color(color.r,color.g,color.b,flag?1:0.5f); 
        img.color = color;
    }

    public GameObject CreateDragItem()
    {
        Item item = DataMgr.Instance.GetItem(itemId);
        GameObject obj = Instantiate(Resources.Load<GameObject>("Model/Item"));
        obj.transform.SetParent(transform.parent, false);
        (obj.transform as RectTransform).anchoredPosition = (transform as RectTransform).anchoredPosition;
        (obj.transform as RectTransform).sizeDelta = (transform as RectTransform).sizeDelta;
        obj.GetComponent<Image>().sprite = item.itemIcon;
        // 复制体设置到最上层，防止被其他物品覆盖
        obj.transform.SetAsLastSibling();
        // 关闭射线检测
        obj.GetComponent<Image>().raycastTarget = false;
        return obj;
    }
}

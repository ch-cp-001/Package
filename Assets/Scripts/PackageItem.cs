using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageItem
{
    public int itemId;
    // 记录该物品存放的左上角格子索引
    public int gridIndex;
    // 不保存占据的格子索引而是记录长宽状态
    public int width;
    public int height;
    
}

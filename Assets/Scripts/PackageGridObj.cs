using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PackageGridObj : MonoBehaviour
{
    // ¸ñ×ÓË÷ÒýÐòºÅ
    public int gridIndex;
    public TextMeshProUGUI texNum;

    private void Start()
    {
        texNum.text = gridIndex.ToString();
    }
}

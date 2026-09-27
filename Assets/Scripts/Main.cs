using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PackageDataMgr.Instance.ReceiveItem(0);
        PackageDataMgr.Instance.ReceiveItem(3);
    }

}

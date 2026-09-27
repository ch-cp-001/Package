using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PackageDatagr.Instance.GetItem(0);
        PackageDatagr.Instance.GetItem(3);
    }

}

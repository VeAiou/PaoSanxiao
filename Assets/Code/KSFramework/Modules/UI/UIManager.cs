using KEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI管理器类，常驻内存，对UI生命周期做管理
/// </summary>
public class UIManager
{
    private static UIManager instance = null;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UIManager();
            }
            return instance;
        }
    }

    public List<UIController> showUI = new List<UIController>();
    public List<UIController> hideUI = new List<UIController>();

    public void Init()
    {
    }

    public void OpenWindow(UIController ui)
    {
        Debug.Log("==========================================>UIManeger打开了一个窗口   "+ ui.UIName);
    }

    public void CloseWindow(UIController ui)
    {
        Debug.Log("==========================================>UIManeger关闭了一个窗口   " + ui.UIName);
    }

    public void CloseAllWindow(UIController ui)
    {

    }
}

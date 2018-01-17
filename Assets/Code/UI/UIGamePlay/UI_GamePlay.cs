using KEngine.UI;
using KSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GamePlay : CSUIController
{
    public static UI_GamePlay Instance;

    public override void OnAwake()
    {
        base.OnAwake();
    }

    public override void OnClose()
    {
        base.OnClose();

        LevelMapMgr.Instance.ClearMap();
        GamePlayMgr.Instance.OutGameLevel();
    }

    public override void OnInit()
    {
        base.OnInit();
        Instance = this;

        for (int i = 0; i < UIOutLet.OutletInfos.Count; i++)
        {
            switch (UIOutLet.OutletInfos[i].Name)
            {
                case "BackButton":
                    EventTriggerListener.Get(UIOutLet.OutletInfos[i].Object).onClick = OnClickClose;
                    break;
            }
        }
    }

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);
    }

    public void OnClickClose(GameObject go)
    {
        UIModule.Instance.OpenWindow<UI_LevelWnd>(WindowID.LevelWnd, WindowID.LevelWnd, null, (ui, agr) =>
           {
               CloseWindow();
           });
    }

    /// <summary>
    /// 加载关卡数据
    /// </summary>
    public void InitLevel()
    {
        TempMapData data = new TempMapData();
        LevelMapMgr.Instance.InitMap(data,()=>
        {
            Debug.Log("关卡数据加载成功");
        });
    }

    public void SetInitOk()
    {
        GamePlayMgr.Instance.SetLoadUIOk();
    }
}

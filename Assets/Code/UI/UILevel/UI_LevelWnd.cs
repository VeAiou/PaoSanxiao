using System.Collections;
using UnityEngine;
using KSFramework;
using System;
using KEngine.UI;

public class UI_LevelWnd : CSUIController
{
    public static UI_LevelWnd Instance;

    public override void OnAwake()
    {
        base.OnAwake();
       
    }

    public override void BeforeOpen(object[] onOpenArgs, Action doOpen)
    {
        base.BeforeOpen(onOpenArgs, doOpen);

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
                    EventTriggerListener.Get(UIOutLet.OutletInfos[i].Object).onClick = OnClickBackButton;
                    break;
            }
        }
    }

    public void OnClickBackButton(GameObject go)
    {
        UIModule.Instance.OpenWindow<UI_LoginWnd>(WindowID.LoginWnd, WindowID.LoginWnd,null, (ui, arg) =>
        {
            this.CloseWindow();
        }, null);
    }

    public override void OnOpen(params object[] args)
    {
        base.OnOpen();
    }

    public override void OnClose()
    {
        base.OnClose();
    }

    public void OnGUI()
    {

    }

}

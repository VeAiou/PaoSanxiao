using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSFramework;
using System;
using KEngine.UI;
using KEngine;
using DG.Tweening;

public class UI_LoginWnd : CSUIController
{
    public static UI_LoginWnd Instance;

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
        //UILogic<UI_LoginWnd>.Instance.UIReg(this);
        for (int i = 0; i < UIOutLet.OutletInfos.Count; i++)
        {
            switch (UIOutLet.OutletInfos[i].Name)
            {
                case "LoginButton":
                    EventTriggerListener.Get(UIOutLet.OutletInfos[i].Object).onClick = OnClickLoginButton;
                    break;
                case "MessagesButton":
                    EventTriggerListener.Get(UIOutLet.OutletInfos[i].Object).onClick = OnClickMessagesButton;
                    break;
                case "PlayButton":
                    EventTriggerListener.Get(UIOutLet.OutletInfos[i].Object).onClick = OnClickPlayButton;
                    break;
            }
        }
    }

    public void OnClickLoginButton(GameObject go)
    {
        Debug.Log(go.name);
    }

    public void OnClickMessagesButton(GameObject go)
    {
        Debug.Log(go.name);
    }

    public void OnClickPlayButton(GameObject go)
    {
        Debug.Log(go.name);
        UIModule.Instance.OpenWindow<UI_LevelWnd>(WindowID.LevelWnd, WindowID.LevelWnd, null, (ui, arg) =>
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
        //UILogic<UI_LoginWnd>.Instance.UIUnReg();
    }

    public void OnGUI()
    {
       
    }
}

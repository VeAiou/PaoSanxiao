using KEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KSFramework
{
    public class CSUIController : UIController
    {
        public UILuaOutlet UIOutLet;

        public override void OnAwake()
        {
            base.OnAwake();
            
        }

        public override void OnOpen(params object[] args)
        {
            base.OnOpen(args);
            UIManager.Instance.OpenWindow(this);
        }

        public override void BeforeOpen(object[] onOpenArgs, Action doOpen)
        {
            base.BeforeOpen(onOpenArgs, doOpen);
        }

        public override void OnClose()
        {
            base.OnClose();
            UIManager.Instance.CloseWindow(this);
        }

        public override void OnInit()
        {
            base.OnInit();
            UIOutLet = GetComponent<UILuaOutlet>();
        }
    }
}
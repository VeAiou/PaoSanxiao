using KEngine;
using KEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KEngine
{
    public class UILoadRequest
    {
        public UnityEngine.Object Asset;
    }

    /// <summary>
    /// UI Async Load State class
    /// </summary>
    public class UILoadState
    {
        public string TemplateName;
        public string InstanceName;
        private UIController uIWindow;
        private UIController parentWindow;
        public Type UIType;
        public bool IsLoading;
        public bool IsStaticUI; // 非复制出来的, 静态UI

        public bool OpenWhenFinish;
        public object[] OpenArgs;

        internal Queue<Action<UIController, object[]>> CallbacksWhenFinish;
        internal Queue<object[]> CallbacksArgsWhenFinish;
        public AbstractResourceLoader UIResourceLoader; // 加载器，用于手动释放资源

        public UIController ParentWindow
        {
            get
            {
                return parentWindow;
            }
            set
            {
                parentWindow = value;
            }
        }

        public UIController UIWindow
        {
            get
            {
                return uIWindow;
            }

            set
            {
                uIWindow = value;
                if(ParentWindow != null)//设置窗口之前必须要保证设置好了父窗口
                {
                    uIWindow.ParentWondow = parentWindow;
                    parentWindow.AddChildList(uIWindow);
                }
            }
        }

        public UILoadState(string uiTemplateName, string uiInstanceName, Type uiControllerType = default(Type), UIController parent = null)
        {
            if (uiControllerType == default(Type)) uiControllerType = typeof(UIController);

            TemplateName = uiTemplateName;
            InstanceName = uiInstanceName;
            UIType = uiControllerType;
            parentWindow = parent;

            IsLoading = true;
            OpenWhenFinish = false;
            OpenArgs = null;

            CallbacksWhenFinish = new Queue<Action<UIController, object[]>>();
            CallbacksArgsWhenFinish = new Queue<object[]>();
        }

        public void Init()
        {
            
        }

        public void Close()
        {
            if(!IsLoading)
            {
                UIModule.Instance.CloseWindow(TemplateName);
            }else
            {
                Debuger.Assert("正在加载资源，不能关闭窗口");
            }
        }

        /// <summary>
        /// 确保加载完成后的回调
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="args"></param>
        public void DoCallback(Action<UIController, object[]> callback, object[] args = null)
        {
            if (args == null)
                args = new object[0];

            if (IsLoading) // Loading
            {
                CallbacksWhenFinish.Enqueue(callback);
                CallbacksArgsWhenFinish.Enqueue(args);
                return;
            }

            // 立即执行即可
            callback(UIWindow, args);
        }

        internal void OnUIWindowLoadedCallbacks(UILoadState uiState, UIController uiObject)
        {
            //if (openState.OpenWhenFinish)  // 加载完打开 模式下，打开时执行回调
            {
                while (uiState.CallbacksWhenFinish.Count > 0)
                {
                    Action<UIController, object[]> callback = uiState.CallbacksWhenFinish.Dequeue();
                    object[] _args = uiState.CallbacksArgsWhenFinish.Dequeue();
                    //callback(uiBase, _args);

                    DoCallback(callback, _args);
                }
            }
        }
    }
}

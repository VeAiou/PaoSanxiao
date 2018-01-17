#region Copyright (c) 2015 KEngine / Kelly <http://github.com/mr-kelly>, All rights reserved.

// KEngine - Toolset and framework for Unity3D
// ===================================
// 
// Filename: KUIModule.cs
// Date:     2015/12/03
// Author:  Kelly
// Email: 23110388@qq.com
// Github: https://github.com/mr-kelly/KEngine
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using KEngine;
using UnityEngine;

namespace KEngine.UI
{
    /// <summary>
    /// UI Module
    /// </summary>
    public class UIModule
    {
        private class _InstanceClass
        {
            public static UIModule _Instance = new UIModule();
        }

        public static UIModule Instance
        {
            get { return _InstanceClass._Instance; }
        }

        /// <summary>
        /// 正在加载的UI统计
        /// </summary>
        private int _loadingUICount = 0;

        public int LoadingUICount
        {
            get { return _loadingUICount; }
            set
            {
                _loadingUICount = value;
                if (_loadingUICount < 0) Log.Error("Error ---- LoadingUICount < 0");
            }
        }


        /// <summary>
        /// A bridge for different UI System, for instance, you can use NGUI or EZGUI or etc.. UI Plugin through UIBridge
        /// </summary>
        public IUIBridge UiBridge;

        /// <summary>
        /// 缓存所有加载的窗口资源
        /// </summary>
        public Dictionary<string, UILoadState> UIWindows = new Dictionary<string,UILoadState>();

        public static Action<UIController> OnInitEvent; 
        public static Action<UIController> OnOpenEvent;
        public static Action<UIController> OnCloseEvent;

        public static GameObject UIParent;
        public static GameObject UICamera;
        public static GameObject UIRoot;

        public UIModule()
        {
            var configUiBridge = AppEngine.GetConfig("KEngine.UI", "UIModuleBridge");

            if (!string.IsNullOrEmpty(configUiBridge))
            {
                var uiBridgeTypeName = string.Format("{0}", configUiBridge);
                var uiBridgeType = KTool.FindType(uiBridgeTypeName);
                if (uiBridgeType != null)
                {
                    UiBridge = Activator.CreateInstance(uiBridgeType) as IUIBridge;
                    Log.Logs("=======================Use UI Bridge: {0}{1}", uiBridgeType, UiBridge.ToString());
                }
                else
                {
                    Log.Error("Cannot find UIBridge Type: {0}", uiBridgeTypeName);
                }
            }

            if (UiBridge == null)
            {
                UiBridge = new UGUIBridge();
            }

            UiBridge.InitBridge();
            UIParent = GameObject.FindGameObjectWithTag("Canvas");
            UICamera = GameObject.FindGameObjectWithTag("UICamera");
            UIRoot = GameObject.FindGameObjectWithTag("UIRoot");
        }

        // 打开窗口（非复制）
        public UILoadState OpenWindow(string uiTemplateName,UIController parent = null, params object[] args)
        {
            UILoadState uiState;
            List<UILoadState> list;
            if (!UIWindows.TryGetValue(uiTemplateName, out list) || list.Count == 0)//没有窗口加载体就创建窗口
            {
                uiState = LoadWindow(uiTemplateName, true, parent, args);
                return uiState;
            }
            if(list[0].UIWindow.IsSingle)//如果正在打开了，并且是单利窗口，先关闭窗口
            {
                CloseWindow(list[0].TemplateName);
                uiState = LoadWindow(uiTemplateName, true, parent, args);
                return uiState;
            }
            else//不是单利窗口创建
            {
                uiState = new UILoadState(uiTemplateName, string.Format("windowTemplateName{0}",list.Count),null, parent);
            }

            OnOpen(uiState, args);
            return uiState;
        }

        public void CloseWindow(string name)
        {
            UILoadState uiState;
            if (!UIWindows.TryGetValue(name, out uiState))
            {
                if (Debug.isDebugBuild)
                    Log.Warning("[CloseWindow]没有加载的UIWindow: {0}", name);
                return; // 未开始Load
            }

            if (uiState.IsLoading) // Loading中
            {
                if (Debug.isDebugBuild)
                    Log.Info("[CloseWindow]IsLoading的{0}", name);
                uiState.OpenWhenFinish = false;
                return;
            }

            //Action doCloseAction = () =>
            //{
            uiState.UIWindow.gameObject.SetActive(false);
            uiState.UIWindow.OnClose();
            if (OnCloseEvent != null)
                OnCloseEvent(uiState.UIWindow);
            if (!uiState.IsStaticUI)
            {
                DestroyWindow(name);
            }
            //};

            //doCloseAction();
        }

        /// <summary>
        /// Destroy all windows that has LoadState.
        /// Be careful to use.
        /// </summary>
        public void DestroyAllWindows()
        {
            List<string> LoadList = new List<string>();

            foreach (KeyValuePair<string, UILoadState> uiWindow in UIWindows)
            {
                if (IsLoad(uiWindow.Key))
                {
                    LoadList.Add(uiWindow.Key);
                }
            }

            foreach (string item in LoadList)
                DestroyWindow(item);
        }

        [Obsolete("Deprecated: Please don't use this")]
        public void CloseAllWindows()
        {
            List<string> toCloses = new List<string>();

            foreach (KeyValuePair<string, UILoadState> uiWindow in UIWindows)
            {
                if (IsOpen(uiWindow.Key))
                {
                    toCloses.Add(uiWindow.Key);
                }
            }

            for (int i = toCloses.Count - 1; i >= 0; i--)
            {
                CloseWindow(toCloses[i]);
            }
        }

        private UILoadState _GetUIState(string name)
        {
            UILoadState uiState;
            UIWindows.TryGetValue(name, out uiState);
            if (uiState != null)
                return uiState;

            return null;
        }

        private UIController GetUIBase(string name)
        {
            UILoadState uiState;
            UIWindows.TryGetValue(name, out uiState);
            if (uiState != null && uiState.UIWindow != null)
                return uiState.UIWindow;

            return null;
        }

        public bool IsOpen(string name)
        {
            UIController uiBase = GetUIBase(name);
            return uiBase == null ? false : uiBase.gameObject.activeSelf;
        }

        public bool IsLoad(string name)
        {
            if (UIWindows.ContainsKey(name))
                return true;
            return false;
        }

        public UILoadState LoadWindow(string windowTemplateName, bool openWhenFinish, UIController parentWindow,params object[] args)
        {
            if (UIWindows.ContainsKey(windowTemplateName))
            {
                Log.Error("[LoadWindow]多次重复LoadWindow: {0}", windowTemplateName);
            }
            Debuger.Assert(!UIWindows.ContainsKey(windowTemplateName));

            UILoadState openState = new UILoadState(windowTemplateName, windowTemplateName, null,parentWindow);
            openState.IsStaticUI = true;
            openState.OpenArgs = args;

            //if (openState.IsLoading)
            openState.OpenWhenFinish = openWhenFinish;

            UIWindows.Add(windowTemplateName, openState);

            KResourceModule.Instance.StartCoroutine(LoadUIAssetBundle(windowTemplateName, openState));

            return openState;
        }

        private IEnumerator LoadUIAssetBundle(string name, UILoadState openState, KCallback callback = null)
        {
            if (openState.UIResourceLoader != null)
            {
                openState.UIResourceLoader.Release(true);// now!
                Log.Info("Release UI ResourceLoader: {0}", openState.UIResourceLoader.Url);
                openState.UIResourceLoader = null;
            }

            LoadingUICount++;

            var request = new UILoadRequest();
            yield return KResourceModule.Instance.StartCoroutine(UiBridge.LoadUIAsset(openState, request));

            GameObject uiObj = (GameObject)request.Asset;
            // 具体加载逻辑结束...这段应该放到Bridge里

            uiObj.SetActive(false);
            uiObj.name = openState.TemplateName;

            var uiBase = UiBridge.CreateUIController(uiObj, openState.TemplateName);

            if (openState.UIWindow != null)
            {
                Log.Info("Destroy exist UI Window, maybe for reload");
                GameObject.Destroy(openState.UIWindow.CachedGameObject);
                openState.UIWindow = null;
            }

            uiBase.CachedTransform.SetParent(UIParent.transform, false);
            openState.UIWindow = uiBase;

            uiBase.UIName = uiBase.UITemplateName = openState.TemplateName;

            UiBridge.UIObjectFilter(uiBase, uiObj);

            openState.IsLoading = false; // Load完
            InitWindow(openState, uiBase, openState.OpenWhenFinish, openState.OpenArgs);

            LoadingUICount--;

            if (callback != null)
                callback(null);
        }

        /// <summary>
        /// Hot reload a ui asset bundle
        /// </summary>
        /// <param name="uiTemplateName"></param>
        public UnityEngine.Coroutine ReloadWindow(string windowTemplateName, KCallback callback)
        {
            UILoadState uiState;
            UIWindows.TryGetValue(windowTemplateName, out uiState);
            if (uiState == null || uiState.UIWindow == null)
            {
                Log.Info("{0} has been destroyed", windowTemplateName);
                return null;
            }
            return KResourceModule.Instance.StartCoroutine(LoadUIAssetBundle(windowTemplateName, uiState));
        }

        public void DestroyWindow(string uiTemplateName)
        {
            UILoadState uiState;
            UIWindows.TryGetValue(uiTemplateName, out uiState);
            if (uiState == null || uiState.UIWindow == null)
            {
                Log.Info("{0} has been destroyed", uiTemplateName);
                return;
            }

            UnityEngine.Object.Destroy(uiState.UIWindow.gameObject);

            // Instance UI State has no Resources loader, so fix here
            if (uiState.UIResourceLoader != null)
                uiState.UIResourceLoader.Release();
            uiState.UIWindow = null;

            UIWindows.Remove(uiTemplateName);
        }

        /// <summary>
        /// 等待并获取UI实例，执行callback
        /// 源起Loadindg UI， 在加载过程中，进度条设置方法会失效
        /// 如果是DynamicWindow,，使用前务必先要Open!
        /// </summary>
        /// <param name="uiTemplateName"></param>
        /// <param name="callback"></param>
        /// <param name="args"></param>
        public void CallUI(string uiTemplateName, Action<UIController, object[]> callback, params object[] args)
        {
            Debuger.Assert(callback);

            UILoadState uiState;
            if (!UIWindows.TryGetValue(uiTemplateName, out uiState))
            {
                uiState = LoadWindow(uiTemplateName, false,uiState.ParentWindow); // 加载，这样就有UIState了, 但注意因为没参数，不要随意执行OnOpen
            }

            uiState.DoCallback(callback, args);
        }

        private void OnOpen(UILoadState uiState, params object[] args)
        {
            if (uiState.IsLoading)
            {
                uiState.OpenWhenFinish = true;
                uiState.OpenArgs = args;
                return;
            }

            UIController uiBase = uiState.UIWindow;
            uiBase.ParentWondow = uiState.ParentWindow;
            if (uiState.ParentWindow != null)//如果有父窗口添加到父窗口下面
            {
                uiBase.CachedTransform.SetParent(uiBase.ParentWondow.transform, false);
            }
            else
            {
                uiBase.CachedTransform.SetParent(UIParent.transform, false);
            }

            //Action doOpenAction = () =>
            {
                if (uiBase.gameObject.activeSelf)
                {
                    uiBase.OnClose();

                    if (OnCloseEvent != null)
                        OnCloseEvent(uiBase);
                }

                uiBase.BeforeOpen(args, () =>
                {
                    uiBase.gameObject.SetActive(true);

                    uiBase.OnOpen(args);

                    if (OnOpenEvent != null)
                        OnOpenEvent(uiBase);
                });
            };

            //            doOpenAction();
        }

        private void InitWindow(UILoadState uiState, UIController uiBase, bool open, params object[] args)
        {
            uiBase.OnInit();
            if (OnInitEvent != null)
                OnInitEvent(uiBase);
            if (open)
            {
                OnOpen(uiState, args);
            }

            if (!open)
            {
                if (!uiState.IsStaticUI)
                {
                    CloseWindow(uiBase.UIName); // Destroy
                    return;
                }
                else
                {
                    uiBase.gameObject.SetActive(false);
                }
            }

            uiState.OnUIWindowLoadedCallbacks(uiState, uiBase);
        }
    }
}
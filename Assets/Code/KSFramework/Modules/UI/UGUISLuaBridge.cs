using UnityEngine;
using System.Collections;
using KEngine;
using KEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace KSFramework
{
    public class UGUISLuaBridge : IUIBridge
    {
        public EventSystem mEventSystem;

        public virtual void InitBridge()
        {
            //            if (EventSystem.current == null)
            //            {
            //                mEventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
            //                mEventSystem.gameObject.AddComponent<StandaloneInputModule>();
            //#if !UNITY_5
            //                mEventSystem.gameObject.AddComponent<TouchInputModule>();
            //#else
            //                mEventSystem.gameObject.GetComponent<StandaloneInputModule>().forceModuleActive = true;
            //#endif
            //            }
            GameObject canvas = GameObject.FindGameObjectWithTag("UIRoot");
            if(canvas == null)
            {
                // 同步加载，返回加载器，加载器中有加载的资源
                StaticAssetLoader reqeust = StaticAssetLoader.Load("ui/canvas.prefab",null,LoaderMode.Sync);
                GameObject go = (GameObject)reqeust.ResultObject;
                go.name = "Canvas";
            }
        }

        public virtual UIController CreateUIController(Type tp,GameObject uiObj, string uiTemplateName)
        {
            //做修改，使c#的uicontroller也能使用，但是优先使用slua的
            UIController uiBase = uiObj.GetComponent<UIController>();
            if (uiBase == null)//默认使用C#
            {
                // Type tp = System.Type.GetType("UI_" + uiTemplateName + ",Assembly-CSharp");
                uiBase = uiObj.AddComponent(tp) as UIController;
                uiBase.IsUseCs = true;
            }else if(!uiBase.IsUseCs)
            {
                uiBase = uiObj.GetComponent<LuaUIController>();
            }

            if(uiBase.IsUseCs)
            {
                uiBase = uiObj.GetComponent<CSUIController>();
            }
            
            KEngine.Debuger.Assert(uiBase);
            return uiBase;
        }

        public virtual void UIObjectFilter(UIController controller, GameObject uiObject)
        {
        }

        public virtual IEnumerator LoadUIAsset(UILoadState loadState, UILoadRequest request)
        {
            string path = string.Format("ui/{0}.prefab", loadState.TemplateName);
            var assetLoader = StaticAssetLoader.Load(path);
            loadState.UIResourceLoader = assetLoader; // 基本不用手工释放的
            while (!assetLoader.IsCompleted)
                yield return null;

            request.Asset = assetLoader.TheAsset;
        }
    }

}

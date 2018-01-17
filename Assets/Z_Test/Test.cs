using KEngine.UI;
using KSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public GameObject go;
	// Use this for initialization
	void Start () {
        CSUIController csController = this.gameObject.GetComponent<CSUIController>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public string name = "";
    public void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 100), "OpenWindow打开黑板"))
        {
            UIModule.Instance.OpenWindow<UI_LoginWnd>(WindowID.LoginWnd, "Billboard",null,null);        }
        if (GUI.Button(new Rect(100, 200, 100, 100), "ToggleWindow打开黑板"))
        {
           // UIModule.Instance.ToggleWindow("Billboard", null,1, 2, 3, 4);        }
        if (GUI.Button(new Rect(100, 300, 100, 100), "OpenDynamic打开黑板"))
        {
           // UIModule.Instance.OpenWindow("Billboard", "Billboard1", UI_LoginWnd.Instance, 1, 2, 3, 4);        }
        if (GUI.Button(new Rect(100, 400, 100, 100), "关闭UI"))
        {
            UIModule.Instance.CloseWindow(WindowID.LoginWnd);        }
    }

    private IEnumerator LoadALLGameObject(string path)
    {
        WWW bundle = new WWW(path);

        yield return bundle;

        //通过Prefab的名称把他们都读取出来
        Object obj0 = bundle.assetBundle.LoadAsset("login");

        //加载到游戏中	
        yield return Instantiate(obj0);
        bundle.assetBundle.Unload(false);
    }
}

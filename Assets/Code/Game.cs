#region Copyright (c) 2015 KEngine / Kelly <http://github.com/mr-kelly>, All rights reserved.

// KEngine - Toolset and framework for Unity3D
// ===================================
// 
// Filename: AppEngineInspector.cs
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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AppSettings;
using KEngine;
using KEngine.UI;
using KSFramework;


public delegate void UpdatePerFrame(float deltaTime);

public delegate void LateUpdatePerFrame(float deltaTime);
public class Game : KSGame
{
    private static UpdatePerFrame mUpdate;
    private static LateUpdatePerFrame mLateUpdate;


    protected override void Awake()
    {
        base.Awake();
    }

    public static void RegisterUpdate(UpdatePerFrame update)
    {
        if (update != null)
        {
            mUpdate += update;
        }
    }

    public static void RemoveUpdate(UpdatePerFrame update)
    {

        if (update != null)
        {
            mUpdate -= update;
        }

    }

    public static void RegisteLateUpdate(LateUpdatePerFrame lateUpdate)
    {
        if (lateUpdate != null)
        {
            mLateUpdate += lateUpdate;
        }
    }

    public static void RemoveLateUpdate(LateUpdatePerFrame lateUpdate)
    {
        if (lateUpdate != null)
        {
            mLateUpdate -= lateUpdate;
        }
    }

    void Update()
    {
        if (mUpdate != null)
        {
            mUpdate(Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        if (mLateUpdate != null)
        {
            mLateUpdate(Time.deltaTime);
        }
    }

    /// <summary>
    /// Add Your Custom Initable(Coroutine) Modules Here...
    /// </summary>
    /// <returns></returns>
    protected override IList<IModuleInitable> CreateModules()
    {
        var modules = base.CreateModules();

        // TIP: Add Your Custom Module here
        //modules.Add(new Module());

        return modules;
    }

    /// <summary>
    /// Before Init Modules, coroutine
    /// </summary>
    /// <returns></returns>
    public override IEnumerator OnBeforeInit()
    {
        // Do Nothing
        yield break;
    }
    /// <summary>
    /// After Init Modules, coroutine
    /// </summary>
    /// <returns></returns>
    public override IEnumerator OnGameStart()
    {

        // Print AppConfigs
        Log.Info("======================================= Read Settings from C# =================================");
        for (int i = 0; i < SettingsManager.SettingsList.Length; i++)
        {
            SettingsManager.SettingsList[i].ReloadAll();
        }
        
        Debug.Log(ActorConfigSettings.Get(1).Des);
        
        yield return null;

        Log.Info("======================================= Open Window 'Login' =================================");
        //UIModule.Instance.OpenWindow(wndName);

        // Test Load a scene in asset bundle
        //SceneLoader.Load("Scene/TestScene/TestScene.unity");

        // 开始加载我们的公告界面！
        //UIModule.Instance.OpenWindow("Billboard");
        // 测试Collect函数，立即回收所有资源
        KResourceModule.Collect();

        StartMgr();

        Messenger.Broadcast(GameDefine.MessageId_Local.GameStart.ToString());
    }

    public void StartMgr()
    {
        UIManager.Instance.Init();
        gameObject.AddComponent<AudioController>();
        GamePlayMgr.Instance.Init();
        LevelMapMgr.Instance.Init();
    }

}

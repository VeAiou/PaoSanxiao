using System;
using System.Collections.Generic;
using UnityEngine;
using KEngine;

/// <summary>
/// 关卡地图管理
/// </summary>
public class LevelMapMgr : CSharpSingletion<LevelMapMgr>
{
    private Action InitFinish;
    private TempMapData mTempMapData;
    public float MoveSpeed = 500;

    public void Init()
    {

    }

    /// <summary>
    /// 初始化地图数据
    /// </summary>
    public void InitMap(TempMapData data, Action callBack)
    {
        InitFinish = callBack;
        mTempMapData = data;

        int length = data.data.MapData.Count;
        for (int i = 0; i < length; i++)
        {
            CreateMap(data.data.MapData[i]);
        }
        UI_GamePlay.Instance.SetInitOk();
    }

    /// <summary>
    /// 清理地图数据
    /// </summary>
    public void Clear()
    {
        InitFinish = null;
        mTempMapData = null;
        ClearMap();
    }

    List<GridCenter> mCenter = new List<GridCenter>();
    private void CreateMap(MapData data)
    {
        var reqeust = KResourceModule.LoadBundle(PrefabsPath.MapCenter);
        GameObject go = GameObject.Instantiate((GameObject)reqeust.ResultObject);
        go.name = data.Index.ToString();
        go.transform.SetParent(UI_GamePlay.Instance.CachedTransform,false);

        GridCenter grid = go.GetComponent<GridCenter>();
        grid.Init(data);

        mCenter.Add(grid);
    }

    public void DelectMap(int id)
    {
        for (int i = 0; i < mCenter.Count; i++)
        {
            if(mCenter[i].mData.Index == id)
            {
                mCenter[i].Clear();
                mCenter.Remove(mCenter[i]);
                break;
            }
        }
    }

    public void ClearMap()
    {
        for (int i = 0; i < mCenter.Count; i++)
        {
            mCenter[i].Clear();
        }
        mCenter.Clear();
    }
}
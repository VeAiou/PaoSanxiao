using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 格子类型
/// </summary>
public enum GridType
{
    Nomorl = 1,//普通格子
    Empty,//空格子，不显示背景，也不做任何处理
    Stick,//障碍物，显示图片
}

/// <summary>
/// 格子状态
/// </summary>
public enum GridState
{
    Nomorl = 1,//正常显示
    Lock,//被锁住不能移动
}

/// <summary>
/// 消除类型
/// </summary>
public enum WipeType
{
    Nomorl = 1,
    ColumnFour,//列4个
    Columnfive,//列5个
    RowFour,//行4个
    RowFive,//行5个
    L,//L 
    T,//T
}

/// <summary>
/// 地图数据
/// </summary>
[Serializable]
public class MapData
{
    /// <summary>
    /// 关卡的第几个地图
    /// </summary>
    public int Index;

    public Vector3 pos = new Vector3();
    /// <summary>
    /// 地图格子数据
    /// </summary>
    public Dictionary<int,List<GridData>> Grids = new Dictionary<int, List<GridData>>();
}

/// <summary>
/// 关卡地图数据
/// </summary>
public class LevelMapData
{
    public List<MapData> MapData = new List<MapData>();
}

public class GridData
{
    /// <summary>
    /// 格子坐标
    /// </summary>
    public int Index;
    /// <summary>
    /// 格子初始状态
    /// </summary>
    public int StartState;
    /// <summary>
    /// 格子初始类型
    /// </summary>
    public int StartType;
    /// <summary>
    /// 格子图片类型
    /// </summary>
    public int SpriteType;
}


public class TempMapData
{
    public LevelMapData data = new LevelMapData();

    MapData mapData = new MapData();
    public TempMapData()
    {

        data.MapData.Clear();
        mapData.Index = 0;
        mapData.pos = new Vector3(-400, -450, 0);
        data.MapData.Add(mapData);

        int r = 10;
        int c = 10;
        for (int i = 0; i < r; i++)
        {
            List<GridData> list = new List<GridData>();
            for (int j = 0; j < c; j++)
            {
                GridData t = new GridData();
                t.Index = i * r + j;
                t.StartState = (int)GridState.Nomorl;
                t.StartType = (int)GridType.Nomorl;
                list.Add(t);
            }
            mapData.Grids.Add(i, list);
        }
    }
}
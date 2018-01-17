using KEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 格子工厂
/// </summary>
public class GridFaction
{
    private static AbstractResourceLoader mGridAssert;

    public static Grid CreateGrid()
    {
        if(mGridAssert == null)
        {
            GetGridAseert();
        }
        GameObject go = GameObject.Instantiate(mGridAssert.ResultObject as GameObject);
        Grid grid = go.GetComponent<Grid>();
        if(grid == null)
        {
            grid = go.AddComponent<Grid>();
        }
        return grid;
    }

    public static void PushGrid(Grid grid)
    {
        grid.CachedGameObject.SetActive(false);
    }

    private static void GetGridAseert()
    {
        mGridAssert = KResourceModule.LoadBundle(PrefabsPath.MapGrid);
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//is GridCenter Script
public class GridCenter : KBehaviour
{
    public Grid[][] DicGrid;
    public MapData mData;
    public int GridSize = 100;
    public List<Grid> mWipeList = new List<Grid>();
    /// <summary>
    /// 列
    /// </summary>
    public int Row;
    /// <summary>
    /// 行
    /// </summary>
    public int Column;

    public Grid mCurItem;

    #region 辅助方法

    public Vector3 GetPos(int id)
    {
        return new Vector3(GetGridPosX(id) * GridSize, GetGridPosY(id) * GridSize, 0);
    }

    /// <summary>
    /// 坐标得ID
    /// </summary>
    /// <param name="x">行</param>
    /// <param name="y">列</param>
    /// <returns></returns>
    public int GetGridID(int x,int y)
    {
        return x * Column + y;
    }

    /// <summary>
    /// 得到行号
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetGridPosY(int id)
    {
        return id % Row;
    }

    /// <summary>
    /// 得到列号
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetGridPosX(int id)
    {
        return (int)Mathf.Floor(id / Column);
    }

    public int GetLeft(int id)
    {
        if (id < 0)
        {
            return -1;
        }
        if (id - Column < 0)
        {
            return -1;
        }
        return id - Column;
    }

    public int GetDown(int id)
    {
        if (id < 0)
        {
            return -1;
        }
        if (id%Column == 0)
        {
            return -1;
        }
        return id - 1;
    }

    public int GetRight(int id)
    {
        if (id < 0)
        {
            return -1;
        }
        if (id+Row > Row*Column)
        {
            return -1;
        }
        return id + Row;
    }

    public int GetUp(int id)
    {
        if(id < 0)
        {
            return -1;
        }
        if(id % Column == Column - 1)
        {
            return -1;
        }
        return id + 1;
    }

    private bool IsEqual(int a, int b)
    {
        if(a < 0 || b < 0)
        {
            return false;
        }
        return a == b;
    }

    private bool IsEqual(int a, int b, int c)
    {
        if (a < 0 || b < 0 || c < 0)
        {
            return false;
        }
        return a == b && b == c;
    }

    private bool IsEqual(int a, int b, int c, int d)
    {
        if (a < 0 || b < 0 || c < 0 || d < 0)
        {
            return false;
        }
        return a == b && b == c && b == d;
    }

    public void SetGridDic(Grid grid, int gridx, int gridy)
    {
        DicGrid[gridx][gridy] = grid;
    }

    private bool IsEqual(int a, int b, int c, int d, int e)
    {
        if (a < 0 || b < 0 || c < 0 || d < 0 || e < 0)
        {
            return false;
        }
        return a == b && b == c && b == d && b == e;
    }
    #endregion
    
    /// <summary>
    /// 设置当前点击的item
    /// </summary>
    /// <param name="grid"></param>
    public void SetCurItem(Grid grid)
    {
        if(IsExchange)//整在交换不能操作
        {
            return;
        }

        if(mCurItem != null && grid == mCurItem)//点击的同一个item
        {
            mCurItem.SetClickState(false);
            mCurItem = null;
        }else if(mCurItem == null)//只选中一个item
        {
            mCurItem = grid;
            grid.SetClickState(true);
        }
        else if(CheckGridIsBorder(grid.index,mCurItem.index))//相邻的2个item都被选中
        {
            ExchangeGrid(mCurItem, grid,true);
        }else
        {
            mCurItem.SetClickState(false);
            mCurItem = grid;
            grid.SetClickState(true);
        }
    }

    private bool IsExchange = false;

    /// <summary>
    /// 交换grid位置
    /// </summary>
    /// <param name="grid1"></param>
    /// <param name="grid2"></param>
    public void ExchangeGrid(Grid grid1,Grid grid2,bool IsCheck)
    {
        IsExchange = true;

        bool a = false;
        bool b = false;
        //播放动画交换grid的位置
        grid1.Exchange(grid2.index,grid2.CachedTransform.localPosition, () => {
            //update Item Exchange or changeBack
            a = true;
            if (a && b)
            {
                IsExchange = false;
                if (IsCheck)
                {
                    bool ba = CheckGridWipe(grid1);
                    bool bb = CheckGridWipe(grid2);
                    UpdateWipe();
                    if (!ba && !bb)//两个都不能消除
                    {
                        ExchangeGrid(grid1, grid2, false);
                    }else
                    {
                        if(mCurItem != null)
                        {
                            mCurItem.SetClickState(false);
                            mCurItem = null;
                        }
                    }
                }
            }
        });
        grid2.Exchange(grid1.index,grid1.CachedTransform.localPosition, () => {
            //update Item Exchange or changeBack
            b = true;
            if (a && b)
            {
                IsExchange = false;
                if (IsCheck)
                {
                    bool ba = CheckGridWipe(grid1);
                    bool bb = CheckGridWipe(grid2);
                    UpdateWipe();
                    if (!ba && !bb)//两个都不能消除
                    {
                        ExchangeGrid(grid1, grid2, false);
                    }
                    else
                    {
                        if (mCurItem != null)
                        {
                            mCurItem.SetClickState(false);
                            mCurItem = null;
                        }
                    }
                }
            }
        });
    }

    /// <summary>
    /// 判断一个grid能否消除
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    private bool CheckGridWipe(Grid grid)
    {
        //处理边界
        int m = grid.index;
        int up1 = GetUp(grid.index);
        int up2 = GetUp(up1);
        int left1 = GetLeft(grid.index);
        int left2 = GetLeft(left1);
        int right1 = GetRight(grid.index);
        int right2 = GetRight(right1);
        int down1 = GetDown(grid.index);
        int down2 = GetDown(down1);

        int cm = grid.GridType;
        int cUp1 = GetGridSprite(up1);
        int cUp2 = GetGridSprite(up2);
        int cLeft1 = GetGridSprite(left1);
        int cLeft2 = GetGridSprite(left2);
        int cRight1 = GetGridSprite(right1);
        int cRight2 = GetGridSprite(right2);
        int cDown1 = GetGridSprite(down1);
        int cDown2 = GetGridSprite(down2);

        //Debug.Log("m   " + m + "cm" + cm);
        //Debug.Log("up1   " + up1 + "cUp1  " + cUp1);
        //Debug.Log("up2   " + up2 + "cUp2  " + cUp2);
        //Debug.Log("left1   " + left1 + "cLeft1  " + cLeft1);
        //Debug.Log("left2   " + left2 + "cLeft2  " + cLeft2);
        //Debug.Log("right1   " + right1 + "cRight1  " + cRight1);
        //Debug.Log("right2   " + right2 + "cRight2  " + cRight2);
        //Debug.Log("down1   " + down1 + "cDown1  " + cDown1);
        //Debug.Log("down2   " + down2 + "cDown2  " + cDown2);

        List<int> wipeList = new List<int>();
        //中间
        if(IsEqual(cUp1,cUp2,cm))//判断上3
        {
            wipeList.Add(m);
            wipeList.Add(up1);
            wipeList.Add(up2);
            if (IsEqual(cm,cDown1))//中间4
            {
                wipeList.Add(down1);
                if (IsEqual(cm,cDown2))
                {
                    wipeList.Add(down2);
                }
            }
        }else if(IsEqual(cDown1, cDown2, cm))
        {
            wipeList.Add(m);
            wipeList.Add(down1);
            wipeList.Add(down2);
            if (IsEqual(cm, cUp1))//中间4
            {
                wipeList.Add(up1);
            }
        }else if(IsEqual(cm,cUp1,cDown1))
        {
            wipeList.Add(m);
            wipeList.Add(up1);
            wipeList.Add(down1);
        }

        //左右
        if(IsEqual(cLeft1, cLeft2, cm))//判断上3
        {
            wipeList.Add(m);
            wipeList.Add(left1);
            wipeList.Add(left2);
            if (IsEqual(cm, cRight1))//中间4
            {
                wipeList.Add(right1);
                if (IsEqual(cm, cRight2))
                {
                    wipeList.Add(right2);
                }
            }
        }else if (IsEqual(cRight1, cRight2, cm))//右边3个
        {
            wipeList.Add(m);
            wipeList.Add(right1);
            wipeList.Add(right2);
            if (IsEqual(cm, cLeft1))//中间4
            {
                wipeList.Add(left1);
            }
        }
        else if (IsEqual(cm, cRight1, cLeft1))
        {
            wipeList.Add(m);
            wipeList.Add(right1);
            wipeList.Add(left1);
        }

        for (int i = 0; i < wipeList.Count; i++)
        {
            Grid g = GetGrid(wipeList[i]);
            if(g == null)
            {
                Debug.LogError("消除逻辑有错误！！！！！！！" + wipeList[i] + "    "+grid.index);
            }
            mWipeList.Add(g);
           
        }
        if(wipeList.Count > 0)
        {
            return true;
        }

        return false;
    }

    public void WipeGrid(Grid grid)
    {
        if(grid == null)
        {
            return;
        }

        grid.Wipe();
    }

    public void UpdateWipe()
    {
        List<int> line = new List<int>();
        for (int i = 0; i < mWipeList.Count; i++)
        {
            if(!line.Contains(mWipeList[i].Gridx))
            {
                line.Add(mWipeList[i].Gridx);
            }
            mWipeList[i].Wipe();
            DicGrid[mWipeList[i].Gridx][mWipeList[i].Gridy] = null;
        }
        mWipeList.Clear();
        // xialuo luoji 
        line.Sort();
        for (int i = 0; i < line.Count; i++)
        {
            IsDropLine(line[i]);
        }
    }

    //整列是否能下落
    public void IsDropLine(int line)
    {
        int id = Column * line + 1;
        for (int i = 1; i < Column; i++)
        {
            id = Column * line + i;
            Grid g = GetGrid(id);
            if (GetGrid(id) == null)
            {
                continue;
            }
            int dropId = DropId(id);
            if(id == dropId || id < 0)
            {
                continue;
            }
            IsDropGrid(g,dropId);
        }
    }
    //单个
    public void IsDropGrid(Grid grid,int index)
    {
        if(grid == null)
        {
            return;
        }
        int dropId = index;
        if(dropId == grid.index || dropId < 0)
        {
            return;
        }

        Vector3 pos = GetPos(dropId);
        grid.Exchange(dropId, pos, () =>{
            //Debug.Log("一个格子下落成功"+ dropId);
            if(grid != null && !grid.IsWipe)
            {
                CheckGridWipe(grid);
                UpdateWipe();
            }
        },true);
    }

    public int DropId(int id)
    {
        int down = GetDown(id);
        if(down < 0)//不能下落了
        {
            return id;
        }

        if(GetGrid(down) != null)
        {
            return id;
        }
        return DropId(down);
    }
    /// <summary>
    /// 所有下落grid
    /// </summary>
    public void IsDropAll()
    {
        for (int i = 0; i < Row; i++)
        {
            IsDropLine(i);
        }
    }

    public Grid GetGrid(int x,int y)
    {
        return DicGrid[x][y];
    }

    public Grid GetGrid(int index)
    {
        int x = GetGridPosX(index);
        int y = GetGridPosY(index);

        if (x < 0 || y < 0 || x > Row - 1 || y > Column - 1)
        {
            return null;
        }
        return GetGrid(x, y);
    }

    private int GetGridSprite(int index)
    {
        int x = GetGridPosX(index);
        int y = GetGridPosY(index);

        if(x<0 || y <0 || x > Row-1 || y > Column-1 || GetGrid(x, y) == null)
        {
            return -1;
        }
        return GetGrid(x, y).GridType;
    }

    /// <summary>
    /// 判断2个grid是否相邻
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="grid1"></param>
    private bool CheckGridIsBorder(int index1 ,int index2)
    {
        int left = GetLeft(index2);
        int right = GetRight(index2);
        int down = GetDown(index2);
        int up = GetUp(index2);
        if (index1 == GetLeft(index2) || index1 == GetRight(index2) || index1 == GetDown(index2) || index1 == GetUp(index2))
        {
            return true;
        }
        return false;
    }

	public void Init (MapData data) 
	{
        mData = data;
        transform.localPosition = data.pos;
        IsExchange = false;
       
        Column = mData.Grids.Count;//列
        Row = mData.Grids[0].Count;//行
        DicGrid = new Grid[Column][];

        foreach (var temp in mData.Grids)
        {
            DicGrid[temp.Key] = new Grid[temp.Value.Count];
            for (int i = 0; i < temp.Value.Count; i++)
            {
                Grid g = GridFaction.CreateGrid();
                g.Init(this,temp.Value[i]);
                DicGrid[temp.Key][i] = g;
            }
        }

        for (int i = 0; i < DicGrid.Length; i++)
        {
            for (int j = 0; j < DicGrid[i].Length; j++)
            {
                CheckGridWipe(DicGrid[i][j]);
            } 
        }
        UpdateWipe();
    }

    public int GetNewType(int gridID)
    {
        int LeftType= -1;
        int DownType = -1;

        int y = GetGridPosY(gridID);
        int x = GetGridPosX(gridID);

        if (y > 1)//列数大于1检查行相邻不能散个相同颜色
        {
            int down = GetDown(gridID);
            int down_down = GetDown(down);

            if (GetGrid(down).GridType == GetGrid(down_down).GridType)
            {
                DownType = GetGrid(down).GridType;
            }
        }
        if (x > 1)
        {
            int left = GetLeft(gridID);
            int left_left = GetLeft(left);
            if (GetGrid(left).GridType == GetGrid(left_left).GridType)
            {
                LeftType = GetGrid(left).GridType;
            }
        }
        if (LeftType != DownType)
        {
            return Range(DownType, LeftType);
        }
        return UnityEngine.Random.Range(0, 4);
    }

    int Range(int down,int left)
    {
        int NewType = UnityEngine.Random.Range(0,4);
        if (down != NewType && left != NewType)
        {
            return NewType;
        }
        return Range(down, left); 
    }

    public void Clear()
    {
        Destroy(gameObject);
    }
}

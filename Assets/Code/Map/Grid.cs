using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using DG.Tweening;


public class Grid : KBehaviour 
{
    public int index;
    public int Gridx;
    public int Gridy;
    public int GridType = 0;
    public GridCenter mCenter;
    public GridData mData;

    private Image spIcon;
    private Image spBg;

    public void Awake()
    {
        EventTriggerListener.Get(CachedGameObject).onClick = OnClickItem;
    }

    private void OnClickItem(GameObject go)
    {
        mCenter.SetCurItem(this);
    }

  

    public void Init (GridCenter center,GridData data) 
	{
        mCenter = center;
        mData = data;
        CachedTransform.SetParent(center.transform,false);

        index = data.Index;
        name = index.ToString();
        Gridx = center.GetGridPosX(index);
        Gridy = center.GetGridPosY(index);
        GridType = mData.SpriteType;
        transform.localPosition = new Vector3(Gridx * mCenter.GridSize, Gridy * mCenter.GridSize, 0);

        spIcon = transform.Find("icon").GetComponent<Image>();
        int type = mCenter.GetNewType(index);
        SetGridSrpiteType(type);
        spIcon.sprite = GamePlayMgr.Instance.GetBlockSprite(type);

        spBg = transform.Find("bg").GetComponent<Image>();
        SetClickState(false);
	}

    private void SetGridSrpiteType(int type)
    {
        GridType = type;
        mData.SpriteType = type;
    }

    /// <summary>
    /// 交换2个grid的位置
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="p"></param>
    public void Exchange(int index,Vector3 pos, Action p,bool FristChange = false)
    {
        float time = Vector3.Distance(pos, CachedTransform.localPosition) / LevelMapMgr.Instance.MoveSpeed;
        Tweener t = CachedTransform.DOLocalMove(pos, time).SetEase(Ease.Linear);
        
        if(FristChange)
        {
            ExChangePosMsg(index);
        }

        t.OnComplete(() =>
        {
            if (!FristChange)
            {
                ExChangePosMsg(index);
            }
            if (p != null)
            {
                p();
            }
        });
    }

    public void SetClickState(bool state)
    {
        spBg.enabled = state;
    }

    public void ExChangePosMsg(int index)
    {
        this.index = index;
        if (mCenter.GetGrid(Gridx, Gridy) == this)
        {
            mCenter.SetGridDic(null, Gridx, Gridy);
        }
        Gridx = mCenter.GetGridPosX(index);
        Gridy = mCenter.GetGridPosY(index);
        name = index.ToString();
        CachedTransform.SetSiblingIndex(index);

        mCenter.SetGridDic(this, Gridx, Gridy);
    }

    public bool IsWipe = false;
    public void Wipe()
    {
        //spBg.enabled = true;
        IsWipe = true;
        Tweener t = CachedTransform.DOScale(0, 0.2f);
        t.OnComplete(() => {
            CachedTransform.localScale = Vector3.one;
            gameObject.SetActive(false);
        });
        
       // Debug.Log("========消除" + this.index);
    }
}

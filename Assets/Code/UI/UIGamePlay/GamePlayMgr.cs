using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KEngine.UI;
using UnityEngine.UI;
using KEngine;

public class GamePlayMgr : CSharpSingletion<GamePlayMgr>
{
    class LevelSprite
    {
        public Sprite sprite;
        public string path;
    }

    private List<LevelSprite> mAllSprite = new List<LevelSprite>();
    private List<LevelSprite> CurLevelSprite = new List<LevelSprite>();

	public void Init()
    {
    }

    private bool IsLoadingLevel = false;
    private bool IsLoadSprite = false;
    private bool IsLoadUI = false;
    private List<string> spritePath = new List<string>()
    {
        "UIAtlas/Block/Clock.png",
         "UIAtlas/Block/Gift.png",
          "UIAtlas/Block/LifePreserver.png",
           "UIAtlas/Block/Spiral.png",
            "UIAtlas/Block/Teddy.png"
    };

    public void SetLoadUIOk()
    {
        IsLoadUI = false;
    }

    public void LodGameLevel(int index)
    {
        IsLoadingLevel = true;
        IsLoadSprite = true;
        IsLoadUI = true;
        Game.RegisterUpdate(CheckStart);
        LoadSprite();
    }

    public void OutGameLevel()
    {
        CurLevelSprite.Clear();
    }

    //加载关卡图片
    private void LoadSprite()
    {
        CurLevelSprite.Clear();
        List<string> temp = new List<string>();

        bool flag = false;
        for (int i = 0; i < spritePath.Count; i++)
        {
            flag = false;
            for (int j = 0; j < mAllSprite.Count; j++)
            {
                if(mAllSprite[j].path == spritePath[i])
                {
                    CurLevelSprite.Add(mAllSprite[j]);
                    flag = true;
                    break;
                }
            }
            if(!flag)//没有加载的加入加载列表
            {
                temp.Add(spritePath[i]);
            }
        }

        int loadCount = temp.Count;
        if (loadCount == 0)
        {
            LoadUI();
            IsLoadSprite = false;
            return;
        }
        for (int i = 0; i < temp.Count; i++)
        {
            SpriteLoader.Load(temp[i],(isok,sprite)=>
            {
                if(!isok)
                {
                    Debug.LogError("加载关卡图标失败" + temp[i]);
                   
                }else
                {
                    LevelSprite sp = new LevelSprite();
                    sp.sprite = sprite;
                    sp.path = temp[i];
                    CurLevelSprite.Add(sp);
                    mAllSprite.Add(sp);
                    loadCount--;
                    Debug.Log("load sprite " + loadCount);
                    if(loadCount == 0)//加载图片完成
                    {
                        IsLoadSprite = false;
                        LoadUI();
                    }
                }
            });
        }
        
       
    }

    //加载完图片加载UI
    private void LoadUI()
    {
        UILoadState state = UIModule.Instance.GetCurMainState();
        UIModule.Instance.OpenWindow<UI_GamePlay>(WindowID.GamePlayWnd, WindowID.GamePlayWnd, null, (ui, agrs) =>
        {
            if (state != null)
            {
                state.Close();
            }
            UI_GamePlay.Instance.InitLevel();
        }, null);
    }

    private void CheckStart(float time)
    {
        if(IsLoadingLevel)
        {
            if(!IsLoadSprite && !IsLoadUI)
            {
                LoadLevelFinish();
                IsLoadingLevel = false;//加载完成
            }
        }else
        {
            Game.RemoveUpdate(CheckStart);
        }
    }

    //加载完成
    private void LoadLevelFinish()
    {
       //去掉loading开始
    }

    public Sprite GetBlockSprite(int id)
    {
        if(id > CurLevelSprite.Count)
        {
            Debug.LogError("no sprite id :  " + id);
            return null;
        }

        return CurLevelSprite[id].sprite;
    }
}

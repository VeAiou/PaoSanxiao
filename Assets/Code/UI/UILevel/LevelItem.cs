using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour {

    private GameObject mLock;
    private Image[] mStars = new Image[3];
    private GameObject mNewSticker;
    private Text LevelNumberText;

    private void Awake()
    {
        FindConptent();
    }

    public void FindConptent()
    {
        mLock = transform.Find("Lock").gameObject;
        Transform starParent = transform.Find("Stars/Stars");
        for (int i = 0; i < starParent.childCount; i++)
        {
            Image a = starParent.GetChild(i).gameObject.GetComponent<Image>();
            mStars[i] = a;
        }
        mNewSticker = transform.Find("Level4/NewSticker").gameObject;
        LevelNumberText = transform.Find("Level4/LevelNumberText").GetComponent<Text>();

        EventTriggerListener.Get(LevelNumberText.transform.parent.gameObject).onClick = OnClickOpenLevel;
    }

    public void Init()
    {

    }

    public void OnClickOpenLevel(GameObject go)
    {
        Debug.Log("打开关卡");
        GamePlayMgr.Instance.LodGameLevel(0);
    }
}

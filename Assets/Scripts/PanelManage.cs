using UnityEngine;
using System.Collections.Generic;

public class PanelManage : MonoBehaviour
{
    // Start is called before the first frame update
    public Panels panels;

    public static PanelManage panelManage;

    public talkContent currentTextDataList;
    [SerializeField] BagItem bagItem;

    private void Awake()
    {
        panelManage = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(playerController.playerController_.playerKeyCodes.OpenShop))
        {
            if (panels.shopPanel.gameObject.activeSelf)
            {
                panels.shopPanel.gameObject.SetActive(false);
                merchantShop.merchantShop_.Buy_close();
            }
            // 如果全部的panel都沒被打開
            else if (!AllPanelStatus())
            {
                merchantShop.merchantShop_.keydownOpenShopPanel();
            }

        }

        if (Input.GetKeyDown(playerController.playerController_.playerKeyCodes.OpenBag))
        {
            OpenBag();
        }

        if (Input.GetKeyDown(playerController.playerController_.playerKeyCodes.OpenMap))
        {
            OpenBigMapPanel();
        }

        if (Input.GetKeyDown(playerController.playerController_.playerKeyCodes.OpenESC))
        {
            OpenESCPanel();
        }


        // --------------------------------------要刪

        if (Input.GetKeyDown(KeyCode.Z))
        {

            talkSystem.talkSystem_.openTalk(currentTextDataList.TextDataList);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {

            BagManage.bagManage.checkItem(bagItem, -10);
        }


    }

    // 開啟背包
    public void OpenBag()
    {
        if (panels.BagPanel.gameObject.activeSelf)
        {
            panels.BagPanel.gameObject.SetActive(false);
            BagManage.bagManage.hiddenBagInfo();
        }
        // 如果全部的panel都沒被打開
        else if (!AllPanelStatus())
        {
            panels.BagPanel.gameObject.SetActive(true);
        }
    }

    // 開啟任務清單
    public void OpenTaskPanel()
    {
        if (panels.TaskPanel.gameObject.activeSelf)
        {
            panels.TaskPanel.gameObject.SetActive(false);
        }
        // 如果全部的panel都沒被打開
        else if (!AllPanelStatus())
        {
            panels.TaskPanel.gameObject.SetActive(true);
        }
    }

    // 開啟大地圖
    public void OpenBigMapPanel()
    {
        if (panels.BigMapPanel.gameObject.activeSelf)
        {
            panels.BigMapPanel.gameObject.SetActive(false);
            playerController.playerController_.setCanRotateCamera(true);
        }
        // 如果全部的panel都沒被打開
        else if (!AllPanelStatus())
        {
            panels.BigMapPanel.gameObject.SetActive(true);
            playerController.playerController_.setCanRotateCamera(false);
            BigMapSystem.bigMapSystem.Update_MapLoad();
        }
    }

    // 開啟選單
    public void OpenESCPanel()
    {
        if (panels.ESCpanel.gameObject.activeSelf)
        {
            panels.ESCpanel.gameObject.SetActive(false);
        }
        // 如果全部的panel都沒被打開
        else if (!AllPanelStatus())
        {
            panels.ESCpanel.gameObject.SetActive(true);
        }
    }

    // show/hide小地圖
    public void OpenSmallMap()
    {
        if (panelManage.panels.smallMap.gameObject.activeSelf)
        {
            panelManage.panels.smallMap.gameObject.SetActive(false);
        }
        else
        {
            panelManage.panels.smallMap.gameObject.SetActive(true);
        }
    }

    // 所有panel狀態
    public bool AllPanelStatus()
    {
        return
        panels.shopPanel.gameObject.activeSelf || panels.BagPanel.gameObject.activeSelf ||
        panels.TaskPanel.gameObject.activeSelf || panels.BigMapPanel.gameObject.activeSelf ||
        panels.talkPanel.gameObject.activeSelf || panels.ESCpanel.gameObject.activeSelf
        ;
    }

    // 退出遊戲
    public void ExitGame()
    {

    }
}


[System.Serializable]
public class Panels
{
    // 商店
    public Transform shopPanel;
    // 背包
    public Transform BagPanel;
    public Transform HotKeyPanel;
    // 任務
    public Transform TaskPanel;

    #region 大地圖
    // 大地圖
    public Transform BigMapPanel;
    public Transform BigMapPhotography;
    #endregion
    // 小地圖
    public Transform smallMap;
    // 對話
    public Transform talkPanel;
    // 選單畫面
    public Transform ESCpanel;
}
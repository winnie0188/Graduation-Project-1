using UnityEngine;
using System.Collections.Generic;

public class PanelManage : MonoBehaviour
{
    // Start is called before the first frame update
    public Panels panels;

    public static PanelManage panelManage;



    //之後要刪
    [SerializeField] BagItem bagItem;
    public talkContent currentTextDataList;
    //

    [SerializeField] Camera[] Cameras;

    // 是否開發者模式
    [SerializeField] bool isCreateMode;


    #region 切換模式
    public void setIsCreateMode(bool Mode)
    {
        isCreateMode = Mode;
    }
    public bool getIsCreateMode()
    {
        return isCreateMode;
    }
    #endregion



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
        // if (Input.GetKeyDown(playerController.playerController_.playerKeyCodes.OpenShop))
        // {
        //     if (talkSystem.talkSystem_.isToch)
        //     {
        //         talkSystem.talkSystem_.openTalk(talkSystem.talkSystem_.TriggerObj.GetComponent<people>().currentTextDataList.TextDataList);
        //     }
        // }

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

        // if (Input.GetKeyDown(KeyCode.Z))
        // {
        //     talkSystem.talkSystem_.openTalk(currentTextDataList);
        // }
        // if (Input.GetKeyDown(KeyCode.X))
        // {

        //     BagManage.bagManage.checkItem(bagItem, -10, false, true);
        // }
        // if (Input.GetKeyDown(KeyCode.C))
        // {

        //     LightingManager.lightingManager.addTime(0);
        // }
        // else if (Input.GetKeyDown(KeyCode.V))
        // {
        //     LightingManager.lightingManager.addTime(1);
        // }



    }

    // 開啟背包
    public void OpenBag()
    {
        if (panels.BagPanel.gameObject.activeSelf)
        {
            Cameras[1].gameObject.SetActive(false);

            panels.BagPanel.gameObject.SetActive(false);
            BagManage.bagManage.hiddenBagInfo();
        }
        // 如果全部的panel都沒被打開
        else if (!AllPanelStatus())
        {
            //Cameras[1].transform.position = Cameras[0].transform.position;
            //Cameras[1].transform.rotation = Cameras[0].transform.rotation;

            Cameras[1].gameObject.SetActive(true);

            panels.BagPanel.gameObject.SetActive(true);
        }
    }

    // 開啟任務清單
    public void OpenTaskPanel()
    {
        if (!AllPanelStatus())
        {
            panels.BagPanel.gameObject.SetActive(true);
            BagManage.bagManage.switch_category(7);
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
            ESCsystem.ESCsystem_.hiddeESCchild();
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
        panels.shopPanel.gameObject.activeSelf || panels.BagPanel.gameObject.activeSelf
         || panels.BigMapPanel.gameObject.activeSelf ||
        panels.talkPanel.gameObject.activeSelf || panels.ESCpanel.gameObject.activeSelf
        ;
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
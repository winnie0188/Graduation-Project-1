using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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
        if (isDontOpen())
        {
            return;
        }
        // if (Input.GetKeyDown(playerController.playerController_.playerKeyCodes.OpenShop))
        // {
        //     if (panels.shopPanel.gameObject.activeSelf)
        //     {
        //         panels.shopPanel.gameObject.SetActive(false);
        //         merchantShop.merchantShop_.Buy_close();
        //     }
        //     // 如果全部的panel都沒被打開
        //     else if (!AllPanelStatus())
        //     {
        //         merchantShop.merchantShop_.keydownOpenShopPanel();
        //     }
        // }
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            LightingManager.lightingManager.addTime();
        }
        // else if (Input.GetKeyDown(KeyCode.V))
        // {
        //     LightingManager.lightingManager.addTime(1);
        // }
    }

    #region UI開啟

    #region 背包

    // 開啟背包
    public void OpenBag()
    {
        bag(!AllPanelStatus());
    }
    //信==================================
    public void letter(Sprite logo, Sprite content, talkContent talk, NPC npc, BagItem bagItem)
    {
        panels.letterContent.GetComponent<Button>().onClick.AddListener(() => letterClose(talk, npc, bagItem));

        panels.letterContent.sprite = content;
        panels.letterLogo.sprite = logo;
        panels.letterPanel.gameObject.SetActive(true);
    }
    public void letterClose(talkContent talk, NPC npc, BagItem bagItem)
    {
        OpenBag();
        panels.letterPanel.gameObject.SetActive(false);
        panels.letterContent.gameObject.SetActive(false);
        panels.letterLogo.transform.parent.gameObject.SetActive(true);


        //清空對方的任務
        npc.setTask(null);
        //新增信的任務
        talkSystem.talkSystem_.openTalk(talk);

        BagManage.bagManage.checkItem(
            bagItem, -1, false, true
        );

        panels.letterContent.GetComponent<Button>().onClick.RemoveAllListeners();
    }
    //信==================================
    #endregion

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
        bigmap(!AllPanelStatus());
    }

    // 開啟選單
    public void OpenESCPanel()
    {
        esc(!AllPanelStatus());
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
    #endregion

    #region 按件開啟
    public bool includeAllState()
    {
        return Guide.guide.canvas1.gameObject.activeSelf || FinshUi.finshUi.uiCanvas.gameObject.activeSelf || AllPanelStatus() || taskSystem.taskSystem_.isOpen();
    }

    //特定ui判斷
    public bool isDontOpen()
    {
        return Guide.guide.canvas1.gameObject.activeSelf || FinshUi.finshUi.uiCanvas.gameObject.activeSelf || taskSystem.taskSystem_.isOpen();
    }

    #endregion

    #region open
    void bag(bool open)
    {
        if (panels.BagPanel.gameObject.activeSelf)
        {
            Cameras[1].gameObject.SetActive(false);

            panels.BagPanel.gameObject.SetActive(false);
            BagManage.bagManage.hiddenBagInfo();
        }
        // 如果全部的panel都沒被打開
        else if (open)
        {
            //Cameras[1].transform.position = Cameras[0].transform.position;
            //Cameras[1].transform.rotation = Cameras[0].transform.rotation;

            Cameras[1].gameObject.SetActive(true);

            panels.BagPanel.gameObject.SetActive(true);

            if (taskSystem.taskSystem_.taskContentPanel.gameObject.activeSelf && !taskSystem.isReset)
            {
                StartCoroutine(taskSystem.taskSystem_.Reset_Panel());
            }
        }
    }


    void esc(bool open)
    {
        if (panels.ESCpanel.gameObject.activeSelf)
        {
            panels.ESCpanel.gameObject.SetActive(false);
            ESCsystem.ESCsystem_.hiddeESCchild();
        }
        // 如果全部的panel都沒被打開
        else if (open)
        {
            panels.ESCpanel.gameObject.SetActive(true);
        }
    }



    void bigmap(bool open)
    {
        if (panels.BigMapPanel.gameObject.activeSelf)
        {
            panels.BigMapPanel.gameObject.SetActive(false);
            playerController.playerController_.setCanRotateCamera(true);
        }
        // 如果全部的panel都沒被打開
        else if (open)
        {
            panels.BigMapPanel.gameObject.SetActive(true);
            playerController.playerController_.setCanRotateCamera(false);
            BigMapSystem.bigMapSystem.Update_MapLoad();
        }
    }
    #endregion
}


[System.Serializable]
public class Panels
{
    // 商店
    public Transform shopPanel;

    //----------背包------------
    public Transform BagPanel;
    //---信---
    public Transform letterPanel;
    public Image letterLogo;
    public Image letterContent;
    //---信---
    public Transform HotKeyPanel;
    //----------背包------------

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
using UnityEngine;

public class PanelManage : MonoBehaviour
{
    // Start is called before the first frame update
    public Panels panels;

    public static PanelManage panelManage;





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

            panels.BigMapPhotography.gameObject.SetActive(false);
            panels.BigMapPanel.gameObject.SetActive(false);
        }
        // 如果全部的panel都沒被打開
        else if (!AllPanelStatus())
        {
            panels.BigMapPanel.gameObject.SetActive(true);
            panels.BigMapPhotography.gameObject.SetActive(true);

            BigMapSystem.bigMapSystem.Update_Map_Void();
        }
    }

    // 所有panel狀態
    public bool AllPanelStatus()
    {
        return panels.shopPanel.gameObject.activeSelf || panels.BagPanel.gameObject.activeSelf || panels.TaskPanel.gameObject.activeSelf || panels.BigMapPanel.gameObject.activeSelf;
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
}
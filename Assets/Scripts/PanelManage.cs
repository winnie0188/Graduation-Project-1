using UnityEngine;

public class PanelManage : MonoBehaviour
{
    // Start is called before the first frame update
    public Panels panels;

    public static PanelManage panelManage;

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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            BagManage.bagManage.checkItem(bagItem, -1);
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

    // 所有panel狀態
    public bool AllPanelStatus()
    {
        return panels.shopPanel.gameObject.activeSelf || panels.BagPanel.gameObject.activeSelf;
    }
}


[System.Serializable]
public class Panels
{
    public Transform shopPanel;
    public Transform BagPanel;
    public Transform HotKeyPanel;
}
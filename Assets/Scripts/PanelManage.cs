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
            else
            {
                merchantShop.merchantShop_.keydownOpenShopPanel();
            }

        }

        if (Input.GetKeyDown(playerController.playerController_.playerKeyCodes.OpenBag))
        {
            if (panels.BagPanel.gameObject.activeSelf)
            {
                panels.BagPanel.gameObject.SetActive(false);
                BagManage.bagManage.hiddenBagInfo();
            }
            else
            {
                panels.BagPanel.gameObject.SetActive(true);
            }
        }
    }
}


[System.Serializable]
public class Panels
{
    public Transform shopPanel;
    public Transform BagPanel;
    public Transform HotKeyPanel;
}
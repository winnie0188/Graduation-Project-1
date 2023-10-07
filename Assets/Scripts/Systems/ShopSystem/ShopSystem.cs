using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSystem : UIinit
{
    [Header("叉叉")]
    [SerializeField] Sprite x;
    [SerializeField] int slotCount;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform buyslotContent;
    [SerializeField] ShopInfo buyInfo;
    int buyCount;
    int buyPrice;
    int buyIndex;

    [SerializeField] Transform selllotContent;
    [SerializeField] ShopInfo sellInfo;
    int sellCount;
    int sellPrice;
    int sellIndex;
    int maxCount;
    [SerializeField] List<merchantItem> merchants = new List<merchantItem>();
    [SerializeField] int Playerprice;//玩家的錢
    [SerializeField] Text PlayerpriceTxt;//玩家的錢渲染
    public static ShopSystem shopSystem;

    private void Awake()
    {
        shopSystem = this;

        // init slot
        initSlot(slotCount, slotPrefab, buyslotContent, 1, -1);
        initSlot2(slotCount, slotPrefab, selllotContent, 1, -1);
        UI_loading(merchants);
        UI_loading2(merchants);
        UpdatePlayerprice();
    }

    public void OpenShopPanel(List<merchantItem> merchantItems)
    {
        merchants = merchantItems;
        UI_loading(merchants);
        UI_loading2(merchants);
        PanelManage.panelManage.panels.shopPanel.gameObject.SetActive(true);
    }

    public void UpdatePlayerprice()
    {
        PlayerpriceTxt.text = Playerprice + "";
    }

    #region buy
    //UI加載
    public void UI_loading(List<merchantItem> merchantItems)
    {
        for (int i = 0; i < buyslotContent.childCount; i++)
        {
            if (i < merchantItems.Count)
            {
                buyslotContent.GetChild(i).gameObject.SetActive(true);

                var ShopItem = merchantItems[i];
                buyslotContent.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = ShopItem.merchantItem_BagItem.BagItem_icon;
                buyslotContent.GetChild(i).GetChild(1).GetComponent<Text>().text = merchantItems[i].merchantItem_BagItem.BagItem_name;
                buyslotContent.GetChild(i).GetChild(2).GetComponent<Text>().text = "$ " + merchantItems[i].price;
                buyslotContent.GetChild(i).GetChild(3).GetComponent<Text>().text = merchantItems[i].merchantItem_BagItem.BagItem_info;
            }
            //溢出處理
            else if (!buyslotContent.GetChild(i).gameObject.activeSelf)
            {
                break;
            }
            else
            {
                // 渲染關閉
                buyslotContent.GetChild(i).gameObject.SetActive(false);
            }
        }

        buyInfoRestart();

    }

    void buyInfoRestart()
    {
        buyInfo.icon.sprite = x;
        buyInfo.price.text = "0";
        buyInfo.count.text = "0";

        buyIndex = -1;
    }

    public override void slot_event(int index)
    {
        if (merchants.Count > index && merchants[index] != null)
        {
            buyCount = 1;
            buyPrice = merchants[index].price;
            buyInfo.icon.sprite = merchants[index].merchantItem_BagItem.BagItem_icon;
            buyInfo.price.text = buyPrice.ToString();
            buyInfo.count.text = buyCount.ToString();

            buyIndex = index;
        }
    }

    public void addCount()
    {
        buyCount++;
        buyInfo.price.text = (buyPrice * buyCount).ToString();
        buyInfo.count.text = buyCount.ToString();
    }

    public void redCount()
    {
        print(1);
        buyCount--;
        if (buyCount <= 0)
        {
            buyCount = 1;
        }

        buyInfo.price.text = (buyPrice * buyCount).ToString();
        buyInfo.count.text = buyCount.ToString();
    }

    public void buy()
    {
        int totalprice = buyPrice * buyCount;
        if (buyIndex != -1 && Playerprice >= totalprice)
        {
            Playerprice -= totalprice;

            BagManage.bagManage.checkItem(
                merchants[buyIndex].merchantItem_BagItem,
                buyCount,
                false,
                true
            );

            UpdatePlayerprice();

            buyInfoRestart();
        }
        else
        {
            print("餘額不足");
        }
    }

    #endregion


    #region sell

    public void UI_loading2(List<merchantItem> merchantItems)
    {
        for (int i = 0; i < selllotContent.childCount; i++)
        {
            if (i < merchantItems.Count)
            {
                selllotContent.GetChild(i).gameObject.SetActive(true);

                var ShopItem = merchantItems[i];
                selllotContent.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = ShopItem.merchantItem_BagItem.BagItem_icon;
                selllotContent.GetChild(i).GetChild(1).GetComponent<Text>().text = merchantItems[i].merchantItem_BagItem.BagItem_name;
                selllotContent.GetChild(i).GetChild(2).GetComponent<Text>().text = "$ " + merchantItems[i].price;
                selllotContent.GetChild(i).GetChild(3).GetComponent<Text>().text = merchantItems[i].merchantItem_BagItem.BagItem_info;
            }
            //溢出處理
            else if (!selllotContent.GetChild(i).gameObject.activeSelf)
            {
                break;
            }
            else
            {
                // 渲染關閉
                selllotContent.GetChild(i).gameObject.SetActive(false);
            }
        }

        sellInfoRestart();
    }

    void sellInfoRestart()
    {
        sellInfo.icon.sprite = x;
        sellInfo.price.text = "0";
        sellInfo.count.text = "0";

        sellIndex = -1;
    }

    public override void slot_event2(int index)
    {
        if (merchants.Count > index && merchants[index] != null)
        {
            sellCount = 1;
            sellPrice = merchants[index].merchantItem_BagItem.sellPrice;
            sellInfo.icon.sprite = merchants[index].merchantItem_BagItem.BagItem_icon;
            sellInfo.price.text = sellPrice.ToString();
            sellInfo.count.text = sellCount.ToString();

            sellIndex = index;
            maxCount = BagManage.bagManage.itemCount(merchants[index].merchantItem_BagItem);
        }
    }

    public void addSell()
    {
        if (sellCount + 1 <= maxCount)
        {
            sellCount++;
            sellInfo.price.text = (sellPrice * sellCount).ToString();
            sellInfo.count.text = sellCount.ToString();
        }
        else
        {
            print("已超過上限");
        }
    }

    public void redSell()
    {
        buyCount--;
        if (buyCount <= 0)
        {
            buyCount = 1;
        }
        else
        {
            print("已超過下限");
        }

        buyInfo.price.text = (buyPrice * buyCount).ToString();
        buyInfo.count.text = buyCount.ToString();
    }

    public void sell()
    {
        if (sellCount <= maxCount)
        {
            Playerprice += (sellCount * sellPrice);
            if (sellIndex != -1)
            {
                BagManage.bagManage.checkItem(
                    merchants[sellIndex].merchantItem_BagItem,
                    sellCount * -1,
                    false,
                    true
                );
            }
            UpdatePlayerprice();
            sellInfoRestart();
        }
        else
        {
            print("背包沒有相應的道具");
        }

    }
    #endregion

}
[System.Serializable]
public class ShopInfo
{
    public Image icon;
    public TMP_Text count;
    public TMP_Text price;
}
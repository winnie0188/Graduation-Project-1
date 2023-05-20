using System.Collections.Generic;
using UnityEngine;

public enum merchantState
{
    live,//此模式不能進行互動
    shop//此模式觸碰可以進行交易
}

public class merchant : people
{
    // Start is called before the first frame update
    [SerializeField] bool isToch;//是否碰到玩家

    [SerializeField] List<merchantItem> AllItem;//商人販賣商品
    //int AllItemcount;

    public merchantState merchantState_;

    [SerializeField] int[] itemIndex;//商品索引
    [SerializeField] int sellLength;//商品數量。會隨時變動

    //設置AllItem.Count
    // private void OnValidate()
    // {
    //     if (AllItem != null)
    //         AllItemcount = AllItem.Count;
    // }

    //商人切換商品
    public void merchantSwitchItem()
    {
        //避免數量溢出
        if (sellLength > AllItem.Count)
        {
            sellLength = AllItem.Count;
        }


        itemIndex = merchantShop.merchantShop_.refresh_product(sellLength, itemIndex, AllItem.Count);
    }

    public bool interaction()//互動
    {//如果是商人模式且被玩家觸碰且商店沒被開啟時觸發。
        if (merchantState_ == merchantState.shop && isToch)
        {
            merchantShop.merchantShop_.OpenShopPanel(AllItem, itemIndex);
            return true;
        }
        return false;
    }

    #region Set
    //設置isToch數值
    public void SetIsTouch(bool isToch)
    {
        this.isToch = isToch;
    }
    #endregion

    #region Get
    //回傳是否刷新
    // public bool GetIsTouch()
    // {
    //     return isToch;
    // }
    #endregion
}



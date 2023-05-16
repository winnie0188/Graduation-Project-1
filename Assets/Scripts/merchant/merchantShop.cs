using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

public class merchantShop : UIinit
{
    // Start is called before the first frame update
    #region  Slot
    [SerializeField] int slotCount;//格子數量
    [SerializeField] Transform slotContent;//格子放置位置
    [SerializeField] GameObject slotPrefab;//格子預置物件
    #endregion

    #region Variable
    public static merchantShop merchantShop_;//唯一
    List<merchantItem> AllItem;//全部商品
    public List<merchant> merchants;//所有商人
    [SerializeField] Transform BUYui;//購買UI
    #endregion

    #region Temp
    int[] tempIndex;//儲存itemIndex
    int tempprice;//商品價格
    int tempId;//儲存index

    #endregion

    #region ShopMoney Basic

    [SerializeField] int Playerprice;//玩家的錢
    [SerializeField] Text PlayerpriceTxt;//玩家的錢渲染

    #endregion


    #region DEMO
    [SerializeField] Text text;
    #endregion


    [Header("商人是否旋轉")]
    [SerializeField] bool mIsRotation;


    private void Awake()
    {
        merchantShop_ = this;

        // init slot
        initSlot(slotCount, slotPrefab, slotContent, 100.0f, -1);

        //DEMO
        examineISrefresh();

        // 更新玩家錢錢渲染
        UpdatePlayerprice();

    }


    //DEMO
    public void examineISrefresh()
    {
        for (int i = 0; i < merchants.Count; i++)
        {
            merchants[i].merchantSwitchItem();

        }
    }


    #region 將商人旋轉到跟玩家一樣角度

    public void Rotation_merchant()
    {
        mIsRotation = true;
        StartCoroutine(RotationMerchant());
    }
    IEnumerator RotationMerchant()
    {
        while (mIsRotation)
        {
            for (int i = 0; i < merchants.Count; i++)
            {
                merchants[i].transform.GetChild(0).localRotation = playerController.playerController_.transform.localRotation;
            }
            yield return null;
        }
    }

    // 如果玩家旋轉視角則設置MisRotation
    public void SetMisRotation(bool MisRotation)
    {
        mIsRotation = MisRotation;
    }

    #endregion


    #region SwitchItem
    //刷新商品
    public int[] refresh_product(int itemCount, int[] itemIndex, int maxIndex)
    {
        itemIndex = new int[itemCount];

        for (int i = 0; i < itemCount; i++)
        {//亂數產生，亂數產生的範圍是0~每次販賣的商品數量
            itemIndex[i] = Random.Range(0, maxIndex);

            for (int j = 0; j < i; j++)
            {//檢查是否與前面產生的數值發生重複，如果有就重新產生
                while (itemIndex[j] == itemIndex[i])
                {//如有重複，將變數j設為0，再次檢查 (因為還是有重複的可能)
                    j = 0;
                    itemIndex[i] = Random.Range(0, maxIndex);   //亂數產生，亂數產生的範圍是0~每次販賣的商品數量
                }
            }
        }
        return itemIndex;
    }
    #endregion

    #region OpenShop
    public void keydownOpenShopPanel()
    {
        KeyCode OpenShop = playerController.playerController_.playerKeyCodes.OpenShop;


        TriggerCalculate();//判斷商人是否碰到玩家

        for (int i = merchants.Count - 1; i >= 0; i--)
        {
            if (merchants[i].interaction())
            {
                break;
            }
        }
    }

    //判斷商人是否碰到玩家
    void TriggerCalculate()
    {
        // // DOTS
        int count = merchants.Count;
        if (count > 0)
        {
            NativeArray<merchantTrigger> nativemerchants = new NativeArray<merchantTrigger>(merchants.Count, Allocator.TempJob);


            for (int i = 0; i < count; i++)
            {
                nativemerchants[i] = new merchantTrigger(playerController.playerController_.transform, merchants[i].transform);
            }

            merchantTriggerJob merchantTrigger_job = new merchantTriggerJob();
            merchantTrigger_job.merchantTrigger = nativemerchants;
            JobHandle jobHandle = merchantTrigger_job.Schedule(count, 10);
            jobHandle.Complete();

            for (int i = count - 1; i >= 0; i--)
            {
                merchants[i].SetIsTouch(nativemerchants[i].isToch);

                if (nativemerchants[i].isToch == true)
                {
                    break;
                }

            }

            nativemerchants.Dispose();

        }
    }

    //打開商店panel
    public void OpenShopPanel(List<merchantItem> merchantItems, int[] itemIndex)
    {
        if (AllItem != merchantItems)
        {
            AllItem = merchantItems;
            UI_loading(itemIndex.Length, itemIndex);
        }

        PanelManage.panelManage.panels.shopPanel.gameObject.SetActive(true);

        slotContent.position -= new Vector3(0, 1000, 0);

    }


    //UI加載
    public void UI_loading(int itemCount, int[] itemIndex)
    {
        for (int i = 0; i < slotContent.childCount; i++)
        {
            if (i < itemCount)
            {
                // 渲染開啟
                slotContent.GetChild(i).gameObject.SetActive(true);

                var AllItem_i = AllItem[itemIndex[i]];
                //商品名稱
                slotContent.GetChild(i).GetChild(2).GetComponent<Text>().text = AllItem_i.merchantItem_BagItem.BagItem_name;
                //商品圖片
                slotContent.GetChild(i).GetChild(0).GetComponent<Image>().sprite = AllItem_i.merchantItem_BagItem.BagItem_icon;//改變SLOT裡的icon
                                                                                                                               //商品價格
                slotContent.GetChild(i).GetChild(1).GetComponent<Text>().text = "$" + AllItem_i.price.ToString();
            }
            //溢出處理
            else if (!slotContent.GetChild(i).gameObject.activeSelf)
            {
                break;
            }
            else
            {
                // 渲染關閉
                slotContent.GetChild(i).gameObject.SetActive(false);
            }
        }

        tempIndex = itemIndex;
        for (int i = 0; i < itemCount; i++)
        {
            var AllItem_i = AllItem[itemIndex[i]];
            //商品名稱
            slotContent.GetChild(i).GetChild(2).GetComponent<Text>().text = AllItem_i.merchantItem_BagItem.BagItem_name;
            //商品圖片
            slotContent.GetChild(i).GetChild(0).GetComponent<Image>().sprite = AllItem_i.merchantItem_BagItem.BagItem_icon;//改變SLOT裡的icon
            //商品價格
            slotContent.GetChild(i).GetChild(1).GetComponent<Text>().text = "$" + AllItem_i.price.ToString();
        }

    }

    #endregion

    #region  BUY
    //更換索引購買
    public override void slot_event(int index)
    {

        var AllItem_index = AllItem[tempIndex[index]];

        BUYui.gameObject.SetActive(true);
        //商品圖片
        BUYui.GetChild(0).GetComponent<Image>().sprite = AllItem_index.merchantItem_BagItem.BagItem_icon;
        //商品名稱
        BUYui.GetChild(1).GetComponent<Text>().text = AllItem_index.merchantItem_BagItem.BagItem_name;
        //商品敘述
        BUYui.GetChild(2).GetComponent<Text>().text = AllItem_index.merchantItem_BagItem.BagItem_info;
        //商品價格
        tempprice = AllItem_index.price;
        tempId = index;
        BUYui.GetChild(3).GetComponent<Text>().text = tempprice.ToString();

    }

    //確定購買
    public void Buy_down()
    {
        var All_index_merchantItem_BagIem = AllItem[tempIndex[tempId]].merchantItem_BagItem;

        if (Playerprice - tempprice >= 0)
        {
            //扣錢
            Playerprice -= tempprice;

            BagManage.bagManage.checkItem(All_index_merchantItem_BagIem, 1);

            text.text = "購買成功";

            // 購滿成功則
            UpdatePlayerprice();
        }
        else
        {
            text.text = "錢不夠";
            print("錢不夠");
        }
    }

    public void UpdatePlayerprice()
    {
        PlayerpriceTxt.text = Playerprice + "";
    }


    //商店關閉
    public void Buy_close()
    {
        BUYui.gameObject.SetActive(false);
    }

    #endregion
}

#region Trigger
public struct merchantTrigger
{
    Unity.Mathematics.float3 player;
    Unity.Mathematics.float3 transform;

    public bool isToch;


    public merchantTrigger(Transform player, Transform transform)
    {
        this.player = player.position;
        this.transform = transform.position;
        isToch = false;

    }
    public void CalculateUpdate()
    {

        if (Mathf.Abs(player.x - transform.x) < 3f && Mathf.Abs(player.y - transform.y) < 3f && Mathf.Abs(player.z - transform.z) < 1.5f)
        {
            isToch = true;
        }
        else
        {
            isToch = false;
        }

    }
}

public struct merchantTriggerJob : IJobParallelFor
{
    public NativeArray<merchantTrigger> merchantTrigger;

    public void Execute(int index)
    {
        var data = merchantTrigger[index];
        data.CalculateUpdate();
        merchantTrigger[index] = data;
    }
}
#endregion
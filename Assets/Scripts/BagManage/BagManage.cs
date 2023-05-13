using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BagManage : UIinit
{
    public static BagManage bagManage;
    public BagSore[] bagSore;


    #region init_Slot
    [SerializeField] int slotCount;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform slotContentParent;

    //背包UI 
    [SerializeField] Transform BagInfo;

    // 分類
    [SerializeField] Transform category;


    #endregion

    #region HotKey

    bool isHotKeyEditor;
    // 判斷現在是哪個HotKey
    int HotKeyID;

    [SerializeField] HotKeyStore hotKeyStore;

    // 熱鍵panel
    [SerializeField] Transform BasicHotKeyPanel;
    [SerializeField] Transform PotionHotKeyPanel;
    [SerializeField] Transform CloteHotKeyPanel;
    [SerializeField] Transform EquipHotKeyPanel;

    #endregion

    #region temp

    //slotContentParent
    ScrollRect scrollRect;
    //新增或刪除物件時，對應了哪個背包
    int bagSoreIndex;
    //每頁已經置頂過了
    bool[] isUpperReady;
    // 當前背包索引
    int BagIndex;
    // 當前道具索引
    int itemIndex;

    #endregion

    #region Drag
    public Transform DragItem;
    #endregion

    private void Awake()
    {
        bagManage = this;

        isUpperReady = new bool[slotContentParent.childCount];
        scrollRect = slotContentParent.GetComponent<ScrollRect>();

        // 熱鍵數量
        hotKeyStore.HotKeys = new HotKey[BasicHotKeyPanel.childCount];
        for (int i = 0; i < hotKeyStore.HotKeys.Length; i++)
        {
            hotKeyStore.HotKeys[i] = new HotKey();
            StartCoroutine(HotKeyAddListener(BasicHotKeyPanel.GetChild(i).GetComponent<Button>(), i, BasicHotKeyPanel, hotKeyStore.HotKeys));
        }

        //藥水熱鍵
        hotKeyStore.HotKeys_potion = new HotKey[PotionHotKeyPanel.childCount];
        for (int i = 0; i < hotKeyStore.HotKeys_potion.Length; i++)
        {
            hotKeyStore.HotKeys_potion[i] = new HotKey();
            StartCoroutine(HotKeyAddListener(PotionHotKeyPanel.GetChild(i).GetComponent<Button>(), i, PotionHotKeyPanel, hotKeyStore.HotKeys_potion));
        }

        //衣服熱鍵
        hotKeyStore.HotKeys_Clothe = new HotKey[CloteHotKeyPanel.childCount];
        for (int i = 0; i < hotKeyStore.HotKeys_Clothe.Length; i++)
        {
            hotKeyStore.HotKeys_Clothe[i] = new HotKey();
            StartCoroutine(HotKeyAddListener(CloteHotKeyPanel.GetChild(i).GetComponent<Button>(), i, CloteHotKeyPanel, hotKeyStore.HotKeys_Clothe));
        }

        // 裝備熱鍵
        hotKeyStore.HotKeys_equip = new HotKey[EquipHotKeyPanel.childCount];
        for (int i = 0; i < hotKeyStore.HotKeys_equip.Length; i++)
        {
            hotKeyStore.HotKeys_equip[i] = new HotKey();
            StartCoroutine(HotKeyAddListener(EquipHotKeyPanel.GetChild(i).GetComponent<Button>(), i, EquipHotKeyPanel, hotKeyStore.HotKeys_equip));
        }


        //slot初始化
        for (int i = 0; i < slotContentParent.childCount; i++)
        {
            initSlot(slotCount, slotPrefab, slotContentParent.GetChild(i), 50);
        }

        // category初始化
        for (int i = 0; i < category.childCount; i++)
        {
            StartCoroutine(categoryAddListener(category.GetChild(i).GetComponent<Button>(), i));
        }


        //DEMO
        initBagUI();

    }

    IEnumerator categoryAddListener(Button btn, int i)
    {
        btn.name = "" + i;
        btn.onClick.AddListener(() => switch_category(i));
        yield return null;
    }

    IEnumerator HotKeyAddListener(Button btn, int i, Transform KeyPanel, HotKey[] hotKeys)
    {
        btn.name = "" + i;
        btn.onClick.AddListener(() => HotKey(i, hotKeys, KeyPanel));
        yield return null;
    }




    //切換分類，記得要拉到按鈕上
    #region switch category
    public void switch_category(int id)
    {
        if (BagInfo.gameObject.activeSelf)
            return;

        for (int i = 0; i < slotContentParent.childCount; i++)
        {
            if (i == id)
            {
                scrollRect.content = slotContentParent.GetChild(i).GetComponent<RectTransform>();
                // 打開當前分類
                slotContentParent.GetChild(i).gameObject.SetActive(true);
                if (isUpperReady[i] == false)
                {
                    isUpperReady[i] = true;
                    // 延遲置頂
                    StartCoroutine(ContentcomeBack(i));
                }

            }
            else
            {
                //關閉當前分類
                slotContentParent.GetChild(i).gameObject.SetActive(false);
            }
        }


        // 判斷現在是什麼背包
        BagIndex = id;
    }

    //內容置頂
    IEnumerator ContentcomeBack(int i)
    {
        yield return null;
        // 這樣才能拖拉
        slotContentParent.GetChild(i).position -= new Vector3(0, 250, 0);
    }
    #endregion

    #region add/delete new obj in Bag
    // 新增物品到背包
    // amount是要增加或減少的數量
    public void checkItem(BagItem newBagIem, int amount)
    {
        //判斷道具種類並放到該類型的背包裏面
        bagSoreIndex = 0;

        switch (newBagIem.BagItemType_)
        {
            case BagItem.BagItemType.potion:
                bagSoreIndex = 0;
                break;
            case BagItem.BagItemType.equipment:
                bagSoreIndex = 1;
                break;
            case BagItem.BagItemType.clothe:
                bagSoreIndex = 2;
                break;
            case BagItem.BagItemType.material:
                bagSoreIndex = 3;
                break;
            case BagItem.BagItemType.food:
                bagSoreIndex = 4;
                break;
            case BagItem.BagItemType.tool:
                bagSoreIndex = 5;
                break;
            case BagItem.BagItemType.block:
                bagSoreIndex = 6;
                break;
        }


        // 更新背包UI
        if (amount > 0)
        {
            PutItemInBag(bagSore[bagSoreIndex], newBagIem, amount);
        }
        else if (amount < 0)
        {
            deleteItemInBag(bagSore[bagSoreIndex], newBagIem, amount);
        }
    }

    //將商品放入背包
    public void PutItemInBag(BagSore BagSore, BagItem NewBagItem, int amount)
    {
        var BagSore_BagItems = BagSore.BagItems;
        int index = BagSore_BagItems.Count;

        //找尋該背包是否有該道具
        if (BagSore_BagItems.Contains(NewBagItem))
        {
            //找到該位置增加count
            for (int i = 0; i < BagSore_BagItems.Count; i++)
            {
                if (BagSore_BagItems[i].Equals(NewBagItem))
                {
                    // 要互動的道具索引
                    index = i;
                    // 
                    BagSore.ItemCount[i] += amount;
                    break;
                }
            }
        }
        else
        {
            //直接加新的道具到背包
            BagSore.BagItems.Add(NewBagItem);
            BagSore.ItemCount.Add(amount);
            BagSore.isWear.Add(false);
        }

        //bagSore[bagSoreIndex] = BagSore;

        Update_BagUI(bagSore[bagSoreIndex], index, true);
    }

    //將商品移出背包
    public void deleteItemInBag(BagSore BagSore, BagItem NewBagItem, int amount)
    {
        var BagSore_BagItems = BagSore.BagItems;

        // 什麼事都沒發生傳-1
        int index = -1;

        //找尋該背包是否有該道具
        if (BagSore_BagItems.Contains(NewBagItem))
        {
            //找到該位置增加count
            for (int i = 0; i < BagSore_BagItems.Count; i++)
            {
                if (BagSore_BagItems[i].Equals(NewBagItem))
                {
                    // 要互動的道具索引
                    index = i;
                    // 

                    if (BagSore.ItemCount[i] + amount > 0)
                    {

                        BagSore.ItemCount[i] += amount;

                        Update_BagUI(bagSore[bagSoreIndex], index, true);
                    }
                    else
                    {
                        BagSore.BagItems.RemoveAt(i);
                        BagSore.ItemCount.RemoveAt(i);
                        BagSore.isWear.RemoveAt(i);

                        Update_BagUI(bagSore[bagSoreIndex], index, false);
                    }


                    break;
                }
            }
        }

    }

    #endregion

    // 點選slot顯示以及快捷鍵資訊
    #region indexSlot
    public override void slot_event(int i)
    {

        // 如果溢出則...
        if (bagSore[BagIndex].BagItems.Count <= i)
        {
            return;
        }


        //如果需樣wear介面就取消註解*****************************************************************************************
        if (!DragItem.gameObject.activeSelf)
        {
            var bagSore_BagIndex_BagItems_i = bagSore[BagIndex].BagItems[i];

            BagInfo.gameObject.SetActive(true);

            // icon
            BagInfo.GetChild(0).GetComponent<Image>().sprite = bagSore_BagIndex_BagItems_i.BagItem_icon;

            //名稱 
            BagInfo.GetChild(1).GetComponent<Text>().text = bagSore_BagIndex_BagItems_i.BagItem_name;

            // 介紹
            BagInfo.GetChild(2).GetComponent<Text>().text = bagSore_BagIndex_BagItems_i.BagItem_info;
        }
        //*********************************************************************************************************


        //當前item索引 
        itemIndex = i;


        //如果需樣wear介面就刪掉*****************************************************************************************
        if (DragItem.gameObject.activeSelf)
            wear();

        //*********************************************************************************************************
    }


    // 裝備鍵
    public void wear()
    {
        // 關閉編輯
        isHotKeyEditor = false;

        // 
        var bagSore_BagIndex_BagItems_i = bagSore[BagIndex].BagItems[itemIndex];
        switch (bagSore_BagIndex_BagItems_i.BagItemType_)
        {
            case BagItem.BagItemType.potion:
                isHotKeyEditor = true;
                HotKeyID = 0;
                break;
            case BagItem.BagItemType.equipment:
                isHotKeyEditor = true;
                HotKeyID = 1;
                break;
            case BagItem.BagItemType.clothe:
                isHotKeyEditor = true;
                HotKeyID = 2;
                break;
            case BagItem.BagItemType.material:
                HotKeyID = 3;
                break;
            case BagItem.BagItemType.food:
                HotKeyID = 4;
                break;
            case BagItem.BagItemType.tool:
                HotKeyID = 5;
                break;
            case BagItem.BagItemType.block:
                HotKeyID = 6;
                isHotKeyEditor = true;
                break;

        }
        hiddenBagInfo();
    }

    // 關閉BagInfo資訊
    public void hiddenBagInfo()
    {
        BagInfo.gameObject.SetActive(false);
    }

    // 判斷服裝位置是否正確
    int canWearClothe(clothe.clotheType clotheType)
    {
        if (clotheType == clothe.clotheType.hat)
        {
            return 0;
        }
        else if (clotheType == clothe.clotheType.clothe)
        {
            return 1;
        }
        else if (clotheType == clothe.clotheType.pants)
        {
            return 2;
        }
        else
        {
            return -1;
        }
    }


    // 選擇快捷鍵(索引)
    public void HotKey(int i, HotKey[] HotKeys, Transform KeyPanel)
    {

        // 進入編輯模式
        if (isHotKeyEditor)
        {
            if (HotKeyID == 0 && KeyPanel != PotionHotKeyPanel)
            {
                return;
            }
            else if (HotKeyID == 1 && KeyPanel != EquipHotKeyPanel)
            {
                return;
            }
            else if (HotKeyID == 2)
            {
                if (KeyPanel != CloteHotKeyPanel)
                {
                    return;
                }
                // 確保每個部位穿正確的衣服
                if (canWearClothe(bagSore[BagIndex].BagItems[itemIndex].clothe.clotheType_) != i)
                {
                    return;
                }
            }
            else if (HotKeyID == 6 && KeyPanel != BasicHotKeyPanel)
            {
                return;
            }

            var item = bagSore[BagIndex].BagItems[itemIndex];



            // 已經裝備過
            if (bagSore[BagIndex].isWear[itemIndex])
            {
                for (int j = 0; j < HotKeys.Length; j++)
                {

                    // 判斷自己原本裝備的位置
                    if (BagIndex == HotKeys[j].HotKey_Bag && itemIndex == HotKeys[j].HotKey_item)
                    {
                        // 判斷新位置是否有東西
                        if (HotKeys[i].HotKey_Bag != -1)
                        {
                            HotKeys[j].HotKey_Bag = HotKeys[i].HotKey_Bag;
                            HotKeys[j].HotKey_item = HotKeys[i].HotKey_item;
                        }
                        else
                        {
                            HotKeys[j].HotKey_Bag = -1;
                            HotKeys[j].HotKey_item = -1;
                        }
                        break;
                    }
                }
            }

            // 沒裝備過則
            else
            {
                bagSore[BagIndex].isWear[itemIndex] = true;
            }


            HotKeys[i].HotKey_Bag = BagIndex;
            HotKeys[i].HotKey_item = itemIndex;


            isHotKeyEditor = false;

            // 刷新
            Refresh_HotKey();
        }

        //沒有進入編輯模式，則使用
        else
        {

        }
    }

    #endregion

    #region 刷新快捷鍵渲染
    void Refresh_HotKey()
    {
        Transform HotKey = PanelManage.panelManage.panels.HotKeyPanel;

        Transform BasicKey = HotKey.GetChild(1);
        Transform PotionKey = HotKey.GetChild(0);

        UpdateHotKeyPanel(BasicKey, hotKeyStore.HotKeys);
        UpdateHotKeyPanel(PotionKey, hotKeyStore.HotKeys_potion);

        UpdateHotKeyPanel(BasicHotKeyPanel, hotKeyStore.HotKeys);
        UpdateHotKeyPanel(PotionHotKeyPanel, hotKeyStore.HotKeys_potion);
        UpdateHotKeyPanel(CloteHotKeyPanel, hotKeyStore.HotKeys_Clothe);
        UpdateHotKeyPanel(EquipHotKeyPanel, hotKeyStore.HotKeys_equip);

    }

    private void UpdateHotKeyPanel(Transform hotKeyPanel, HotKey[] hotKeys)
    {
        for (int i = 0; i < hotKeys.Length; i++)
        {
            Transform hotKeyItem = hotKeyPanel.GetChild(i);
            if (hotKeys[i].HotKey_Bag != -1)
            {
                hotKeyItem.GetChild(0).gameObject.SetActive(true);
                hotKeyItem.GetChild(0).GetComponent<Image>().sprite = bagSore[hotKeys[i].HotKey_Bag].BagItems[hotKeys[i].HotKey_item].BagItem_icon;
            }
            else
            {
                hotKeyItem.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    #endregion


    #region Update_Bag
    //加載紀錄時的背包
    public void initBagUI()
    {
        for (int i = 0; i < bagSore.Length; i++)
        {
            var bagSore_i_BagItems_Count = bagSore[i].BagItems.Count;

            for (int j = 0; j < bagSore_i_BagItems_Count; j++)
            {
                if (j < bagSore_i_BagItems_Count)
                {
                    slotContentParent.GetChild(i).GetChild(j).gameObject.SetActive(true);
                    //道具圖片//改變SLOT裡的icon
                    slotContentParent.GetChild(i).GetChild(j).GetChild(0).GetComponent<Image>().sprite = bagSore[i].BagItems[j].BagItem_icon;
                    //道具名字
                    slotContentParent.GetChild(i).GetChild(j).GetChild(1).GetComponent<Text>().text = bagSore[i].BagItems[j].BagItem_name;
                    //道具數量
                    slotContentParent.GetChild(i).GetChild(j).GetChild(2).GetComponent<Text>().text = bagSore[i].ItemCount[j].ToString();
                }
            }
        }
    }

    public void defaultSlot(int index)
    {
        //道具圖片//改變SLOT裡的icon
        slotContentParent.GetChild(bagSoreIndex).GetChild(index).GetChild(0).GetComponent<Image>().sprite = null;
        //道具名字
        slotContentParent.GetChild(bagSoreIndex).GetChild(index).GetChild(1).GetComponent<Text>().text = "道具名";
        //道具數量
        slotContentParent.GetChild(bagSoreIndex).GetChild(index).GetChild(2).GetComponent<Text>().text = "數量";
    }

    // isAdd=>true 增加資料 isAdd=>刪除資料
    public void Update_BagUI(BagSore BagSore, int index, bool isAdd)
    {
        if (isAdd)
        {
            //道具圖片//改變SLOT裡的icon
            slotContentParent.GetChild(bagSoreIndex).GetChild(index).GetChild(0).GetComponent<Image>().sprite = BagSore.BagItems[index].BagItem_icon;
            //道具名字
            slotContentParent.GetChild(bagSoreIndex).GetChild(index).GetChild(1).GetComponent<Text>().text = BagSore.BagItems[index].BagItem_name;
            //道具數量
            slotContentParent.GetChild(bagSoreIndex).GetChild(index).GetChild(2).GetComponent<Text>().text = BagSore.ItemCount[index].ToString();
        }
        else
        {
            //從index包含以後的物件全部重新加載
            if (BagSore.BagItems.Count == index)
            {
                defaultSlot(index);
            }
            else
            {
                for (int i = index; i < BagSore.BagItems.Count + 1; i++)
                {
                    if (i == BagSore.BagItems.Count)
                    {
                        defaultSlot(i);
                        break;
                    }
                    //道具圖片//改變SLOT裡的icon
                    slotContentParent.GetChild(bagSoreIndex).GetChild(i).GetChild(0).GetComponent<Image>().sprite = BagSore.BagItems[i].BagItem_icon;
                    //道具名字
                    slotContentParent.GetChild(bagSoreIndex).GetChild(i).GetChild(1).GetComponent<Text>().text = BagSore.BagItems[i].BagItem_name;
                    //道具數量
                    slotContentParent.GetChild(bagSoreIndex).GetChild(i).GetChild(2).GetComponent<Text>().text = BagSore.ItemCount[i].ToString();
                }
            }



            // 刷新快捷鍵對應關係
            Refresh_HotKeyStore(hotKeyStore.HotKeys, index);
            Refresh_HotKeyStore(hotKeyStore.HotKeys_Clothe, index);
            Refresh_HotKeyStore(hotKeyStore.HotKeys_equip, index);
            Refresh_HotKeyStore(hotKeyStore.HotKeys_potion, index);
            // 刷新快捷鍵渲染
            Refresh_HotKey();
        }
    }
    // 刷新快捷鍵對應關係
    public void Refresh_HotKeyStore(HotKey[] hotKeys, int index)
    {
        for (int j = 0; j < hotKeys.Length; j++)
        {
            if (hotKeys[j].HotKey_Bag == bagSoreIndex)
            {
                if (hotKeys[j].HotKey_item > index)
                {
                    hotKeys[j].HotKey_item -= 1;
                }
                else if (hotKeys[j].HotKey_item == index)
                {
                    hotKeys[j].HotKey_item = -1;
                    hotKeys[j].HotKey_Bag = -1;
                }
            }
        }
    }

    #endregion

    #region Drag
    // 
    public void initDrag(Transform newDrag)
    {
        // 初始化Drag物件
        DragItem.gameObject.SetActive(true);

        DragItem.GetComponent<Image>().sprite = newDrag.GetChild(0).GetComponent<Image>().sprite;

        newDrag.GetChild(0).GetComponent<Image>().color -= new Color(0, 0, 0, 1);

        // 觸發編輯事件
        newDrag.GetComponent<Button>().onClick.Invoke();

        StartCoroutine(DragCheck(newDrag));
    }

    // 檢查drag是否有問題
    IEnumerator DragCheck(Transform newDrag)
    {
        Vector3 prePos = new Vector3(0, 0, 0);
        Vector3 nowPos = DragItem.position;
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            if (prePos != nowPos)
            {
                prePos = nowPos;
                nowPos = DragItem.position;
            }
            else
            {
                break;
            }
        }

        newDrag.GetChild(0).GetComponent<Image>().color += new Color(0, 0, 0, 1);
        newDrag.GetComponent<CanvasGroup>().blocksRaycasts = true;
        DragItem.gameObject.SetActive(false);
    }



    // 放開後會發生的事件(拖曳，碰撞到的)
    public void DragEnd(Transform newDrag, Transform EditDragSlot, bool isToch)
    {
        //Image newDragImg = newDrag.GetComponent<Image>();
        newDrag.GetChild(0).GetComponent<Image>().color += new Color(0, 0, 0, 1);

        if (isToch)
        {
            EditDragSlot.GetComponent<Button>().onClick.Invoke();
        }
    }



    #endregion
}

[System.Serializable]
public class BagSore
{
    //背包名
    public string BagName;

    //背包存放
    public List<BagItem> BagItems;

    //每個道具數量
    public List<int> ItemCount;

    //是否已裝備 
    public List<bool> isWear;

}

[System.Serializable]
public class HotKey
{
    public int HotKey_Bag = -1;
    public int HotKey_item = -1;
}

[System.Serializable]
public class HotKeyStore
{
    // 熱鍵物資儲存
    // 一般的熱鍵
    public HotKey[] HotKeys;
    //藥水熱鍵
    public HotKey[] HotKeys_potion;
    //衣服熱鍵
    public HotKey[] HotKeys_Clothe;
    public HotKey[] HotKeys_equip;
}
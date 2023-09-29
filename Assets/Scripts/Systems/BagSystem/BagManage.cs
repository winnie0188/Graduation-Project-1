using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class BagManage : UIinit
{
    public static BagManage bagManage;
    public BagSore[] bagSore;


    #region init_Slot
    [SerializeField] int slotCount;
    [SerializeField] GameObject slotPrefab;
    public Transform slotContentParent;

    //背包UI 
    [SerializeField] Transform BagInfo;

    // 分類
    [SerializeField] Transform category;
    [SerializeField] Image CategoryTitle;
    [SerializeField] Sprite[] Title;
    // 

    #endregion

    #region HotKey



    public HotKeyStore hotKeyStore;

    // 熱鍵panel
    [SerializeField] Transform BasicHotKeyPanel;
    [SerializeField] Transform PotionHotKeyPanel;
    [SerializeField] Transform CloteHotKeyPanel;
    [SerializeField] Transform EquipHotKeyPanel;

    #endregion

    #region temp

    //slotContentParent
    public ScrollRect scrollRect;

    //當前背包索引:切分頁需要
    int bagSoreIndex = 9;


    // 背包索引:快捷鍵需要
    int BagIndex;

    // 當前道具索引
    int itemIndex;

    #endregion

    #region Drag
    public Transform DragItem;
    KeyDrag keyDrag;
    public DragState HotKeyState = DragState.WAIT;
    #endregion


    private void Awake()
    {
        bagManage = this;
        keyDrag = FindObjectOfType<KeyDrag>();
        InitBag();
    }


    void InitBag()
    {
        scrollRect = slotContentParent.GetComponent<ScrollRect>();


        // 熱鍵初始化----------------------------------------------
        hotKeyStore.HotKeys = InitializeHotKeys(BasicHotKeyPanel.childCount, BasicHotKeyPanel);
        hotKeyStore.HotKeys_potion = InitializeHotKeys(PotionHotKeyPanel.childCount, PotionHotKeyPanel);
        hotKeyStore.HotKeys_Clothe = InitializeHotKeys(CloteHotKeyPanel.childCount, CloteHotKeyPanel);
        hotKeyStore.HotKeys_equip = InitializeHotKeys(EquipHotKeyPanel.childCount, EquipHotKeyPanel);

        HotKey[] InitializeHotKeys(int count, Transform panel)
        {
            HotKey[] hotKeys = new HotKey[count];
            for (int i = 0; i < count; i++)
            {
                hotKeys[i] = new HotKey();
                StartCoroutine(HotKeyAddListener(panel.GetChild(i).GetComponent<Button>(), i, panel, hotKeys));
            }
            return hotKeys;
        }
        // 熱鍵初始化----------------------------------------------




        PanelManage panelManage = FindObjectOfType<PanelManage>();

        int Allcount = 0;
        bool isCreate = panelManage.getIsCreateMode();
        BagItemStore bagItemStore = StoreSetting.storeSetting.GetBagItemStore();

        for (int i = 0; i < slotContentParent.childCount; i++)
        {
            if (i != slotContentParent.childCount - 3 && i < slotContentParent.childCount - 1)
            {
                if (isCreate)
                {
                    Allcount += bagItemStore.bagStoreIndex[i];
                    initSlot(
                        bagItemStore.bagStoreIndex[i],
                        slotPrefab,
                        slotContentParent.GetChild(i),
                        1, -1
                    );
                }
                else
                {
                    Allcount += slotCount;
                    initSlot(slotCount, slotPrefab, slotContentParent.GetChild(i), 1, -1);
                }

            }
            else if (i == slotContentParent.childCount - 1)
            {
                initSlot(Allcount, slotPrefab, slotContentParent.GetChild(i), 1, -1);
            }
        }

        // category初始化
        for (int i = 0; i < category.childCount; i++)
        {
            StartCoroutine(categoryAddListener(category.GetChild(i).GetComponent<Button>(), i));
        }

        initBagUI();

        if (isCreate)
        {
            StartCoroutine(setCreaterItem());
        }

    }
    #region 創作者模式
    IEnumerator setCreaterItem()
    {
        BagItemStore bagItemStore = StoreSetting.storeSetting.GetBagItemStore();
        int count = bagItemStore.BagItems.Length;
        //Transform Alltag = slotContentParent.GetChild(slotContentParent.childCount - 1);

        yield return null;

        for (int i = 0; i < count; i++)
        {
            checkItem(
                bagItemStore.BagItems[i],
                1000,
                false,
                false
            );
        }
        Update_AllBag();
    }
    #endregion

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



    //切換分類
    // ***********************************************************記得層級要child符合
    #region switch category
    public void switch_category(int id)
    {

        if (BagInfo.gameObject.activeSelf)
            return;

        for (int i = 0; i < slotContentParent.childCount; i++)
        {
            if (i == id)
            {
                // 更改標題
                CategoryTitle.sprite = Title[i];

                Transform target = slotContentParent.GetChild(i);

                if (target.name == "任務")
                {
                    scrollRect.content = target.GetChild(0).GetComponent<RectTransform>();
                    if (taskSystem.isReset == false)
                    {
                        StartCoroutine(taskSystem.taskSystem_.Reset_Panel());
                    }
                    else
                    {
                        // 打開當前分類
                        slotContentParent.GetChild(i).gameObject.SetActive(true);
                    }
                }
                else
                {
                    scrollRect.content = target.GetComponent<RectTransform>();
                    // 打開當前分類
                    slotContentParent.GetChild(i).gameObject.SetActive(true);
                }



                // 標籤動畫
                category.GetChild(i).GetChild(0).DOMove(new Vector3(10, 0, 0) + category.GetChild(i).position, 0.5f);
            }
            else
            {
                // 標籤動畫
                category.GetChild(i).GetChild(0).DOMove(new Vector3(0, 0, 0) + category.GetChild(i).position, 0.5f);
                //關閉當前分類
                slotContentParent.GetChild(i).gameObject.SetActive(false);
            }
        }


        // 判斷現在是什麼背包
        bagSoreIndex = id;
    }


    #endregion

    #region add/delete new obj in Bag
    // 新增物品到背包
    // amount是要增加或減少的數量
    // iswear是為了存檔
    public void checkItem(BagItem newBagIem, int amount, bool iswear, bool isUpdateUi)
    {
        //判斷道具種類並放到該類型的背包裏面

        // 更新背包UI
        if (amount > 0)
        {
            PutItemInBag(bagSore[newBagIem.bagSoreIndex], newBagIem, amount, iswear, isUpdateUi);
        }
        else if (amount < 0)
        {
            deleteItemInBag(bagSore[newBagIem.bagSoreIndex], newBagIem, amount, isUpdateUi);
        }
    }

    //將商品放入背包
    public void PutItemInBag(BagSore BagSore, BagItem NewBagItem, int amount, bool iswear, bool isUpdateUi)
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
            BagSore.isWear.Add(iswear);
        }

        //bagSore[bagSoreIndex] = BagSore;
        if (isUpdateUi)
            Update_BagUI(NewBagItem.bagSoreIndex, index, true);
    }

    //將商品移出背包
    public void deleteItemInBag(BagSore BagSore, BagItem NewBagItem, int amount, bool isUpdateUi)
    {
        int index = findIndex(BagSore, NewBagItem);
        if (index != -1)
        {
            if (BagSore.ItemCount[index] + amount > 0)
            {

                BagSore.ItemCount[index] += amount;

                if (isUpdateUi)
                    Update_BagUI(NewBagItem.bagSoreIndex, index, true);
            }
            else
            {
                BagSore.BagItems.RemoveAt(index);
                BagSore.ItemCount.RemoveAt(index);
                BagSore.isWear.RemoveAt(index);

                if (isUpdateUi)
                    Update_BagUI(NewBagItem.bagSoreIndex, index, false);
            }
        }



    }

    //找尋該道具在哪裡
    public int findIndex(BagSore BagSore, BagItem NewBagItem)
    {
        var BagSore_BagItems = BagSore.BagItems;
        // 什麼事都沒發生傳-1
        int index = -1;
        if (BagSore_BagItems.Contains(NewBagItem))
        {
            //找到該位置增加count
            for (int i = 0; i < BagSore_BagItems.Count; i++)
            {
                if (BagSore_BagItems[i].Equals(NewBagItem))
                {
                    // 要互動的道具索引
                    index = i;
                    break;
                }
            }
        }

        return index;
    }

    #endregion

    // 點選slot顯示以及快捷鍵資訊
    #region indexSlot
    public override void slot_event(int i)
    {
        if (HotKeyState == DragState.WAIT)
        {
            if (bagSoreIndex >= bagSore.Length)
            {
                var Allcategory = slotContentParent.GetChild(bagSore.Length).GetChild(i);

                if (Allcategory.GetChild(1).GetComponent<Text>().text == "道具名")
                {
                    return;
                }
                else
                {
                    var IJindex = Allcategory.GetChild(2).GetComponent<Text>().text.Split('/');

                    BagIndex = int.Parse(IJindex[0]);
                    itemIndex = int.Parse(IJindex[1]);
                }
                print(1);
            }
            // 如果沒溢出則...
            else if (bagSore[bagSoreIndex].BagItems.Count > i)
            {
                BagIndex = bagSoreIndex;
                //當前item索引 
                itemIndex = i;
                print(2);
            }
            else
            {
                return;
            }

            HotKeyState = DragState.BAGITEMDRAG;
            keyDrag.itemdrag(
                DragItem, slotContentParent.GetChild(BagIndex).GetChild(itemIndex)
            );


            //如果需樣wear介面就取消註解*****************************************************************************************
            // if (!DragItem.gameObject.activeSelf)
            // {
            //     var bagSore_BagIndex_BagItems_i = bagSore[BagIndex].BagItems[itemIndex];

            //     BagInfo.gameObject.SetActive(true);

            //     // icon
            //     BagInfo.GetChild(0).GetComponent<Image>().sprite = bagSore_BagIndex_BagItems_i.BagItem_icon;

            //     //名稱 
            //     BagInfo.GetChild(1).GetComponent<Text>().text = bagSore_BagIndex_BagItems_i.BagItem_name;

            //     // 介紹
            //     BagInfo.GetChild(2).GetComponent<Text>().text = bagSore_BagIndex_BagItems_i.BagItem_info;
            // }
            //*********************************************************************************************************


            //如果需樣wear介面就刪掉*****************************************************************************************
            //if (DragItem.gameObject.activeSelf)
            // wear();
        }
        else if (HotKeyState == DragState.HOTKEYDRAG)
        {
            keyDrag.hotdragEnd(0);
        }
        else if (HotKeyState == DragState.BAGITEMDRAG)
        {
            keyDrag.itemdragEnd();
        }
        //*********************************************************************************************************
    }


    // 裝備鍵
    // public void wear()
    // {
    //     HotKeyState = DragState.BAGITEMDRAG;

    // 之後可刪
    //hiddenBagInfo();
    // }

    // 關閉BagInfo資訊
    public void hiddenBagInfo()
    {
        BagInfo.gameObject.SetActive(false);
    }

    // // 判斷服裝位置是否正確
    // int canWearClothe(clothe.clotheType clotheType)
    // {
    //     if (clotheType == clothe.clotheType.hat)
    //     {
    //         return 0;
    //     }
    //     else if (clotheType == clothe.clotheType.clothe)
    //     {
    //         return 1;
    //     }
    //     else if (clotheType == clothe.clotheType.pants)
    //     {
    //         return 2;
    //     }
    //     else
    //     {
    //         return -1;
    //     }
    // }


    // 選擇快捷鍵(索引)
    public void HotKey(int i, HotKey[] HotKeys, Transform KeyPanel)
    {

        // 進入編輯模式
        if (HotKeyState == DragState.BAGITEMDRAG)
        {
            keyDrag.itemdragEnd();
            // 判斷可以裝嗎
            int bagSoreIndex = -1;

            if (KeyPanel == BasicHotKeyPanel)
            {
                bagSoreIndex = 0;
            }
            else if (KeyPanel == PotionHotKeyPanel)
            {
                bagSoreIndex = 1;
            }
            else if (KeyPanel == EquipHotKeyPanel)
            {
                bagSoreIndex = 2;
            }
            else if (KeyPanel == CloteHotKeyPanel)
            {
                bagSoreIndex = 3;
            }

            var item = bagSore[BagIndex].BagItems[itemIndex];

            if (item.keySoreIndex != bagSoreIndex)
            {
                return;
            }
            // 判斷可以裝嗎



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


            // 刷新
            Refresh_HotKey();
            playerController.playerController_.SetBag_Item(playerController.playerController_.GetArm());

        }

        //沒有進入編輯模式，則使用
        else if (HotKeyState == DragState.WAIT)
        {

            if (HotKeys[i].HotKey_Bag != -1)
            {
                HotKeyState = DragState.HOTKEYDRAG;
                keyDrag.hotdrag(
                    DragItem, KeyPanel.GetChild(i), HotKeys[i]
                );
            }
        }
        else if (HotKeyState == DragState.HOTKEYDRAG)
        {

            keyDrag.switchSlot(
                 KeyPanel.GetChild(i), HotKeys[i]
            );

        }

    }



    #endregion

    #region 刷新快捷鍵渲染
    public void Refresh_HotKey()
    {
        Transform HotKey = PanelManage.panelManage.panels.HotKeyPanel;


        Transform PotionKey = HotKey.GetChild(0);
        Transform BasicKey = HotKey.GetChild(1);
        Transform EquipKey = HotKey.GetChild(2);

        UpdateHotKeyPanel(BasicKey, hotKeyStore.HotKeys);
        UpdateHotKeyPanel(PotionKey, hotKeyStore.HotKeys_potion);
        UpdateHotKeyPanel(EquipKey, hotKeyStore.HotKeys_equip);

        UpdateHotKeyPanel(BasicHotKeyPanel, hotKeyStore.HotKeys);
        UpdateHotKeyPanel(PotionHotKeyPanel, hotKeyStore.HotKeys_potion);
        UpdateHotKeyPanel(CloteHotKeyPanel, hotKeyStore.HotKeys_Clothe);
        UpdateHotKeyPanel(EquipHotKeyPanel, hotKeyStore.HotKeys_equip);

        FindObjectOfType<dolumi>().UpdateClothe();

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
                    slotContentParent.GetChild(i).GetChild(j).GetChild(2).GetComponent<Text>().text = bagSore[i].BagItems[j].bagSoreIndex.ToString();
                }
            }
        }
    }

    // 更新全部標籤的背包
    public void Update_AllBag()
    {
        var Allcategory = slotContentParent.GetChild(slotContentParent.childCount - 1);
        int index = 0;
        for (int i = 0; i < bagSore.Length; i++)
        {

            for (int j = 0; j < bagSore[i].BagItems.Count; j++)
            {
                Allcategory.GetChild(index).GetChild(0).GetComponent<Image>().sprite = bagSore[i].BagItems[j].BagItem_icon;
                //道具名字
                Allcategory.GetChild(index).GetChild(1).GetComponent<Text>().text = bagSore[i].BagItems[j].BagItem_name;
                //道具數量
                Allcategory.GetChild(index).GetChild(2).GetComponent<Text>().text = bagSore[i].BagItems[j].bagSoreIndex + "/" + j;
                index++;
            }
        }


        while (index < Allcategory.childCount && Allcategory.GetChild(index).GetChild(0).GetComponent<Image>().sprite != talkSystem.talkSystem_.peopleIcon.Icon[0])
        {
            defaultSlot(slotContentParent.childCount - 1, index);
            index++;
        }

    }

    // isAdd=>true 增加資料 isAdd=>刪除資料
    public void Update_BagUI(int bagSoreIndex, int index, bool isAdd)
    {
        var BagSore = bagSore[bagSoreIndex];
        if (isAdd)
        {
            //道具圖片//改變SLOT裡的icon
            slotContentParent.GetChild(bagSoreIndex).GetChild(index).GetChild(0).GetComponent<Image>().sprite = BagSore.BagItems[index].BagItem_icon;
            //道具名字
            slotContentParent.GetChild(bagSoreIndex).GetChild(index).GetChild(1).GetComponent<Text>().text = BagSore.BagItems[index].BagItem_name;
            //道具數量
            slotContentParent.GetChild(bagSoreIndex).GetChild(index).GetChild(2).GetComponent<Text>().text = BagSore.BagItems[index].bagSoreIndex.ToString();
        }
        else
        {
            //從index包含以後的物件全部重新加載
            if (BagSore.BagItems.Count == index)
            {
                defaultSlot(bagSoreIndex, index);
            }
            else
            {
                for (int i = index; i < BagSore.BagItems.Count + 1; i++)
                {
                    if (i == BagSore.BagItems.Count)
                    {
                        defaultSlot(bagSoreIndex, i);
                        break;
                    }
                    //道具圖片//改變SLOT裡的icon
                    slotContentParent.GetChild(bagSoreIndex).GetChild(i).GetChild(0).GetComponent<Image>().sprite = BagSore.BagItems[i].BagItem_icon;
                    //道具名字
                    slotContentParent.GetChild(bagSoreIndex).GetChild(i).GetChild(1).GetComponent<Text>().text = BagSore.BagItems[i].BagItem_name;
                    //道具數量
                    slotContentParent.GetChild(bagSoreIndex).GetChild(i).GetChild(2).GetComponent<Text>().text = BagSore.BagItems[i].bagSoreIndex.ToString();
                }
            }

            // 刷新快捷鍵對應關係
            Refresh_HotKeyStore(hotKeyStore.HotKeys, index, bagSoreIndex);
            Refresh_HotKeyStore(hotKeyStore.HotKeys_Clothe, index, bagSoreIndex);
            Refresh_HotKeyStore(hotKeyStore.HotKeys_equip, index, bagSoreIndex);
            Refresh_HotKeyStore(hotKeyStore.HotKeys_potion, index, bagSoreIndex);
            // 刷新快捷鍵渲染
            Refresh_HotKey();
        }

        Update_AllBag();

    }

    // 預設格子
    void defaultSlot(int bagIndex, int index)
    {
        //道具圖片//改變SLOT裡的icon
        slotContentParent.GetChild(bagIndex).GetChild(index).GetChild(0).GetComponent<Image>().sprite = talkSystem.talkSystem_.peopleIcon.Icon[0];
        //道具名字
        slotContentParent.GetChild(bagIndex).GetChild(index).GetChild(1).GetComponent<Text>().text = "道具名";
        //道具數量
        slotContentParent.GetChild(bagIndex).GetChild(index).GetChild(2).GetComponent<Text>().text = "數量";
    }

    // 刷新快捷鍵對應關係
    public void Refresh_HotKeyStore(HotKey[] hotKeys, int index, int bagSoreIndex)
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

                    // 刪除手持
                    playerController.playerController_.SetBag_Item(index);
                }
            }
        }
    }

    #endregion

    // #region Drag
    // // 
    // public void initDrag(Transform newDrag)
    // {
    //     // 初始化Drag物件
    //     DragItem.gameObject.SetActive(true);

    //     DragItem.GetComponent<Image>().sprite = newDrag.GetChild(0).GetComponent<Image>().sprite;

    //     newDrag.GetChild(0).GetComponent<Image>().color -= new Color(0, 0, 0, 1);

    //     // 觸發編輯事件
    //     newDrag.GetComponent<Button>().onClick.Invoke();

    //     StartCoroutine(DragCheck(newDrag));
    // }

    // // 檢查drag是否有問題
    // IEnumerator DragCheck(Transform newDrag)
    // {
    //     Vector3 prePos = new Vector3(0, 0, 0);
    //     Vector3 nowPos = DragItem.position;
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(0.2f);

    //         if (prePos != nowPos)
    //         {
    //             prePos = nowPos;
    //             nowPos = DragItem.position;
    //         }
    //         else
    //         {
    //             break;
    //         }
    //     }

    //     newDrag.GetChild(0).GetComponent<Image>().color += new Color(0, 0, 0, 1);
    //     newDrag.GetComponent<CanvasGroup>().blocksRaycasts = true;
    //     DragItem.gameObject.SetActive(false);
    // }



    // // 放開後會發生的事件(拖曳，碰撞到的)
    // public void DragEnd(Transform newDrag, Transform EditDragSlot, bool isToch)
    // {
    //     //Image newDragImg = newDrag.GetComponent<Image>();
    //     newDrag.GetChild(0).GetComponent<Image>().color += new Color(0, 0, 0, 1);

    //     if (isToch)
    //     {
    //         EditDragSlot.GetComponent<Button>().onClick.Invoke();
    //     }
    // }



    // #endregion
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
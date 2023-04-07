using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // 熱鍵物資儲存
    // 一般的熱鍵
    [SerializeField] HotKey[] HotKeys;
    //藥水熱鍵
    [SerializeField] HotKey[] HotKeys_potion;
    //衣服熱鍵
    [SerializeField] HotKey[] HotKeys_Clothe;
    [SerializeField] HotKey[] HotKeys_equip;

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
        HotKeys = new HotKey[BasicHotKeyPanel.childCount];
        for (int i = 0; i < HotKeys.Length; i++)
        {
            HotKeys[i] = new HotKey();
            StartCoroutine(HotKeyAddListener(BasicHotKeyPanel.GetChild(i).GetComponent<Button>(), i, BasicHotKeyPanel));
        }

        //藥水熱鍵
        HotKeys_potion = new HotKey[PotionHotKeyPanel.childCount];
        for (int i = 0; i < HotKeys_potion.Length; i++)
        {
            HotKeys_potion[i] = new HotKey();
            StartCoroutine(HotKeyAddListener(PotionHotKeyPanel.GetChild(i).GetComponent<Button>(), i, PotionHotKeyPanel));
        }

        //衣服熱鍵
        HotKeys_Clothe = new HotKey[CloteHotKeyPanel.childCount];
        for (int i = 0; i < HotKeys_Clothe.Length; i++)
        {
            HotKeys_Clothe[i] = new HotKey();
            StartCoroutine(HotKeyAddListener(CloteHotKeyPanel.GetChild(i).GetComponent<Button>(), i, CloteHotKeyPanel));
        }

        // 裝備熱鍵
        HotKeys_equip = new HotKey[EquipHotKeyPanel.childCount];
        for (int i = 0; i < HotKeys_equip.Length; i++)
        {
            HotKeys_equip[i] = new HotKey();
            StartCoroutine(HotKeyAddListener(EquipHotKeyPanel.GetChild(i).GetComponent<Button>(), i, EquipHotKeyPanel));
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

    IEnumerator HotKeyAddListener(Button btn, int i, Transform KeyPanel)
    {
        btn.name = "" + i;
        btn.onClick.AddListener(() => HotKey(i, HotKeys, KeyPanel));
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

    #region add new obj in Bag
    // 新增物品到背包
    public void checkItem(BagItem newBagIem)
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
        Update_BagUI(bagSore[bagSoreIndex], PutItemInBag(bagSore[bagSoreIndex], newBagIem), true);
    }

    //將商品放入背包
    public int PutItemInBag(BagSore BagSore, BagItem NewBagItem)
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

                    BagSore.ItemCount[i]++;
                    break;
                }
            }
        }
        else
        {
            //直接加新的道具到背包
            BagSore_BagItems.Add(NewBagItem);
            BagSore.ItemCount.Add(1);
            BagSore.isWear.Add(false);
        }

        bagSore[bagSoreIndex] = BagSore;

        return index;
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

        // var bagSore_BagIndex_BagItems_i = bagSore[BagIndex].BagItems[i];

        // BagInfo.gameObject.SetActive(true);

        // // icon
        // BagInfo.GetChild(0).GetComponent<Image>().sprite = bagSore_BagIndex_BagItems_i.BagItem_icon;

        // //名稱 
        // BagInfo.GetChild(1).GetComponent<Text>().text = bagSore_BagIndex_BagItems_i.BagItem_name;

        // // 介紹
        // BagInfo.GetChild(2).GetComponent<Text>().text = bagSore_BagIndex_BagItems_i.BagItem_info;

        //*********************************************************************************************************


        //當前item索引 
        itemIndex = i;


        //如果需樣wear介面就刪掉*****************************************************************************************

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

            var newPosImg = KeyPanel.GetChild(i).GetChild(0).GetComponent<Image>();

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
                            KeyPanel.GetChild(j).GetChild(0).GetComponent<Image>().sprite = newPosImg.sprite;
                        }
                        else
                        {
                            HotKeys[j].HotKey_Bag = -1;
                            HotKeys[j].HotKey_item = -1;
                            KeyPanel.GetChild(j).GetChild(0).GetComponent<Image>().sprite = null;
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


            // 渲染
            // slot.icon
            newPosImg.sprite = item.BagItem_icon;

            isHotKeyEditor = false;
        }

        //沒有進入編輯模式，則使用
        else
        {

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
            for (int i = index; i < BagSore.BagItems.Count; i++)
            {
                //道具圖片//改變SLOT裡的icon
                slotContentParent.GetChild(bagSoreIndex).GetChild(i).GetChild(0).GetComponent<Image>().sprite = BagSore.BagItems[i].BagItem_icon;
                //道具名字
                slotContentParent.GetChild(bagSoreIndex).GetChild(i).GetChild(1).GetComponent<Text>().text = BagSore.BagItems[i].BagItem_name;
                //道具數量
                slotContentParent.GetChild(bagSoreIndex).GetChild(i).GetChild(2).GetComponent<Text>().text = BagSore.ItemCount[i].ToString();
            }
        }
    }

    #endregion

    #region Drag
    // 
    public void initDrag(Transform newDrag)
    {
        // 如果有隱藏
        if (!DragItem.gameObject.activeSelf)
        {
            // 初始化Drag物件
            DragItem.gameObject.SetActive(true);

            DragItem.GetComponent<Image>().sprite = newDrag.GetChild(0).GetComponent<Image>().sprite;

            newDrag.GetComponent<Image>().color -= new Color(0, 0, 0, 1);

            // 觸發編輯事件
            newDrag.GetComponent<Button>().onClick.Invoke();
        }
        else
        {

        }

    }

    // 放開後會發生的事件(拖曳，碰撞到的)
    public void DragEnd(Transform newDrag, Transform EditDragSlot, bool isToch)
    {
        Image newDragImg = newDrag.GetComponent<Image>();
        newDragImg.color += new Color(0, 0, 0, 1);

        if (isToch)
            EditDragSlot.GetComponent<Button>().onClick.Invoke();
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

    // //右側裝備欄
    // public List<BagItem> potionBagItem;//背包物品_藥水
    // public List<BagItem> equipmentBagItem;//背包物品_裝備

    // //左側裝備欄
    // public List<BagItem> clotheBagItem;//衣服

    // //無法手持
    // public List<BagItem> materialBagItem;//材料


    // //可以手持
    // public List<BagItem> foodBagItem;//食物
    // public List<BagItem> toolBagItem;//工具
}

[System.Serializable]
public class HotKey
{
    public int HotKey_Bag = -1;
    public int HotKey_item = -1;
}


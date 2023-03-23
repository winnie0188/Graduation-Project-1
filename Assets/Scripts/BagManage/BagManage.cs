using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagManage : UIinit
{
    public static BagManage bagManage;
    public BagSore[] bagSore;


    #region initSlot
    [SerializeField] int slotCount;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform slotContentParent;
    #endregion

    #region temp
    //slotContentParent
    ScrollRect scrollRect;
    //新增或刪除物件時，對應了哪個背包
    int bagSoreIndex;
    //每頁已經置頂過了
    bool[] isUpperReady;
    #endregion

    private void Awake()
    {
        isUpperReady = new bool[slotContentParent.childCount];
        scrollRect = slotContentParent.GetComponent<ScrollRect>();

        bagManage = this;

        //slot初始化
        for (int i = 0; i < slotContentParent.childCount; i++)
        {
            initSlot(slotCount, slotPrefab, slotContentParent.GetChild(i));
        }


        //DEMO
        initBagUI();

    }

    //切換分類，記得要拉到按鈕上
    #region switch category
    public void switch_category(int id)
    {
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
    }
    //內容置頂
    IEnumerator ContentcomeBack(int i)
    {
        yield return null;
        // 這樣才能拖拉
        slotContentParent.GetChild(i).position -= new Vector3(0, 1000, 0);
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
        }

        bagSore[bagSoreIndex] = BagSore;

        return index;
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


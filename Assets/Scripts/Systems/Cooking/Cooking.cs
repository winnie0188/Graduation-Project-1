using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class Cooking : UIinit
{
    public Transform CookPanel;
    [SerializeField] recipeStore recipeStore;

    Dictionary<string, recipeLock> recipeDictionary = new Dictionary<string, recipeLock>();
    int count = 1;

    [SerializeField] Transform messagePanel;
    [SerializeField] Text message;

    [SerializeField] Text countText;

    [SerializeField] Sprite[] resultSprite;
    [SerializeField] Image resultImage;


    [Header("除了關閉其他都檔")]
    [SerializeField] Transform mask;
    [Header("烹飪時間")]
    [SerializeField] int time;
    [Header("進度條:加個mask移動跑進度")]
    [SerializeField] Slider loadBar;

    [Space(20)]

    [Header("新道具顯示")]
    [SerializeField] Image newprops;
    [SerializeField] recipe failure;

    [Header("右中左")]
    [SerializeField] slotData[] slotDatas;

    [SerializeField] CookingBtnState cookingBtnState;

    [Header("本體有image")]
    [SerializeField] Transform drag;

    // [Header("九宮格")]
    BagItem[] jiugongge = new BagItem[9];
    BagItem tempItem;

    CookingRight cookingRight;
    CookingCenter cookingCenter;
    CookingLeft cookingLeft;

    bool isShowRecipe = false;

    public static Cooking cooking;

    public CookingBtnState CookingBtnState { get => cookingBtnState; set => cookingBtnState = value; }

    public BagItem TempItem { get => tempItem; set => tempItem = value; }
    public BagItem[] Jiugongge { get => jiugongge; set => jiugongge = value; }
    public Dictionary<string, recipeLock> RecipeDictionary { get => recipeDictionary; set => recipeDictionary = value; }
    public recipeStore RecipeStore { get => recipeStore; set => recipeStore = value; }
    internal CookingCenter CookingCenter { get => cookingCenter; set => cookingCenter = value; }
    public Image Newprops { get => newprops; set => newprops = value; }
    internal CookingLeft CookingLeft { get => cookingLeft; set => cookingLeft = value; }
    internal CookingRight CookingRight { get => cookingRight; set => cookingRight = value; }
    public bool IsShowRecipe { get => isShowRecipe; set => isShowRecipe = value; }

    private void Awake()
    {
        cooking = this;

        for (int i = 0; i < RecipeStore.Recipes.Length; i++)
        {
            var cookDatas = RecipeStore.Recipes[i].BagItems;

            List<BagItem> bagItems = new List<BagItem>();

            for (int j = 0; j < cookDatas.Length; j++)
            {
                bagItems.Add(cookDatas[j].bagItem);
            }

            string str = search(bagItems.ToArray());

            RecipeDictionary.Add(
                str,
                new recipeLock()
                {
                    recipe = RecipeStore.Recipes[i],
                    islock = true,
                    index = i
                }
            );
        }

        cookingRight = new CookingRight(
            FindObjectOfType<BagManage>().SlotCount,
            slotDatas[0].slotPrefab,
            slotDatas[0].slotContent,
            1,
            -1
        );
        cookingCenter = new CookingCenter(
            9,
            slotDatas[1].slotPrefab,
            slotDatas[1].slotContent,
            1,
            -1
        );
        cookingLeft = new CookingLeft(
            RecipeStore.Recipes.Length,
            slotDatas[2].slotPrefab,
            slotDatas[2].slotContent,
            1,
            0
        );

        cookingRight.initRightSlot();
        cookingCenter.initCenterSlot();
        cookingLeft.initLeftSlot();


        cookingRight.UpdateRightUI(FindObjectOfType<BagManage>());
        cookingLeft.initUI();
    }

    #region button

    //九宮格不能重複，九宮格可以移回去
    //料理按鈕
    public void cuisine()
    {


        recipe recipe = failure;

        if (RecipeDictionary.TryGetValue(search(Jiugongge), out recipeLock recipeLock))
        {
            recipe = recipeLock.recipe;
            int itemCount;

            for (int i = 0; i < Jiugongge.Length; i++)
            {
                if (Jiugongge[i] != null)
                {
                    itemCount = BagManage.bagManage.itemCount(Jiugongge[i]);

                    for (int j = 0; j < recipe.BagItems.Length; j++)
                    {
                        if (Jiugongge[i] == recipe.BagItems[j].bagItem)
                        {
                            if (itemCount == -1)
                            {
                                openMessage("缺少食材:" + Jiugongge[i].BagItem_name);
                                return;
                            }
                            else
                            {
                                //數量不夠就減count
                                while (recipe.BagItems[j].count * count > itemCount)
                                {
                                    if (count == 1)
                                    {
                                        openMessage("食材:" + Jiugongge[i].BagItem_name + "數量不夠");
                                        return;
                                    }
                                    count -= 1;
                                }
                            }
                        }
                    }
                }

            }


            for (int i = 0; i < recipe.BagItems.Length; i++)
            {
                BagManage.bagManage.checkItem(
                    recipe.BagItems[i].bagItem,
                    recipe.BagItems[i].count * count * -1,
                    false,
                    true
                );
            }

        }
        else
        {
            int nullItem = 0;
            //全部空的話則不繼續
            for (int i = 0; i < Jiugongge.Length; i++)
            {
                if (Jiugongge[i] == null)
                {
                    nullItem++;
                }
            }

            if (nullItem == 9)
            {
                openMessage("請放置材料");
                return;
            }
            else
            {
                //配方沒的話放入的道具減數量*count
                for (int i = 0; i < Jiugongge.Length; i++)
                {
                    if (Jiugongge[i] != null)
                    {
                        BagManage.bagManage.checkItem(
                            Jiugongge[i],
                            count * -1,
                            false,
                            true
                        );
                    }
                }
            }
        }

        //清空九宮格
        cookingCenter.UpdateslotUi(null);

        load(true);

        Tween t = loadBar.DOValue(1, time).SetEase(Ease.Flash);
        t.OnComplete(() =>
        {
            //獲得道具
            BagManage.bagManage.checkItem(
                recipe.Dishes,
                count,
                false,
                true
            );


            //關閉遮罩那些
            load(false);

            //獲得道具的圖片
            Newprops.sprite = recipe.Dishes.BagItem_icon;
            //數字回歸
            count = 1;
            countText.text = count.ToString();

            //動畫結束後進度條歸0
            loadBar.value = 0;

            if (recipeLock != null)
            {
                if (recipeLock.islock == true)
                {
                    recipeLock.islock = false;
                    openMessage("恭喜解鎖新菜品「" + recipe.Dishes.BagItem_name + "」");
                    cookingLeft.UpdateUI();
                }
                else
                {
                    openMessage("獲得食材:「" + recipe.Dishes.BagItem_name + "」*" + count);
                }

                resultImage.sprite = resultSprite[1];
            }
            else
            {
                openMessage("烹飪失敗");
                resultImage.sprite = resultSprite[2];
            }

        });
    }


    #region 右中左
    public override void slot_event(int i)
    {
        cookingRight.slot_event(i);
    }

    public override void slot_event2(int i)
    {
        cookingCenter.slot_event(i);
    }

    public override void slot_event3(int i)
    {
        cookingLeft.slot_event(i);
    }
    #endregion


    public void addCount()
    {
        count++;
        countText.text = count.ToString();
    }

    public void redCount()
    {
        if (count - 1 > 0)
        {
            count--;
            countText.text = count.ToString();
        }
    }



    public void openMessage(string s)
    {
        messagePanel.gameObject.SetActive(true);
        message.text = s;
    }

    public void closeMessage()
    {
        messagePanel.gameObject.SetActive(false);
        resultImage.sprite = resultSprite[0];
    }

    #endregion

    void load(bool isLoad)
    {
        mask.gameObject.SetActive(isLoad);
    }

    //也可給背景用
    public void switcStateNONE()
    {
        cooking.CookingBtnState = CookingBtnState.NONE;
        cooking.endDrag();

        cooking.TempItem = null;
    }


    string search(BagItem[] bagItems)
    {
        int[] list = new int[9];

        for (int j = 0; j < bagItems.Length; j++)
        {
            if (bagItems[j] == null)
            {
                list[j] = -1;
            }
            else
            {
                list[j] = bagItems[j].id;
            }
        }

        Array.Sort(list);


        string str = "";
        for (int j = 0; j < list.Length; j++)
        {
            if (j < list.Length - 1)
            {
                str += list[j];
            }
            else
            {
                str += list[j] + "/";
            }
        }

        return str;
    }

    public void startDrag(Sprite sprite)
    {
        if (drag.GetChild(0).TryGetComponent<Image>(out var image))
        {
            image.sprite = sprite;
            StartCoroutine(dragging());
        }
        drag.gameObject.SetActive(true);
    }

    public void endDrag()
    {
        StopCoroutine(dragging());
        drag.gameObject.SetActive(false);
    }

    IEnumerator dragging()
    {
        yield return null;
        drag.position = Input.mousePosition;
        StartCoroutine(dragging());
    }
}

[System.Serializable]
public class slotData
{
    [Header("0:img")]
    public GameObject slotPrefab;
    public Transform slotContent;
}

[System.Serializable]
public class recipeLock
{
    public recipe recipe;
    public bool islock;
    public int index;
}


class CookingRight
{
    int slotCount;
    GameObject slotPrefab;
    Transform slotContent;
    float size;
    int child;

    public CookingRight(int slotCount, GameObject slotPrefab, Transform slotContent, float size, int child)
    {
        this.slotCount = slotCount;
        this.slotPrefab = slotPrefab;
        this.slotContent = slotContent;
        this.size = size;
        this.child = child;
    }

    public void initRightSlot()
    {
        Cooking cooking = Cooking.cooking;
        cooking.initSlot(slotCount, slotPrefab, slotContent, size, child);
    }

    public void slot_event(int i)
    {
        Cooking cooking = Cooking.cooking;

        if (cooking.CookingBtnState == CookingBtnState.NONE)
        {
            cooking.CookingBtnState = CookingBtnState.RIGHT;
            cooking.startDrag(BagManage.bagManage.bagSore[4].BagItems[i].BagItem_icon);

            cooking.TempItem = BagManage.bagManage.bagSore[4].BagItems[i];
        }
        else if (Cooking.cooking.CookingBtnState == CookingBtnState.RIGHT)
        {
            cooking.switcStateNONE();
        }
        else if (cooking.CookingBtnState == CookingBtnState.CENTER)
        {
            for (int j = 0; j < cooking.Jiugongge.Length; j++)
            {
                if (cooking.Jiugongge[j] == cooking.TempItem)
                {
                    //清空九宮格對應物品
                    cooking.Jiugongge[j] = null;
                    cooking.CookingCenter.refreshChild(j);
                    break;
                }
            }

            cooking.switcStateNONE();
        }
    }

    public void UpdateRightUI(BagManage bagManage)
    {
        BagItem item;

        for (int i = 0; i < slotContent.childCount; i++)
        {
            if (i < bagManage.bagSore[4].BagItems.Count)
            {
                item = bagManage.bagSore[4].BagItems[i];

                if (slotContent.GetChild(i).GetChild(0).TryGetComponent<Image>(out Image view))
                {
                    view.sprite = item.BagItem_icon;
                }

                slotContent.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                slotContent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}

class CookingCenter
{
    int slotCount;
    GameObject slotPrefab;
    Transform slotContent;
    [Header("最好大點")]
    float size;
    int child;

    public CookingCenter(int slotCount, GameObject slotPrefab, Transform slotContent, float size, int child)
    {
        this.slotCount = slotCount;
        this.slotPrefab = slotPrefab;
        this.slotContent = slotContent;
        this.size = size;
        this.child = child;
    }

    public void initCenterSlot()
    {
        Cooking cooking = Cooking.cooking;
        cooking.initSlot2(slotCount, slotPrefab, slotContent, size, child);
    }

    public void UpdateslotUi(recipe recipe)
    {
        if (recipe == null)
        {
            //清空九宮格
            for (int i = 0; i < slotContent.childCount; i++)
            {
                if (slotContent.GetChild(i).GetChild(0).TryGetComponent<Image>(out Image image))
                {
                    Cooking.cooking.Jiugongge[i] = null;
                    image.sprite = talkSystem.talkSystem_.peopleIcon.Icon[0];
                }
            }
        }
        else
        {
            //更新九宮格
            for (int i = 0; i < slotContent.childCount; i++)
            {
                if (slotContent.GetChild(i).GetChild(0).TryGetComponent<Image>(out Image image))
                {
                    Cooking.cooking.Jiugongge[i] = recipe.BagItems[i].bagItem;
                    if (Cooking.cooking.Jiugongge[i] != null)
                    {
                        image.sprite = recipe.BagItems[i].bagItem.BagItem_icon;
                    }
                    else
                    {
                        image.sprite = talkSystem.talkSystem_.peopleIcon.Icon[0];
                    }
                }
            }

            //獲得道具的圖片
            Cooking.cooking.Newprops.sprite = recipe.Dishes.BagItem_icon;
        }
    }

    public void refreshChild(int i)
    {
        if (slotContent.GetChild(i).GetChild(0).TryGetComponent<Image>(out Image imagej))
        {
            imagej.sprite = talkSystem.talkSystem_.peopleIcon.Icon[0];
        }
    }

    public void slot_event(int i)
    {
        Cooking cooking = Cooking.cooking;

        if (cooking.CookingBtnState == CookingBtnState.RIGHT)
        {
            if (cooking.IsShowRecipe == true)
            {
                cooking.IsShowRecipe = false;
                UpdateslotUi(null);
            }

            //放置
            cooking.Jiugongge[i] = cooking.TempItem;

            //如果已經擺過的話，之前放過的清掉
            for (int j = 0; j < cooking.Jiugongge.Length; j++)
            {
                if (cooking.Jiugongge[i] == cooking.Jiugongge[j] && i != j)
                {
                    cooking.Jiugongge[j] = null;
                    if (slotContent.GetChild(j).GetChild(0).TryGetComponent<Image>(out Image imagej))
                    {
                        imagej.sprite = talkSystem.talkSystem_.peopleIcon.Icon[0];
                    }

                    break;
                }
            }

            if (slotContent.GetChild(i).GetChild(0).TryGetComponent<Image>(out Image imageI))
            {
                imageI.sprite = cooking.TempItem.BagItem_icon;
            }


            cooking.switcStateNONE();
        }
        else if (cooking.CookingBtnState == CookingBtnState.NONE && cooking.Jiugongge[i] != null)
        {
            //更新為中心模式
            cooking.CookingBtnState = CookingBtnState.CENTER;
            cooking.startDrag(cooking.Jiugongge[i].BagItem_icon);
            cooking.TempItem = cooking.Jiugongge[i];
        }
        else if (Cooking.cooking.CookingBtnState == CookingBtnState.CENTER)
        {
            cooking.switcStateNONE();
        }


    }
}

class CookingLeft
{
    int slotCount;
    GameObject slotPrefab;
    Transform slotContent;
    float size;
    int child;

    public CookingLeft(int slotCount, GameObject slotPrefab, Transform slotContent, float size, int child)
    {
        this.slotCount = slotCount;
        this.slotPrefab = slotPrefab;
        this.slotContent = slotContent;
        this.size = size;
        this.child = child;
    }

    public void initLeftSlot()
    {
        Cooking cooking = Cooking.cooking;
        cooking.initSlot3(slotCount, slotPrefab, slotContent, size, child);
    }

    public void slot_event(int i)
    {
        Cooking cooking = Cooking.cooking;

        if (cooking.CookingBtnState != CookingBtnState.NONE)
        {
            cooking.switcStateNONE();
        }

        recipe recipe = cooking.RecipeStore.Recipes[i];

        cooking.CookingCenter.UpdateslotUi(recipe);
        cooking.IsShowRecipe = true;
    }


    public void initUI()
    {
        foreach (var item in Cooking.cooking.RecipeDictionary)
        {
            Transform transform = slotContent.GetChild(item.Value.index).GetChild(0);
            transform.GetChild(0).GetComponent<Text>().text = item.Value.recipe.Dishes.BagItem_name;
            transform.gameObject.SetActive(false);
        }
    }

    public void UpdateUI()
    {
        foreach (var item in Cooking.cooking.RecipeDictionary)
        {
            //記得把文字放進子
            if (item.Value.islock)
            {
                slotContent.GetChild(item.Value.index).GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                slotContent.GetChild(item.Value.index).GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}




public enum CookingBtnState
{
    NONE,
    RIGHT,
    CENTER
}
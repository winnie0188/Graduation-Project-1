using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class WorkSystem : UIinit
{
    public Transform workPanel;
    [SerializeField] workStore workStore;

    Dictionary<string, workLock> workDictionary = new Dictionary<string, workLock>();
    int count = 1;

    [SerializeField] Transform messagePanel;
    [SerializeField] Text message;
    [SerializeField] Text remark;

    [SerializeField] Text countText;

    [SerializeField] Sprite[] resultSprite;
    [SerializeField] Image resultImage;


    [Header("除了關閉其他都擋")]
    [SerializeField] Transform mask;

    [Header("烹飪時間")]
    [SerializeField] int time;
    [Header("進度條:加個mask移動跑進度")]
    [SerializeField] Slider loadBar;

    [Space(20)]

    [Header("新道具顯示")]
    [SerializeField] Image newprops;
    [SerializeField] workItem failure;

    [Header("右中左")]
    [SerializeField] slotData[] slotDatas;

    [SerializeField] CookingBtnState cookingBtnState;

    [Header("本體有image")]
    [SerializeField] Transform drag;

    // [Header("九宮格")]
    BagItem[] jiugongge = new BagItem[9];
    BagItem tempItem;

    WorkRight workRight;
    WorkCenter workCenter;
    WorkLeft workLeft;

    bool isShowRecipe = false;

    public static WorkSystem workSystem;

    public CookingBtnState CookingBtnState { get => cookingBtnState; set => cookingBtnState = value; }

    public BagItem TempItem { get => tempItem; set => tempItem = value; }
    public BagItem[] Jiugongge { get => jiugongge; set => jiugongge = value; }
    public Dictionary<string, workLock> WorkDictionary { get => workDictionary; set => workDictionary = value; }
    public workStore WorkStore { get => workStore; set => workStore = value; }
    internal WorkCenter WorkCenter { get => workCenter; set => workCenter = value; }
    public Image Newprops { get => newprops; set => newprops = value; }
    internal WorkLeft WorkLeft { get => workLeft; set => workLeft = value; }
    internal WorkRight WorkRight { get => workRight; set => workRight = value; }
    public bool IsShowRecipe { get => isShowRecipe; set => isShowRecipe = value; }

    private void Awake()
    {
        workSystem = this;

        for (int i = 0; i < WorkStore.WorkItems.Length; i++)
        {
            var cookDatas = WorkStore.WorkItems[i].BagItems;

            List<BagItem> bagItems = new List<BagItem>();

            for (int j = 0; j < cookDatas.Length; j++)
            {
                bagItems.Add(cookDatas[j].bagItem);
            }

            string str = search(bagItems.ToArray());

            WorkDictionary.Add(
                str,
                new workLock()
                {
                    workItem = WorkStore.WorkItems[i],
                    islock = true,
                    index = i
                }
            );
        }

        workRight = new WorkRight(
            FindObjectOfType<BagManage>().SlotCount,
            slotDatas[0].slotPrefab,
            slotDatas[0].slotContent,
            1,
            -1
        );
        workCenter = new WorkCenter(
            9,
            slotDatas[1].slotPrefab,
            slotDatas[1].slotContent,
            1,
            -1
        );
        workLeft = new WorkLeft(
            WorkStore.WorkItems.Length,
            slotDatas[2].slotPrefab,
            slotDatas[2].slotContent,
            1,
            0
        );

        workRight.initRightSlot();
        workCenter.initCenterSlot();
        workLeft.initLeftSlot();


        workRight.UpdateRightUI(FindObjectOfType<BagManage>());
        workLeft.initUI();
    }

    #region button

    //九宮格不能重複，九宮格可以移回去
    //料理按鈕
    public void cuisine()
    {
        workItem workItem = failure;

        if (WorkDictionary.TryGetValue(search(Jiugongge), out workLock workLock))
        {
            workItem = workLock.workItem;
            int itemCount;

            for (int i = 0; i < Jiugongge.Length; i++)
            {
                if (Jiugongge[i] != null)
                {
                    itemCount = BagManage.bagManage.itemCount(Jiugongge[i]);

                    for (int j = 0; j < workItem.BagItems.Length; j++)
                    {
                        if (Jiugongge[i] == workItem.BagItems[j].bagItem)
                        {
                            if (itemCount == -1)
                            {
                                openMessage("缺少材料:" + Jiugongge[i].BagItem_name, "");
                                return;
                            }
                            else
                            {
                                //數量不夠就減count
                                while (workItem.BagItems[j].count * count > itemCount)
                                {
                                    if (count == 1)
                                    {
                                        openMessage("材料:" + Jiugongge[i].BagItem_name + "數量不夠", "");
                                        return;
                                    }
                                    count -= 1;
                                }
                            }
                        }
                    }
                }

            }


            for (int i = 0; i < workItem.BagItems.Length; i++)
            {

                BagManage.bagManage.checkItem(
                    workItem.BagItems[i].bagItem,
                    workItem.BagItems[i].count * count * -1,
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
                openMessage("請放置材料", "");
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
        workCenter.UpdateslotUi(null);

        load(true);

        Tween t = loadBar.DOValue(1, time).SetEase(Ease.Flash);
        t.OnComplete(() =>
        {
            //獲得道具
            BagManage.bagManage.checkItem(
                workItem.Dishes,
                count,
                false,
                true
            );


            //關閉遮罩那些
            load(false);

            //獲得道具的圖片
            Newprops.sprite = workItem.Dishes.BagItem_icon;
            //數字回歸
            count = 1;
            countText.text = count.ToString();

            //動畫結束後進度條歸0
            loadBar.value = 0;

            if (workLock != null)
            {
                if (workLock.islock == true)
                {
                    workLock.islock = false;
                    openMessage("恭喜解鎖新香水「" + workItem.Dishes.BagItem_name + "」", "或許世界發生變化，也說不定？");
                    workLeft.UpdateUI();
                }
                else
                {
                    openMessage("獲得香水:「" + workItem.Dishes.BagItem_name + "」*" + count, "");
                }

                resultImage.sprite = resultSprite[1];
            }
            else
            {
                openMessage("製作失敗", "");
                resultImage.sprite = resultSprite[2];
            }

        });
    }


    #region 右中左
    public override void slot_event(int i)
    {
        workRight.slot_event(i);
    }

    public override void slot_event2(int i)
    {
        workCenter.slot_event(i);
    }

    public override void slot_event3(int i)
    {
        workLeft.slot_event(i);
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



    public void openMessage(string s, string mark)
    {
        messagePanel.gameObject.SetActive(true);
        message.text = s;

        remark.text = mark;
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
        workSystem.CookingBtnState = CookingBtnState.NONE;
        workSystem.endDrag();

        workSystem.TempItem = null;
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

        // Array.Sort(list);


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


class WorkRight
{
    int slotCount;
    GameObject slotPrefab;
    Transform slotContent;
    float size;
    int child;

    public WorkRight(int slotCount, GameObject slotPrefab, Transform slotContent, float size, int child)
    {
        this.slotCount = slotCount;
        this.slotPrefab = slotPrefab;
        this.slotContent = slotContent;
        this.size = size;
        this.child = child;
    }

    public void initRightSlot()
    {
        WorkSystem workSystem = WorkSystem.workSystem;
        workSystem.initSlot(slotCount, slotPrefab, slotContent, size, child);
    }

    public void slot_event(int i)
    {
        WorkSystem workSystem = WorkSystem.workSystem;

        if (workSystem.CookingBtnState == CookingBtnState.NONE)
        {
            workSystem.CookingBtnState = CookingBtnState.RIGHT;
            workSystem.startDrag(BagManage.bagManage.bagSore[3].BagItems[i].BagItem_icon);

            workSystem.TempItem = BagManage.bagManage.bagSore[3].BagItems[i];
        }
        else if (workSystem.CookingBtnState == CookingBtnState.RIGHT)
        {
            workSystem.switcStateNONE();
        }
        else if (workSystem.CookingBtnState == CookingBtnState.CENTER)
        {
            for (int j = 0; j < workSystem.Jiugongge.Length; j++)
            {
                if (workSystem.Jiugongge[j] == workSystem.TempItem)
                {
                    //清空九宮格對應物品
                    workSystem.Jiugongge[j] = null;
                    workSystem.WorkCenter.refreshChild(j);
                    break;
                }
            }

            workSystem.switcStateNONE();
        }
    }

    public void UpdateRightUI(BagManage bagManage)
    {
        BagItem item;

        for (int i = 0; i < slotContent.childCount; i++)
        {
            if (i < bagManage.bagSore[3].BagItems.Count)
            {
                item = bagManage.bagSore[3].BagItems[i];

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

class WorkCenter
{
    int slotCount;
    GameObject slotPrefab;
    Transform slotContent;
    [Header("最好大點")]
    float size;
    int child;

    public WorkCenter(int slotCount, GameObject slotPrefab, Transform slotContent, float size, int child)
    {
        this.slotCount = slotCount;
        this.slotPrefab = slotPrefab;
        this.slotContent = slotContent;
        this.size = size;
        this.child = child;
    }

    public void initCenterSlot()
    {
        WorkSystem workSystem = WorkSystem.workSystem;
        workSystem.initSlot2(slotCount, slotPrefab, slotContent, size, child);
    }

    public void UpdateslotUi(workItem workItem)
    {
        WorkSystem workSystem = WorkSystem.workSystem;

        if (workItem == null)
        {
            //清空九宮格
            for (int i = 0; i < slotContent.childCount; i++)
            {
                if (slotContent.GetChild(i).GetChild(0).TryGetComponent<Image>(out Image image))
                {
                    workSystem.Jiugongge[i] = null;
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
                    workSystem.Jiugongge[i] = workItem.BagItems[i].bagItem;
                    if (workSystem.Jiugongge[i] != null)
                    {
                        image.sprite = workItem.BagItems[i].bagItem.BagItem_icon;
                    }
                    else
                    {
                        image.sprite = talkSystem.talkSystem_.peopleIcon.Icon[0];
                    }
                }
            }

            //獲得道具的圖片
            workSystem.Newprops.sprite = workItem.Dishes.BagItem_icon;
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
        WorkSystem workSystem = WorkSystem.workSystem;

        if (workSystem.CookingBtnState == CookingBtnState.RIGHT)
        {
            if (workSystem.IsShowRecipe == true)
            {
                workSystem.IsShowRecipe = false;
                UpdateslotUi(null);
            }

            //放置
            workSystem.Jiugongge[i] = workSystem.TempItem;

            //如果已經擺過的話，之前放過的清掉
            for (int j = 0; j < workSystem.Jiugongge.Length; j++)
            {
                if (workSystem.Jiugongge[i] == workSystem.Jiugongge[j] && i != j)
                {
                    workSystem.Jiugongge[j] = null;
                    if (slotContent.GetChild(j).GetChild(0).TryGetComponent<Image>(out Image imagej))
                    {
                        imagej.sprite = talkSystem.talkSystem_.peopleIcon.Icon[0];
                    }

                    break;
                }
            }

            if (slotContent.GetChild(i).GetChild(0).TryGetComponent<Image>(out Image imageI))
            {
                imageI.sprite = workSystem.TempItem.BagItem_icon;
            }


            workSystem.switcStateNONE();
        }
        else if (workSystem.CookingBtnState == CookingBtnState.NONE && workSystem.Jiugongge[i] != null)
        {
            //更新為中心模式
            workSystem.CookingBtnState = CookingBtnState.CENTER;
            workSystem.startDrag(workSystem.Jiugongge[i].BagItem_icon);
            workSystem.TempItem = workSystem.Jiugongge[i];
        }
        else if (workSystem.CookingBtnState == CookingBtnState.CENTER)
        {
            workSystem.switcStateNONE();
        }
    }
}

class WorkLeft
{
    int slotCount;
    GameObject slotPrefab;
    Transform slotContent;
    float size;
    int child;

    public WorkLeft(int slotCount, GameObject slotPrefab, Transform slotContent, float size, int child)
    {
        this.slotCount = slotCount;
        this.slotPrefab = slotPrefab;
        this.slotContent = slotContent;
        this.size = size;
        this.child = child;
    }

    public void initLeftSlot()
    {
        WorkSystem workSystem = WorkSystem.workSystem;
        workSystem.initSlot3(slotCount, slotPrefab, slotContent, size, child);
    }

    public void slot_event(int i)
    {
        WorkSystem workSystem = WorkSystem.workSystem;

        if (workSystem.CookingBtnState != CookingBtnState.NONE)
        {
            workSystem.switcStateNONE();
        }

        workItem workItem = workSystem.WorkStore.WorkItems[i];

        workSystem.WorkCenter.UpdateslotUi(workItem);
        workSystem.IsShowRecipe = true;
    }


    public void initUI()
    {
        foreach (var item in WorkSystem.workSystem.WorkDictionary)
        {
            Transform transform = slotContent.GetChild(item.Value.index).GetChild(0);
            transform.GetChild(0).GetComponent<Text>().text = item.Value.workItem.Dishes.BagItem_name;
            transform.gameObject.SetActive(false);
        }
    }

    public void UpdateUI()
    {
        foreach (var item in WorkSystem.workSystem.WorkDictionary)
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


[System.Serializable]
public class workLock
{
    public workItem workItem;
    public bool islock;
    public int index;
}
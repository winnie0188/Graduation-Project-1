using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using QFSW.MOP2;
using System.Collections.Generic;

public class taskSystem : MonoBehaviour
{
    #region "任務"
    // 任務列表
    public List<taskItem> currentTaskArray = new List<taskItem>();
    //目的地物件
    public GameObject LightCircle;
    public SignPanel[] signPanels;
    [SerializeField] Text signText;
    #endregion

    #region slot
    public Transform taskContentPanel;
    [SerializeField] int slotCount;
    [SerializeField] GameObject contentList;
    [SerializeField] GameObject taskList;
    //存lightcircle
    public List<GameObject> noUseLightCircle = new List<GameObject>();
    public Dictionary<taskItem, GameObject> UseLightCircle = new Dictionary<taskItem, GameObject>();

    // 是否只有左邊
    bool isLeft;
    [SerializeField] Sprite[] TopIcons;

    // 判斷是否要畫面重載，確保任務不會跑版
    public static bool isReset = false;

    #endregion

    #region 主畫面預覽
    // 遊玩時顯示在右上角的panel
    [SerializeField] Transform BasicUiTaskPanel;
    #endregion


    // 唯一性
    public static taskSystem taskSystem_;

    #region  主畫面預覽
    public void BaiscUIstate()
    {
        BasicUiTaskPanel.gameObject.SetActive(!BasicUiTaskPanel.gameObject.activeSelf);
    }
    public void updateBasicUI()
    {
        var title = "";
        var content = "";
        if (currentTaskArray.Count > 0)
        {
            title = currentTaskArray[0].task_Title;
            content = currentTaskArray[0].task_Content;
        }
        BasicUiTaskPanel.transform.GetChild(0).GetComponent<Text>().text = title;
        BasicUiTaskPanel.transform.GetChild(1).GetComponent<Text>().text = content;
    }

    #endregion

    public void switchCurrentTaskArray(int i)
    {
        var temp = currentTaskArray[0];
        currentTaskArray[0] = currentTaskArray[i];
        currentTaskArray[i] = temp;

        // 更新ui
        StartCoroutine(UpdateTaskUI());
    }


    // 設置並獲取slotCount



    void Awake()
    {
        taskSystem_ = this;
        SetTaskPool();

        setSlotCount();
        initTaskList();
    }


    void setSlotCount()
    {
        if (currentTaskArray.Count % 2 == 1)
        {
            slotCount = currentTaskArray.Count / 2 + 1;
            isLeft = true;
        }
        else
        {
            slotCount = currentTaskArray.Count / 2;
            isLeft = false;
        }

    }

    public void initTaskList()
    {
        for (int i = 0; i < slotCount; i++)
        {
            if (i >= taskContentPanel.GetChild(0).childCount)
            {
                Vector3 temp = taskContentPanel.transform.position;
                Instantiate(taskList, temp, Quaternion.identity, taskContentPanel.GetChild(0));
            }
        }

        StartCoroutine(UpdateTaskUI());
    }

    public IEnumerator Reset_Panel()
    {
        taskContentPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        for (int i = 0; i < 5; i++)
        {
            taskContentPanel.gameObject.SetActive(false);
            yield return null;
            taskContentPanel.gameObject.SetActive(true);
            yield return null;
        }
        taskContentPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.3921569f);

        isReset = true;
    }


    // 初始化pool
    void SetTaskPool()
    {
        taskContentPanel.GetChild(0).gameObject.AddComponent<ContentSizeFitter>();
        taskContentPanel.GetChild(0).gameObject.AddComponent<VerticalLayoutGroup>();

        // ContentSizeFitter資料交接
        // 獲取目標組件
        ContentSizeFitter sourceFitter = contentList.GetComponent<ContentSizeFitter>();
        ContentSizeFitter targetFitter = taskContentPanel.GetChild(0).GetComponent<ContentSizeFitter>();

        // 將組件序列化為 JSON 字符串
        string json = JsonUtility.ToJson(sourceFitter);

        // 將 JSON 字符串反序列化為組件
        JsonUtility.FromJsonOverwrite(json, targetFitter);

        // VerticalLayoutGroup資料交接
        VerticalLayoutGroup sourceVertical = contentList.GetComponent<VerticalLayoutGroup>();
        VerticalLayoutGroup targetVertical = taskContentPanel.GetChild(0).GetComponent<VerticalLayoutGroup>();

        json = JsonUtility.ToJson(sourceVertical);
        JsonUtility.FromJsonOverwrite(json, targetVertical);


        // 獲取源和目標的 RectTransform
        RectTransform sourceRect = contentList.GetComponent<RectTransform>();
        RectTransform targetRect = taskContentPanel.GetChild(0).GetComponent<RectTransform>();

        // 複製源 RectTransform 的屬性到目標 RectTransform
        targetRect.anchorMin = sourceRect.anchorMin;
        targetRect.anchorMax = sourceRect.anchorMax;
        targetRect.anchoredPosition = sourceRect.anchoredPosition;
        targetRect.sizeDelta = sourceRect.sizeDelta;
        targetRect.pivot = sourceRect.pivot;
        targetRect.localScale = sourceRect.localScale;

    }

    #region 任務add/delete
    public void addTask(taskItem taskItem)
    {
        initTask(taskItem);
        currentTaskArray.Add(taskItem);

        setSlotCount();
        initTaskList();

        switchCurrentTaskArray(currentTaskArray.Count - 1);
    }

    public void removeTask(taskItem taskItem)
    {
        currentTaskArray.Remove(taskItem);

        setSlotCount();
        initTaskList();

        finshTask(taskItem);
    }

    #endregion

    #region 更新UI
    IEnumerator UpdateTaskUI()
    {

        Transform parent = taskContentPanel.GetChild(0);

        int index = -1;

        for (int i = 0; i < parent.childCount; i++)
        {
            if (i < slotCount)
            {
                int left = index + 1;
                int Right = index + 2;

                index = Right;


                //print(left + "/" + Right);

                if (Right < currentTaskArray.Count - 1)
                {

                    taskItemsAddTask(currentTaskArray[left], currentTaskArray[Right], left, Right);
                }
                else
                {
                    if (isLeft)
                    {
                        taskItemsAddTask(currentTaskArray[left], null, left, Right);
                    }
                    else
                    {
                        taskItemsAddTask(currentTaskArray[left], currentTaskArray[Right], left, Right);
                    }
                }

                void taskItemsAddTask(taskItem taskItem1, taskItem taskItem2, int id1, int id2)
                {

                    taskItem[] taskItems = new taskItem[2];
                    taskItems[0] = taskItem1;
                    taskItems[1] = taskItem2;

                    int[] ids = new int[2];
                    ids[0] = id1;
                    ids[1] = id2;

                    setTask(parent.GetChild(i).gameObject, taskItems, ids);
                }
                parent.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                parent.GetChild(i).gameObject.SetActive(false);
            }

        }

        yield return null;

        if (BagManage.bagManage.scrollRect.content == taskContentPanel.GetChild(0) && PanelManage.panelManage.panels.BagPanel.gameObject.activeSelf)
        {
            StartCoroutine(Reset_Panel());
        }
        else
        {
            isReset = false;
        }



        updateBasicUI();
    }
    void setTask(GameObject taskList, taskItem[] taskItems, int[] ids)
    {
        // 左
        if (taskItems[0] != null)
        {
            setTask(taskList.transform.GetChild(0).GetChild(1).gameObject, taskItems[0], ids[0]);
        }


        // 右
        if (taskItems[1] != null)
        {
            setTask(taskList.transform.GetChild(1).GetChild(0).gameObject, taskItems[1], ids[1]);
            taskList.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            taskList.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        }

        void setTask(GameObject task, taskItem taskItem, int id)
        {
            if (task.TryGetComponent<taskData>(out var taskData))
            {
                taskData.setTaskTitle(taskItem.task_Title);
                taskData.setTaskContent(taskItem.task_Content);
                taskData.setTaskNeed(taskItem.task_need);
                taskData.setTaskGet(taskItem.task_get);
                taskData.setID(id);

                if (id == 0)
                {
                    taskData.setTaskTop(TopIcons[0]);
                }
                else
                {
                    taskData.setTaskTop(TopIcons[1]);
                }
            }
        }
    }


    #endregion


    #region 任務初始化
    void initTask(taskItem taskItem)
    {
        if (taskItem.TaskType == TaskType.walk)
        {
            var taskPos = taskItem.gettask_Walk().taskPos;
            if (noUseLightCircle.Count <= 0)
            {
                UseLightCircle.Add(taskItem, Instantiate(LightCircle, taskPos, Quaternion.identity, transform));

            }
            else
            {
                noUseLightCircle[0].SetActive(true);
                noUseLightCircle[0].transform.position = taskPos;

                UseLightCircle.Add(taskItem, noUseLightCircle[0]);
                noUseLightCircle.RemoveAt(0);
            }
        }

        if (taskItem.TaskType == TaskType.follow)
        {
            var follow = taskItem.gettask_Follow();

            follow.currentIndex = -1;
            //要記得改名
            follow.npc = NpcFactory.npcFactory.factorys[follow.npcName];

        }

        if (taskItem.TaskType == TaskType.guide)
        {
            var guide = taskItem.gettask_Guide();
            Guide.guide.maskPanel.gameObject.SetActive(true);

            Guide.guide.setTarget(Guide.guide.guideList[guide.guide], 0, null);
            //長度初始化
            guide.ListLength = Guide.guide.guideList[guide.guide].guideData.Count;
        }

        if (taskItem.TaskType == TaskType.sign)
        {
            var sign = taskItem.gettask_Sign();
            signPanels[sign.panelId].panel.gameObject.SetActive(true);
        }
    }
    #endregion

    #region 任務達成
    void finshTask(taskItem taskItem)
    {
        if (taskItem.TaskType == TaskType.walk)
        {
            removeCircle(taskItem);
        }

        if (taskItem.TaskType == TaskType.follow)
        {
            var npc = taskItem.gettask_Follow().npc.GetComponent<NPC>();
            NpcFactory.npcFactory.closeTaskTalk(npc.transform);
            taskItem.gettask_Follow().currentIndex = -1;
            removeCircle(taskItem);
        }

        if (taskItem.TaskType == TaskType.guide)
        {
            var guide = taskItem.gettask_Guide();
            Guide.guide.maskPanel.gameObject.SetActive(false);
            Guide.guide.guideList[guide.guide].currentIndex = 0;

        }



        finshTask(taskItem.finshTask, taskItem.finshTask.openFinshUi);
    }

    public void finshTask(FinshTask finshTask, bool openFinshUi)
    {
        if (openFinshUi)
        {
            List<Sprite> sprites = new List<Sprite>();

            for (int i = 0; i < finshTask.bagItem.Length; i++)
            {
                sprites.Add(finshTask.bagItem[i].BagItem_icon);
            }

            FinshUi.finshUi.openCanvas(sprites, null, finshTask, finshTask.FinshContent);
        }
        else
        {
            if (finshTask.openFadePanel == true)
            {
                FadePanel.fadePanel.playAni();
            }

            if (finshTask.talkContent != null)
            {
                talkSystem.talkSystem_.openTalk(finshTask.talkContent);
            }
            if (finshTask.taskItem != null)
            {
                taskSystem.taskSystem_.addTask(finshTask.taskItem);
            }
            if (finshTask.bagItem.Length != 0)
            {
                for (int i = 0; i < finshTask.bagItem.Length; i++)
                {
                    BagManage.bagManage.checkItem(
                        finshTask.bagItem[i], 1, false, true
                    );
                }
            }
            if (finshTask.removeBagItem.Length != 0)
            {
                for (int i = 0; i < finshTask.removeBagItem.Length; i++)
                {
                    BagManage.bagManage.checkItem(
                        finshTask.removeBagItem[i], -1, false, true
                    );
                }
            }
            if (finshTask.teleports.Length != 0)
            {
                for (int i = 0; i < finshTask.teleports.Length; i++)
                {
                    NpcFactory.npcFactory.AIteleport(
                        NpcFactory.npcFactory.factorys[finshTask.teleports[i].AiName],
                        finshTask.teleports[i].position
                    );
                }
            }

            if (finshTask.playerRotateY != 0)
            {
                playerController.playerController_.taskRotate(finshTask.playerRotateY);
            }
            if (finshTask.npcTaskState.Length != 0)
            {
                for (int i = 0; i < finshTask.npcTaskState.Length; i++)
                {
                    Transform npc = NpcFactory.npcFactory.factorys[finshTask.npcTaskState[i].npcName];
                    NpcFactory.npcFactory.setNpcTask(
                        npc,
                        finshTask.npcTaskState[i].state
                    );
                }
            }

            if (finshTask.AnimaData.Length != 0)
            {
                for (int i = 0; i < finshTask.AnimaData.Length; i++)
                {
                    Transform npc = NpcFactory.npcFactory.factorys[finshTask.AnimaData[i].NPC];
                    NpcFactory.npcFactory.setAni(
                        npc,
                        finshTask.AnimaData[i].ani,
                        finshTask.AnimaData[i].state
                    );
                }
            }
        }
    }

    void removeCircle(taskItem taskItem)
    {
        if (UseLightCircle.TryGetValue(taskItem, out GameObject temp))
        {
            temp.SetActive(false);
            //資料交換
            noUseLightCircle.Add(temp);
            UseLightCircle.Remove(taskItem);
        }
    }
    #endregion

    #region panel
    public bool isOpen()
    {
        for (int i = 0; i < signPanels.Length; i++)
        {
            if (signPanels[i].panel.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
    public void setSign(Text text)
    {
        if (text.text.Length > 0)
        {
            setPanels(0);
            signText.text = text.text;
            signPanels[0].panel.gameObject.SetActive(false);
        }
    }

    public void setPanels(int id)
    {
        signPanels[id].isFinsh = true;
    }
    #endregion
}


[System.Serializable]
public class SignPanel
{
    public Transform panel;
    public bool isFinsh;
}
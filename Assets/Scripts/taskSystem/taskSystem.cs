using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using QFSW.MOP2;


public class taskSystem : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] ObjectPool TaskPool = null;

    // 任務列表
    [SerializeField] taskItem[] currentTaskArray;

    #region slot
    [SerializeField] Transform taskContentPanel;
    [SerializeField] int slotCount;


    [SerializeField] GameObject contentList;


    #endregion

    // 遊玩時顯示在右上角的panel
    [SerializeField] Transform BasicUiTaskPanel;

    // 是否只有左邊
    bool isLeft;

    [SerializeField] Sprite[] TopIcons;


    // 判斷是否要畫面重載，確保任務不會跑版
    public static bool isReset = false;

    // 唯一性
    public static taskSystem taskSystem_;




    public void switchCurrentTaskArray(int i)
    {
        var temp = currentTaskArray[0];
        currentTaskArray[0] = currentTaskArray[i];
        currentTaskArray[i] = temp;

        // 更新ui
        UpdateTaskUI();
    }

    // 設置並獲取slotCount
    void setSlotCount()
    {
        taskSystem_ = this;

        if (currentTaskArray.Length % 2 == 1)
        {
            slotCount = currentTaskArray.Length / 2 + 1;
            isLeft = true;
        }
        else
        {
            slotCount = currentTaskArray.Length / 2;
            isLeft = false;
        }
    }


    void Awake()
    {
        TaskPool.Initialize();
        TaskPool.ObjectParent.parent = taskContentPanel;


        SetTaskPool();

        setSlotCount();

        initTaskList();
    }

    public void initTaskList()
    {
        for (int i = 0; i < slotCount; i++)
        {
            TaskPool.GetObject(transform.position);
        }


        UpdateTaskUI();
    }

    public IEnumerator Reset_Panel()
    {
        taskContentPanel.gameObject.SetActive(true);
        yield return null;
        taskContentPanel.gameObject.SetActive(false);
        yield return null;
        taskContentPanel.gameObject.SetActive(true);
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


    void deleteTask()
    {

    }

    #endregion

    #region 更新UI
    void UpdateTaskUI()
    {

        int index = -1;

        for (int i = 0; i < taskContentPanel.GetChild(0).childCount; i++)
        {
            if (taskContentPanel.GetChild(0).GetChild(i).gameObject.activeSelf)
            {

                int left = index + 1;
                int Right = index + 2;

                index = Right;


                if (Right < currentTaskArray.Length - 1)
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

                    setTask(taskContentPanel.GetChild(0).GetChild(i).gameObject, taskItems, ids);
                }
            }
        }
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


    /*
        #region 更新釘選
        public override void slot_event(int i)
        {
            // 當前位置置頂
            var temp = taskArray[0];
            taskArray[0] = taskArray[i];
            taskArray[i] = temp;

            // 更新UI
            UpdateTaskUI();
        }
        #endregion

        #region 展開/隱藏
        
    // 展開置頂或隱藏
    public void ShowAHide()
    {
        if (BasicUiTaskPanel.gameObject.activeSelf)
        {
            BasicUiTaskPanel.gameObject.SetActive(false);
        }
        else
        {
            BasicUiTaskPanel.gameObject.SetActive(true);
        }
    }
#endregion
*/
}

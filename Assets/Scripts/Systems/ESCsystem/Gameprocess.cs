using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameprocess : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] taskItem firstTask;
    [SerializeField] BagItem firstBag;

    // 如果沒讀取檔案則加載這個
    public void init()
    {
        if (PanelManage.panelManage.getIsCreateMode())
            return;

        initCoroutine();
    }

    void initCoroutine()
    {
        //新增任務
        taskSystem.taskSystem_.currentTaskArray.Add(firstTask);
        //新道具
        BagManage.bagManage.checkItem(
                   firstBag,
                   1,
                   false,
                   true
                );

        BagManage bagManage = BagManage.bagManage;
        var hotKeyStore = bagManage.hotKeyStore;

        //換裝
        var clothe1 = hotKeyStore.HotKeys_Clothe[0];
        clothe1.HotKey_Bag = firstBag.bagSoreIndex;
        clothe1.HotKey_item =
        BagManage.bagManage.findIndex(BagManage.bagManage.bagSore[clothe1.HotKey_Bag], firstBag);

        BagManage.bagManage.Refresh_HotKey();
    }

    private void FixedUpdate()
    {
        if (PanelManage.panelManage.getIsCreateMode())
            return;

        var list = taskSystem.taskSystem_.currentTaskArray;
        for (int i = 0; i < list.Count; i++)
        {
            switch (list[i].TaskType)
            {
                case TaskType.walk:

                    taskWalk(list[i]);

                    break;
                case TaskType.collect:
                    break;
                default:
                    break;
            }
        }
    }

    void taskWalk(taskItem taskItem)
    {
        task_Walk task_Walk = taskItem.gettask_Walk();
        if ((playerController.playerController_.transform.position - task_Walk.taskPos).magnitude < task_Walk.distance)
        {
            finshTask(taskItem.finshTask);
            taskSystem.taskSystem_.currentTaskArray.Remove(taskItem);
        }
    }


    #region 任務達成
    void finshTask(FinshTask finshTask)
    {
        if (finshTask.talkContent != null)
        {
            talkSystem.talkSystem_.openTalk(finshTask.talkContent);
        }
    }
    #endregion
}

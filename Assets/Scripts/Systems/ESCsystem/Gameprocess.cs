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
        taskSystem.taskSystem_.addTask(firstTask);

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
        NavPathArrow.navPathArrow.hidePath();
        for (int i = 0; i < list.Count; i++)
        {
            switch (list[i].TaskType)
            {
                case TaskType.walk:
                    taskWalk(list[i], i == 0);
                    break;
                case TaskType.collect:
                    break;
                case TaskType.follow:
                    taskFollow(list[i], i == 0);
                    break;
                case TaskType.guide:
                    taskGuide(list[i], i == 0);
                    break;
                case TaskType.sign:
                    taskSign(list[i], i == 0);
                    break;
                default:
                    break;
            }
        }
    }

    void taskWalk(taskItem taskItem, bool isfirst)
    {
        task_Walk task_Walk = taskItem.gettask_Walk();
        if ((playerController.playerController_.transform.position - task_Walk.taskPos).magnitude < task_Walk.distance)
        {
            taskSystem.taskSystem_.removeTask(taskItem);
        }

        if (isfirst)
        {
            Vector3[] points = new Vector3[2];
            points[0] = playerController.playerController_.transform.position;
            points[1] = task_Walk.taskPos;

            NavPathArrow.navPathArrow.drawPath(points);
        }
    }

    void taskFollow(taskItem taskItem, bool isfirst)
    {
        // print(1);
        task_followNpc task_FollowNpc = taskItem.gettask_Follow();
        Transform npc = null;
        bool isOpenTalk = false;


        if (task_FollowNpc.npc != null && task_FollowNpc.npc.gameObject.activeSelf)
        {

            npc = task_FollowNpc.npc;

            if (isfirst)
            {
                Vector3[] points = new Vector3[2];
                points[0] = playerController.playerController_.transform.position;
                points[1] = npc.transform.position;

                NavPathArrow.navPathArrow.drawPath(points);
            }



            if ((npc.transform.position - task_FollowNpc.endPOS).magnitude <= 1)
            {
                taskSystem taskSystem = taskSystem.taskSystem_;
                if (!taskSystem.UseLightCircle.TryGetValue(taskItem, out var use) && taskItem.InstantiateNewCircle)
                {
                    if (taskSystem.noUseLightCircle.Count <= 0)
                    {
                        taskSystem.UseLightCircle.Add(taskItem, Instantiate(taskSystem.LightCircle, task_FollowNpc.endPOS, Quaternion.identity, transform));

                    }
                    else
                    {
                        taskSystem.noUseLightCircle[0].SetActive(true);
                        taskSystem.noUseLightCircle[0].transform.position = task_FollowNpc.endPOS;

                        taskSystem.UseLightCircle.Add(taskItem, taskSystem.noUseLightCircle[0]);
                        taskSystem.noUseLightCircle.RemoveAt(0);
                    }
                }


                if ((playerController.playerController_.transform.position - task_FollowNpc.endPOS).magnitude <= task_FollowNpc.endDistance)
                {
                    taskSystem.taskSystem_.removeTask(taskItem);
                }
                else
                {
                    isOpenTalk = true;
                }
            }
            else
            {
                if ((playerController.playerController_.transform.position - npc.transform.position).magnitude <= task_FollowNpc.distance)
                {
                    NpcFactory.npcFactory.taskMove(task_FollowNpc, npc);
                }
                else
                {
                    isOpenTalk = true;
                }
            }



            if (isOpenTalk && NpcFactory.npcFactory.getNPCtalkFinsh(npc))
            {
                var NPCsay = task_FollowNpc.NPCsay;
                NpcFactory.npcFactory.openTaskTalk(npc, NPCsay[Random.Range(0, NPCsay.Length)]);
            }
            else if (!isOpenTalk)
            {
                NpcFactory.npcFactory.closeTaskTalk(npc);
            }

        }
    }

    void taskGuide(taskItem taskItem, bool isfirst)
    {
        var guide = taskItem.gettask_Guide();
        if (guide.ListLength == Guide.guide.guideList[guide.guide].currentIndex)
        {
            taskSystem.taskSystem_.removeTask(taskItem);
        }
    }

    void taskSign(taskItem taskItem, bool isfirst)
    {
        var sign = taskItem.gettask_Sign();
        if (taskSystem.taskSystem_.signPanels[sign.panelId].isFinsh)
        {
            taskSystem.taskSystem_.removeTask(taskItem);
        }
    }
}

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

        LightingManager.lightingManager.setDate(1, 1);
        LightingManager.lightingManager.setWeek(1);
        LightingManager.lightingManager.setYear(517);

        NpcFactory.npcFactory.setAllTask(true);
    }

    private void FixedUpdate()
    {
        if (PanelManage.panelManage.getIsCreateMode())
            return;

        if (!FadePanel.fadePanel.isFinsh())
        {
            return;
        }

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
                case TaskType.immediately:
                    taskImmediately(list[i]);
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

        if (isfirst && taskItem.isInstantiatePath)
        {
            Vector3[] points = new Vector3[2];
            points[0] = playerController.playerController_.transform.position;
            points[1] = task_Walk.taskPos;

            NavPathArrow.navPathArrow.drawPath(points);
        }
    }

    void taskFollow(taskItem taskItem, bool isfirst)
    {
        task_followNpc task_FollowNpc = taskItem.gettask_Follow();
        Transform npc = null;
        bool isOpenTalk = task_FollowNpc.isNearSay;

        Vector3 playerPos = playerController.playerController_.transform.position;

        //如果是線性劇情需要抵達+對話結束兩個條件;
        int LINETALK = 0;

        if (task_FollowNpc.npc != null && task_FollowNpc.npc.gameObject.activeSelf)
        {

            npc = task_FollowNpc.npc;
            Vector3 npcPos = npc.transform.position;

            if (isfirst && taskItem.isInstantiatePath)
            {
                Vector3[] points = new Vector3[2];
                points[0] = playerPos;
                points[1] = npcPos;

                NavPathArrow.navPathArrow.drawPath(points);
            }


            if (task_FollowNpc.isIdle && task_FollowNpc.followSayType == FollowSayType.LINER)
            {
                if ((playerPos - npcPos).magnitude > task_FollowNpc.distance)
                {
                    isOpenTalk = !isOpenTalk;
                }
            }
            else
            {
                if ((npcPos - task_FollowNpc.endPOS).magnitude <= 1.5f)
                {
                    // var v = task_FollowNpc.endPOS;
                    // v.y = npc.transform.position.y;
                    // npc.transform.position = v;
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


                    if ((playerPos - task_FollowNpc.endPOS).magnitude <= task_FollowNpc.endDistance)
                    {
                        if (task_FollowNpc.followSayType != FollowSayType.LINER)
                        {
                            taskSystem.taskSystem_.removeTask(taskItem);
                        }
                        LINETALK += 1;
                    }
                    else if ((playerPos - npcPos).magnitude > task_FollowNpc.distance)
                    {
                        isOpenTalk = !isOpenTalk;
                    }
                }
                else
                {
                    if ((playerPos - npcPos).magnitude <= task_FollowNpc.distance)
                    {
                        NpcFactory.npcFactory.taskMove(task_FollowNpc, npc);
                    }
                    else
                    {
                        isOpenTalk = !isOpenTalk;
                    }
                }
            }

            if (isOpenTalk)
            {
                var NPCsays = task_FollowNpc.nPCsays;

                if (task_FollowNpc.followSayType == FollowSayType.RADOM)
                {
                    if (NpcFactory.npcFactory.getNPCtalkFinsh(npc))
                    {
                        NpcFactory.npcFactory.openTaskTalk(
                            npc,
                            NPCsays[Random.Range(0, NPCsays.Length)].content
                        );
                    }
                }
                else if (task_FollowNpc.followSayType == FollowSayType.LINER)
                {
                    int index = task_FollowNpc.currentIndex;
                    Transform preTalk = null;
                    if (index != -1)
                    {
                        preTalk = NpcFactory.npcFactory.factorys[NPCsays[index].whoTalk];
                    }

                    if (preTalk == null || NpcFactory.npcFactory.getNPCtalkFinsh(preTalk))
                    {
                        index = index + 1;
                        task_FollowNpc.currentIndex = index;

                        if (task_FollowNpc.currentIndex < NPCsays.Length)
                        {
                            var newTalk = NpcFactory.npcFactory.factorys[NPCsays[index].whoTalk];

                            if (newTalk != preTalk && preTalk != null)
                            {
                                NpcFactory.npcFactory.closeTaskTalk(preTalk);
                            }
                            NpcFactory.npcFactory.openTaskTalk(
                                newTalk,
                                NPCsays[index].content
                            );
                        }
                        else
                        {
                            clotheAll();
                            if (task_FollowNpc.isIdle)
                            {
                                taskSystem.taskSystem_.removeTask(taskItem);
                            }
                            else
                            {
                                LINETALK += 1;
                                if (LINETALK == 2)
                                {
                                    taskSystem.taskSystem_.removeTask(taskItem);
                                }
                            }
                        }

                    }
                }
            }
            else if (!isOpenTalk)
            {
                clotheAll();
            }

            void clotheAll()
            {
                for (int i = 0; i < task_FollowNpc.TalkNPCNames.Length; i++)
                {
                    NpcFactory.npcFactory.closeTaskTalk(
                        NpcFactory.npcFactory.factorys[task_FollowNpc.TalkNPCNames[i]]
                    );
                }
            }

            // if (isOpenTalk && NpcFactory.npcFactory.getNPCtalkFinsh(npc))
            // {
            //     var NPCsay = task_FollowNpc.NPCsay;
            //     if (task_FollowNpc.followSayType == FollowSayType.RADOM)
            //     {
            //         NpcFactory.npcFactory.openTaskTalk(npc, NPCsay[Random.Range(0, NPCsay.Length)], -1);
            //     }
            //     else if (task_FollowNpc.followSayType == FollowSayType.LINER)
            //     {
            //         int index = NpcFactory.npcFactory.getTalkIndex(npc) + 1;
            //         if (index < NPCsay.Length)
            //         {
            //             NpcFactory.npcFactory.openTaskTalk(npc, NPCsay[index], index);
            //         }
            //         else if (task_FollowNpc.isIdle)
            //         {
            //             taskSystem.taskSystem_.removeTask(taskItem);
            //         }
            //     }
            // }
            // else if (!isOpenTalk)
            // {
            //     NpcFactory.npcFactory.closeTaskTalk(npc);
            // }

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
    void taskImmediately(taskItem taskItem)
    {
        taskSystem.taskSystem_.removeTask(taskItem);
    }

}

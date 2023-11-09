using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcFactory : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform factoryParent;
    public Dictionary<string, Transform> factorys = new Dictionary<string, Transform>();
    public static NpcFactory npcFactory;
    private void Awake()
    {
        npcFactory = this;
        for (int i = 0; i < factoryParent.childCount; i++)
        {
            factorys.Add(factoryParent.GetChild(i).name, factoryParent.GetChild(i));
        }

        factorys.Add("player", FindObjectOfType<playerInteractive>().transform);

        foreach (var item in factorys)
        {
            if (item.Value.TryGetComponent<NPC>(out var npc))
            {
                npc.Init();
            }
        }
    }
    private void FixedUpdate()
    {
        foreach (var item in factorys)
        {
            if (item.Value.TryGetComponent<NPC>(out var npc))
            {
                npc.Runnig();
            }
        }
    }

    public void getDateSign(int day, int start)
    {
        foreach (var item in factorys)
        {
            if (item.Value.TryGetComponent<NPC>(out var npc))
            {
                npc.receivedSignal(day, start);
            }
        }
    }


    public void setAllTask(bool state)
    {
        foreach (var item in factorys)
        {
            if (item.Value.TryGetComponent<NPC>(out var npc))
            {
                npc.isTask = state;
            }
        }
    }


    public void setAni(Transform npc, string ani, bool state)
    {
        NPC npcScript = npc.GetComponent<NPC>();

        npcScript.Ani1.SetBool(ani, state);
    }

    public void setNpcTask(Transform npc, bool state)
    {
        NPC npcScript = npc.GetComponent<NPC>();

        if (state == false)
        {
            if (npcScript.NPCstate1 == NPCstate.WORK)
            {
                if (npcScript.NPCdata1.NPCType == NPCType.FATDOVE)
                {
                    npcScript.fatdoveTranslate(1);
                }
                else
                {
                    npcScript.setCenter(npcScript.NPCdata1.ShopPos[0], true, 3);
                }
            }

            else if (npcScript.NPCstate1 == NPCstate.WALK)
            {
                ActivePos activePos = npcScript.NPCdata1.hauntingPlace[Random.Range(0, npcScript.NPCdata1.hauntingPlace.Length)];
                npcScript.setCenter(
                    activePos.pos,
                     false,
                    activePos.range
                );
            }
        }


        npcScript.isTask = state;
    }

    public void taskMove(task_followNpc task_FollowNpc, Transform npc)
    {
        NPC npcScript = npc.GetComponent<NPC>();
        npcScript.LookAt(task_FollowNpc.endPOS);
        npcScript.GetComponent<NPC>().walkFront();
    }

    public void taskStopMove(Transform npc)
    {
        NPC npcScript = npc.GetComponent<NPC>();
        npcScript.GetComponent<NPC>().stopWalk();
    }


    public void openTaskTalk(Transform npc, string content)
    {
        if (npc != factorys["player"])
        {
            NPC npcScript = npc.GetComponent<NPC>();
            npcScript.setContent(content);
        }
        else
        {
            playerInteractive player = npc.GetComponent<playerInteractive>();
            player.setContent(content);
        }
    }

    public void closeTaskTalk(Transform npc)
    {
        if (npc != factorys["player"])
        {
            NPC npcScript = npc.GetComponent<NPC>();
            npcScript.closeConetnt();
        }
        else
        {
            playerInteractive player = npc.GetComponent<playerInteractive>();
            player.closeConetnt();
        }

    }

    public bool getNPCtalkFinsh(Transform npc)
    {
        if (npc != factorys["player"])
        {
            NPC npcScript = npc.GetComponent<NPC>();
            return npcScript.isFinshTalk;
        }
        else
        {
            playerInteractive player = npc.GetComponent<playerInteractive>();
            return player.isFinshTalk;
        }
    }

    //順移
    public void AIteleport(Transform npc, Vector3 pos)
    {
        if (npc != factorys["player"])
        {
            npc.position = pos;
        }
        else
        {
            playerController.playerController_.transform.position = pos;
        }
    }



}


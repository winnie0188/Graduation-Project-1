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
    }
    private void FixedUpdate()
    {
        foreach (var item in factorys)
        {
            item.Value.GetComponent<NPC>().Runnig();
        }
    }

    public void taskMove(task_followNpc task_FollowNpc, Transform npc)
    {
        NPC npcScript = npc.GetComponent<NPC>();
        npcScript.LookAt(task_FollowNpc.endPOS);
        npcScript.GetComponent<NPC>().walkFront();
    }

    public void openTaskTalk(Transform npc, string content)
    {
        NPC npcScript = npc.GetComponent<NPC>();
        npcScript.setContent(content);
    }

    public void closeTaskTalk(Transform npc)
    {
        NPC npcScript = npc.GetComponent<NPC>();
        npcScript.closeConetnt();
    }

    public bool getNPCtalkFinsh(Transform npc)
    {
        NPC npcScript = npc.GetComponent<NPC>();
        return npcScript.isFinshTalk;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiologySystem : MonoBehaviour
{
    public Transform monsterParent;
    [SerializeField] ParentDiction[] eggs;
    [SerializeField] MonsterDinary[] monster;

    //洛洛
    public Lolo Lolo;

    Dictionary<string, ParentDiction> eggDictionary = new Dictionary<string, ParentDiction>();
    DictionaryPool eggPool = new DictionaryPool();

    //重生計算
    List<RebirthData> rebirthDatas = new List<RebirthData>();


    Dictionary<string, MonsterDinary> monsterDictionary = new Dictionary<string, MonsterDinary>();
    DictionaryPool monsterPool = new DictionaryPool();
    Dictionary<Transform, string> monsterKey = new Dictionary<Transform, string>();
    public static BiologySystem biologySystem;


    private void Awake()
    {
        biologySystem = this;

        for (int i = 0; i < monster.Length; i++)
        {
            monsterDictionary.Add(
                monster[i].key,
                new MonsterDinary
                {
                    monster = monster[i].monster,
                    initTime = monster[i].initTime
                }
            );
        }

        for (int i = 0; i < eggs.Length; i++)
        {
            eggDictionary.Add(
                eggs[i].key,
                new ParentDiction()
                {
                    parent = eggs[i].parent,
                    prefab = eggs[i].prefab
                }
            );
        }

        monster = new MonsterDinary[0];
        eggs = new ParentDiction[0];
    }


    public void FixedUpdate()
    {
        foreach (var monsterList in monsterPool.Live)
        {
            foreach (var monster in monsterList.Value)
            {
                if (monster.TryGetComponent<Biology>(out Biology biology))
                {
                    biology.running();
                }
            }
        }
    }

    public void addEgg(string egg, Biology biology)
    {
        if (eggDictionary.TryGetValue(egg, out var parentDiction))
        {
            if (eggPool.Die.TryGetValue(egg, out var dies))
            {
                biology.setEgg(dies[0]);
                dies[0].gameObject.SetActive(true);

                dies.Remove(dies[0]);
            }
            else
            {
                GameObject eggPrefab = Instantiate(parentDiction.prefab);
                biology.setEgg(eggPrefab.transform);

                if (eggPool.Live.TryGetValue(egg, out var lives))
                {
                    lives.Add(eggPrefab.transform);
                }
                else
                {
                    eggPool.Live.Add(egg, new List<Transform>(){
                        eggPrefab.transform
                    });
                }
            }
        }
    }

    public void removeEgg(string egg, Biology biology)
    {
        if (eggPool.Die.TryGetValue(egg, out var dieList))
        {
            dieList.Add(biology.getEgg());
        }
        else
        {
            eggPool.Die.Add(egg, new List<Transform>(){
                biology.getEgg()
            });
        }


        if (eggPool.Live.TryGetValue(egg, out var liveList))
        {
            liveList.Remove(biology.getEgg());
        }
    }


    public string getKey(Transform biology)
    {
        if (monsterKey.TryGetValue(biology, out var key))
        {
            return key;
        }

        return "";
    }


    public void initBiologys()
    {
        foreach (var item in monsterDictionary)
        {
            for (int i = 0; i < item.Value.initTime[0]; i++)
            {
                addBiology(item.Key, Vector3.zero, true);
            }
        }
    }


    #region 物件重生
    public void RebirthFunction(int initRirthTime, int rebirthTime, Transform rebirthTransform)
    {
        StartCoroutine(Rebirth(initRirthTime, rebirthTime, rebirthTransform));
    }

    //重生物件
    IEnumerator Rebirth(int initRirthTime, int rebirthTime, Transform rebirthTransform)
    {
        //存植物重生時間
        RebirthData rebirthData = new RebirthData
        {
            currentTime = initRirthTime,
            key = getKey(rebirthTransform)
        };

        //新增到list
        rebirthDatas.Add(
            rebirthData
        );

        //要更新重生的時間
        for (int i = initRirthTime; i < rebirthTime; i++)
        {
            yield return new WaitForSeconds(1);
            rebirthData.currentTime = i;
        }

        //新增動物
        addBiology(
            rebirthData.key,
            Vector3.zero,
            true
        );

        //移除list
        rebirthDatas.Remove(
            rebirthData
        );
    }
    #endregion


    public void addBiology(string key, Vector3 pos, bool isRandom)
    {
        if (monsterPool.Die.TryGetValue(key, out var dieList))
        {
            Transform monster = dieList[0];
            if (monster.TryGetComponent<Biology>(out Biology biology))
            {
                Vector3 position;
                if (!isRandom)
                {
                    position = pos;
                }
                else
                {
                    Vector3[] GenerateLocation = biology.GetMonster().GenerateLocation;
                    position = GenerateLocation[Random.Range(0, GenerateLocation.Length)];
                    position.x = Random.Range(position.x - 10, position.x + 10);
                    position.z = Random.Range(position.z - 10, position.z + 10);
                }

                if (Physics.Raycast(position + new Vector3(0, 50, 0), Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("block")))
                {
                    position = hit.point;
                }


                biology.transform.position = position + new Vector3(0, 1, 0);
                biology.init(position);

                dieList.Remove(monster);

                addLiveDiction(key, biology.transform);

                monsterKey.Add(biology.transform, key);
            }
        }
        else if (monsterDictionary.TryGetValue(key, out MonsterDinary prefab))
        {
            if (Instantiate(prefab.monster).TryGetComponent<Biology>(out Biology biology))
            {
                Vector3 position;
                if (!isRandom)
                {
                    position = pos;
                }
                else
                {
                    Vector3[] GenerateLocation = biology.GetMonster().GenerateLocation;
                    position = GenerateLocation[Random.Range(0, GenerateLocation.Length)];
                    position.x = Random.Range(position.x - 2, position.x + 2);
                    position.z = Random.Range(position.z - 2, position.z + 2);
                }

                if (Physics.Raycast(position + new Vector3(0, 50, 0), Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("block")))
                {
                    position = hit.point;
                }

                biology.transform.position = position + new Vector3(0, 1, 0);
                biology.transform.SetParent(monsterParent);

                biology.init(position);

                addLiveDiction(key, biology.transform);
                monsterKey.Add(biology.transform, key);
            }
        }
    }


    public void removeBiology(string key, Transform biology)
    {
        if (monsterPool.Die.TryGetValue(key, out var dieList))
        {
            dieList.Add(biology);
        }
        else
        {
            monsterPool.Die.Add(key, new List<Transform>(){
                biology
            });
        }

        removeLiveDiction(key, biology);
        monsterKey.Remove(biology);
    }


    void addLiveDiction(string key, Transform monster)
    {
        if (monsterPool.Live.TryGetValue(key, out var liveList))
        {
            liveList.Add(monster);
        }
        else
        {
            monsterPool.Live.Add(key, new List<Transform>(){
                monster
            });
        }
    }

    void removeLiveDiction(string key, Transform monster)
    {
        if (monsterPool.Live.TryGetValue(key, out var liveList))
        {
            liveList.Remove(monster);
        }
    }
}

[System.Serializable]
public class DictionaryPool
{
    public Dictionary<string, List<Transform>> Live = new Dictionary<string, List<Transform>>();
    public Dictionary<string, List<Transform>> Die = new Dictionary<string, List<Transform>>();
}

[System.Serializable]
public class MonsterDinary
{
    public string key;
    public GameObject monster;

    [Header("初始生成數量")]
    public int[] initTime;
}

[System.Serializable]
public class ParentDiction
{
    public string key;
    public Transform parent;
    public GameObject prefab;
}




[System.Serializable]
public class BiologyData
{
    public float maxHp;
    public float Hp;
}


[System.Serializable]
public class floatData
{
    public float max;
    public float current;
}


[System.Serializable]
public class Lolo : BiologyData
{
    //攻擊目標
    public List<Transform> attTarge = new List<Transform>();
    public bool attMode;
    [Header("技能冷卻")]
    public floatData[] skillTime;

    [Header("活動冷卻")]
    public floatData ActiveCooldown;

    [Header("free冷卻")]
    public floatData FreeCooldown;

    [SerializeField] Text HpTxt;
    [SerializeField] Transform HpLine;

    public void Cooldown(bool isAtt)
    {
        if (isAtt)
        {
            FreeCooldown.current = 0;
        }
        else
        {
            if ((FreeCooldown.current += Time.deltaTime) > FreeCooldown.max)
            {
                FreeCooldown.current = FreeCooldown.max;
            }
        }

        if ((ActiveCooldown.current += Time.deltaTime) > ActiveCooldown.max)
        {
            ActiveCooldown.current = ActiveCooldown.max;
        }
    }

    public void CoolUp()
    {
        FreeCooldown.current = 0;
        if ((ActiveCooldown.current -= Time.deltaTime) < 0)
        {
            ActiveCooldown.current = 0;
        }
    }

    public bool canAtt(NPCstate nPCstate)
    {
        if (ActiveCooldown.current < ActiveCooldown.max && FreeCooldown.current < FreeCooldown.max && nPCstate == NPCstate.WALK)
        {
            return true;
        }
        return false;
    }

    public void UpdateLoloHp(float hp)
    {
        if (Hp + hp > maxHp || Hp + hp < 0)
        {
            return;
        }

        Hp += hp;

        float HpPerson = Hp / maxHp;

        HpTxt.text = (HpPerson * 100) + "%";
        HpLine.localScale = new Vector3(1 - HpPerson, 1, 1);
    }
}



[System.Serializable]
public class Monster : BiologyData
{
    [Header("到這範圍會開始追玩家")]
    public int range;
    [Header("到這範圍會開始攻擊")]
    public int attactRange;

    [Header("掉落物")]
    public Drops[] drops;

    [Header("生成位置")]
    public Vector3[] GenerateLocation;

    [Header("技能")]
    public SkillData[] monsterSkills;

    [Header("是否為主動發動技能")]
    public bool counterAttack;

    [Header("要守護的東西:蛋parent，偵測子變少就開扁")]
    public string eggParent;
}

[System.Serializable]
public class SkillData
{

    public MonsterSkill monsterSkills;

    //時間
    [Header("別調這是要動態改的，預設為0")]
    public float coolZero;
}


[System.Serializable]
public class SKILL_NONE
{

}

[System.Serializable]
public class SKILL_NORMAL
{

}

[System.Serializable]
public class SKILL_SHOOT
{
    public string shoot;
}

[System.Serializable]
public class SKILL_COLLISION
{
    public float speedUp;
}

[System.Serializable]
public class MonthBuff
{
    public int[] month;
    public buff[] buff;
}

[System.Serializable]
public class buff
{
    public string buffKey;
    public float value;
    [Header("持續時間")]
    public float duration;
}

[System.Serializable]
public class SKILL_ROLL
{

}

[System.Serializable]
public class SKILL_BUFF
{
    public buff[] buffs;
}

[System.Serializable]
public class SKILL_FIRE
{
    [Header("潛入地底")]
    public bool isDive;
    [Header("丟完是否逃跑")]
    public bool isFireEndRunAwake;
    [Header("是否彈跳")]
    public bool bounce;
    public string bullect;
    public Drops[] drops;

}

[System.Serializable]
public class SKILL_RUNAWAY
{
    public Drops[] drops;
}

[System.Serializable]
public class SKILL_HOVER
{
    [Header("下落攻擊/是/否:飛在空中攻擊")]
    public bool whereabouts;
    [Header("攻擊間隔")]
    public float interval;
}



[System.Serializable]
public class SKILL_FROMTO
{
    //抓人高度
    public int height;
}

[System.Serializable]
public class SKILL_DEFENSE
{

}

[System.Serializable]
public class SKILL_SUMMON
{
    public Vector3[] position;
    public string monsterKey;
    public string bullectKey;
}



public enum SkillType
{
    //無
    NONE,
    //一般
    NORMAL,
    //噴射
    SHOOT,
    //衝撞
    COLLISION,
    //翻滾
    ROLL,
    //BUFF
    BUFF,
    //投擲
    FIRE,
    //逃跑
    RUNAWAY,
    //盤旋
    HOVER,
    //從特定位置衝到某位置
    FROMTO,
    //防禦
    DEFENSE,
    //生怪
    SUMMON
}

[System.Serializable]
public class Drops
{
    //掉落物
    public BagItem bagItems;
    //掉落機率
    public int person;
    public IntData count;
}


[System.Serializable]
public class IntData
{
    public int current;
    public int max;
}
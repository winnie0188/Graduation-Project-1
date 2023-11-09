using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuppliesSystem : MonoBehaviour
{

    [Header("會被clean")]
    [SerializeField] List<MonsterDinary> supplies = new List<MonsterDinary>();
    [SerializeField] Transform suppliesParent;
    Dictionary<string, MonsterDinary> suppliesDiction = new Dictionary<string, MonsterDinary>();
    Dictionary<Transform, string> suppliesKey = new Dictionary<Transform, string>();
    DictionaryPool suppliePool = new DictionaryPool();

    List<RebirthData> rebirthDatas = new List<RebirthData>();

    public static SuppliesSystem suppliesSystem;

    private void Awake()
    {
        suppliesSystem = this;

        for (int i = 0; i < supplies.Count; i++)
        {
            if (suppliesDiction.TryGetValue(supplies[i].key, out var monsterDinary))
            {
                print("已使用過");
            }
            else
            {
                suppliesDiction.Add(
                    supplies[i].key,
                    new MonsterDinary
                    {
                        monster = supplies[i].monster,
                        initTime = supplies[i].initTime
                    }
                );
            }
        }
        supplies.Clear();

    }

    public void FixedUpdate()
    {
        Dictionary<string, List<Transform>> Live = suppliePool.Live;

        foreach (var item in Live)
        {
            foreach (var supplies in item.Value)
            {
                if (supplies.TryGetComponent<Supplies>(out Supplies s))
                {
                    s.running();
                }
            }
        }
    }


    public void initAllSupplies()
    {
        foreach (var item in suppliesDiction)
        {
            for (int i = 0; i < item.Value.initTime[0]; i++)
            {
                AddSupplies(item.Key, Vector3.zero, true);
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

        //新增植物
        AddSupplies(
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


    public string getKey(Transform item)
    {
        if (suppliesKey.TryGetValue(item, out var key))
        {
            return key;
        }

        return "";
    }

    //isNotDefault = 50高空射線
    public void AddSupplies(string key, Vector3 pos, bool isRandom)
    {
        if (suppliePool.Die.TryGetValue(key, out var dieList))
        {
            Transform item = dieList[0];
            if (item.TryGetComponent<Supplies>(out Supplies supplies))
            {
                Vector3 position;
                if (!isRandom)
                {
                    position = pos;
                }
                else
                {
                    Vector3[] GenerateLocation = supplies.GenerateLocation;
                    position = GenerateLocation[Random.Range(0, GenerateLocation.Length)];
                    position.x = Random.Range(position.x - 10, position.x + 10);
                    position.z = Random.Range(position.z - 10, position.z + 10);
                }

                if (Physics.Raycast(position + new Vector3(0, 50, 0), Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("block")))
                {
                    position = hit.point;
                }

                supplies.init(position);

                dieList.Remove(supplies.transform);

                addLiveDiction(key, supplies.transform);

                suppliesKey.Add(supplies.transform, key);
            }
        }
        else if (suppliesDiction.TryGetValue(key, out var prefab))
        {
            if (Instantiate(prefab.monster).TryGetComponent<Supplies>(out Supplies supplies))
            {
                Vector3 position;
                if (!isRandom)
                {
                    position = pos;
                }
                else
                {
                    Vector3[] GenerateLocation = supplies.GenerateLocation;
                    position = GenerateLocation[Random.Range(0, GenerateLocation.Length)];

                    position.x = Random.Range(position.x - 10, position.x + 10);
                    position.z = Random.Range(position.z - 10, position.z + 10);
                }

                if (Physics.Raycast(position + new Vector3(0, 50, 0), Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("block")))
                {
                    position = hit.point;
                }

                supplies.transform.SetParent(suppliesParent);

                supplies.init(position);

                addLiveDiction(key, supplies.transform);
                suppliesKey.Add(supplies.transform, key);
            }
        }
    }


    public void removeSupplies(string key, Transform item)
    {
        if (suppliePool.Die.TryGetValue(key, out var dieList))
        {
            dieList.Add(item);
        }
        else
        {
            suppliePool.Die.Add(key, new List<Transform>(){
                item
            });
        }

        removeLiveDiction(key, item);
        // monsterKey.Remove(item);
    }


    void addLiveDiction(string key, Transform supplies)
    {
        if (suppliePool.Live.TryGetValue(key, out var liveList))
        {
            liveList.Add(supplies);
        }
        else
        {
            suppliePool.Live.Add(key, new List<Transform>(){
                supplies
            });
        }
    }

    void removeLiveDiction(string key, Transform item)
    {
        if (suppliePool.Live.TryGetValue(key, out var liveList))
        {
            liveList.Remove(item);
        }
    }

}

//之後存檔要記錄重生的東西
[System.Serializable]
public class RebirthData
{
    public int currentTime;
    public string key;
}
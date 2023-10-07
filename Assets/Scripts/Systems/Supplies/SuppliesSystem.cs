using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuppliesSystem : MonoBehaviour
{

    [Header("會被clean")]
    [SerializeField] List<MonsterDinary> supplies = new List<MonsterDinary>();
    [SerializeField] Transform suppliesParent;
    Dictionary<string, GameObject> suppliesDiction = new Dictionary<string, GameObject>();
    Dictionary<Transform, string> suppliesKey = new Dictionary<Transform, string>();
    DictionaryPool suppliePool = new DictionaryPool();

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
                suppliesDiction.Add(supplies[i].key, supplies[i].monster);
            }
        }
        supplies.Clear();

    }

    public string getKey(Transform item)
    {
        if (suppliesKey.TryGetValue(item, out var key))
        {
            return key;
        }

        return "";
    }

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
                }

                if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("block")))
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
            if (Instantiate(prefab).TryGetComponent<Supplies>(out Supplies supplies))
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
                }

                if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("block")))
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

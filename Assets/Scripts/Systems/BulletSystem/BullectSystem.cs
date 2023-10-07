using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullectSystem : MonoBehaviour
{
    [Header("之後會歸0")]
    [SerializeField] bullectData[] bullectDatas;
    public Dictionary<string, bullectData> bullectDictions = new Dictionary<string, bullectData>();
    public static BullectSystem bullectSystem;


    private void Awake()
    {
        bullectSystem = this;

        for (int i = 0; i < bullectDatas.Length; i++)
        {
            bullectDictions.Add(bullectDatas[i].key, bullectDatas[i]);
        }

        bullectDatas = new bullectData[0];
    }

    public void fire(string key, Transform parent, Vector3 end, Transform targe)
    {
        Vector3 start = parent.position;
        Vector3 direction = (end - start).normalized;
        start = start + direction * 0.5f;


        if (bullectDictions.TryGetValue(key, out bullectData data))
        {
            if (!data.bullectActiveData.isEndSummor)
            {
                end = start + direction * 1.5f;
            }

            if (data.bullectActiveData.isGroundSummor)
            {
                Vector3 summorPos;
                if (data.bullectActiveData.isEndSummor)
                {
                    summorPos = end;
                }
                else
                {
                    summorPos = parent.transform.position;
                }


                if (Physics.Raycast(summorPos, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("block")))
                {
                    start = hit.point + Vector3.up * 0.6f;
                    end = start + Vector3.up;
                }
            }



            Transform bullect = null;
            for (int i = 0; i < data.bullectParent.childCount; i++)
            {
                if (!data.bullectParent.GetChild(i).gameObject.activeSelf)
                {
                    bullect = data.bullectParent.GetChild(i);
                    setGameObject(bullect, start, end);
                    data.bullectParent.GetChild(i).gameObject.SetActive(true);
                    break;
                }
            }

            if (bullect == null)
            {
                bullect = Instantiate(data.bullect).transform;
                bullect.SetParent(data.bullectParent);
                setGameObject(bullect, start, end);
            }

            StartCoroutine(bulletUpdate(data.bullectActiveData, bullect, parent, targe));
        }

    }

    IEnumerator bulletUpdate(bullectActiveData bullectActive, Transform bullect, Transform parent, Transform targe)
    {
        float currentDeletTime = 0;

        Transform enemy = null;

        bool isAtt = false;

        float cooltime = 0;

        while (currentDeletTime < bullectActive.maxDeletTime)
        {
            if (!bullectActive.isGroundSummor)
            {
                //改變圖像朝向
                bullect.GetChild(0).rotation = playerController.playerController_.transform.rotation;
            }

            yield return null;
            float deltaTime = Time.deltaTime;
            currentDeletTime += deltaTime;

            bullect.transform.Translate(Vector3.forward * bullectActive.speed * deltaTime);

            if ((bullect.position - targe.position).magnitude < bullectActive.distance)
            {
                enemy = targe;
            }

            if (enemy != null)
            {
                if (bullectActive.isBack)
                {
                    if ((bullect.position - parent.position).magnitude < 1)
                    {
                        currentDeletTime = bullectActive.maxDeletTime;
                    }
                    LookAt(bullect, parent.position);
                }

                if (!isAtt)
                {
                    if (bullectActive.buff.Length > 0)
                    {
                        buff buff = bullectActive.buff[0];
                        BuffSystem.buffSystem.buff(
                            buff.buffKey,
                            enemy,
                            buff.value,
                            buff.duration
                        );
                    }

                    if (enemy.TryGetComponent<playerController>(out var player))
                    {
                        player.HpUpdate(-bullectActive.power);
                    }
                    else if (enemy.TryGetComponent<NPC>(out var Lolo))
                    {
                        BiologySystem.biologySystem.Lolo.UpdateLoloHp(-bullectActive.power);
                    }

                    if (bullectActive.isKnock)
                    {
                        enemy.position += new Vector3(0, 2, 0);
                    }

                    if (bullectActive.isTocuchGone)
                    {
                        currentDeletTime = bullectActive.maxDeletTime;
                    }


                    enemy = null;
                    isAtt = true;
                }
                else
                {
                    if ((cooltime += deltaTime) >= bullectActive.maxCoolTime)
                    {
                        cooltime = 0;
                        isAtt = false;
                    }
                }

            }
        }

        bullect.gameObject.SetActive(false);
    }

    void setGameObject(Transform gameobject, Vector3 pos, Vector3 end)
    {
        gameobject.position = pos;
        LookAt(gameobject, end);
    }

    void LookAt(Transform start, Vector3 endPOS)
    {
        Vector3 endLine = endPOS;
        Vector3 startLine = start.position;
        start.transform.rotation =
        Quaternion.Slerp(
            start.transform.rotation,
            Quaternion.LookRotation(
                new Vector3(endLine.x, 0, endLine.z) - new Vector3(startLine.x, 0, startLine.z)
            ),
            100
        );
    }
}

[System.Serializable]
public class bullectData
{
    public string key;
    public Transform bullectParent;
    public GameObject bullect;
    public bullectActiveData bullectActiveData;
}

[System.Serializable]
public class bullectActiveData
{
    public int power;
    public float speed;
    [Header("是否飛回來")]
    public bool isBack;
    [Header("觸發buff")]
    public buff[] buff;
    [Header("地面生成")]
    public bool isGroundSummor;
    [Header("是否在終點生成")]
    public bool isEndSummor;
    [Header("是否擊飛")]
    public bool isKnock;
    [Header("碰到則消失")]
    public bool isTocuchGone;
    public float maxCoolTime;
    public float maxDeletTime;
    [Header("偵測前方距離")]
    public float distance;
}

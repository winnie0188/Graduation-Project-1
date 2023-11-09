using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootSystem : MonoBehaviour
{
    [Header("之後會歸0")]
    [SerializeField] ShootData[] shootDatas;
    public Dictionary<string, ShootData> shootDictions = new Dictionary<string, ShootData>();

    public static ShootSystem shootSystem;

    private void Awake()
    {
        shootSystem = this;

        for (int i = 0; i < shootDatas.Length; i++)
        {
            shootDictions.Add(shootDatas[i].key, shootDatas[i]);
        }

        shootDatas = new ShootData[0];
    }


    public void shoot(string key, Transform parent, Vector3 end, int power, float continued, Transform enemyTransform, Vector3 Start)
    {
        Vector3 start;

        if (Start != Vector3.zero)
        {
            start = Start;
        }
        else
        {
            start = parent.position;
        }

        Vector3 direction = (end - start).normalized;
        start = start + direction * 0.5f;
        end = start + direction * 1.5f;

        if (shootDictions.TryGetValue(key, out ShootData shootData))
        {
            Transform shoot = null;
            for (int i = 0; i < shootData.parent.childCount; i++)
            {
                if (!shootData.parent.GetChild(i).gameObject.activeSelf)
                {
                    shoot = shootData.parent.GetChild(i);
                    setGameObject(shoot, start, end);
                    shoot.gameObject.SetActive(true);
                    break;
                }
            }

            if (shoot == null)
            {
                shoot = Instantiate(shootData.prefab).transform;
                shoot.SetParent(shootData.parent);
                setGameObject(shoot, start, end);
            }

            buff[] buff = shootData.buff;

            StartCoroutine(ShootUpdate(continued, shoot, start, direction, power, enemyTransform, buff));

        }

    }


    IEnumerator ShootUpdate(float continued, Transform shoot, Vector3 start, Vector3 direction, float power, Transform enemyTransform, buff[] buffs)
    {
        float currentDeletTime = 0;
        //長度
        float length = 1.5f;

        Vector3 end = Vector3.zero;

        float lengthIncrement = 0.5f; // 控制长度的增加量
        float waitTime = 0.01f; // 控制循环速度的等待时间

        while (currentDeletTime < (continued / 2.0f))
        {
            yield return new WaitForSeconds(waitTime);
            currentDeletTime += waitTime;

            length += lengthIncrement;
            end = start + direction * length;

            setGameObject(shoot, start, end);


            if ((enemyTransform.position - end).magnitude < 1)
            {
                if (enemyTransform.TryGetComponent<playerController>(out playerController player))
                {
                    player.HpUpdate(power * -1);
                }
                else if (enemyTransform.TryGetComponent<NPC>(out NPC npc))
                {
                    BiologySystem.biologySystem.Lolo.UpdateLoloHp(power * -1);
                }

                if (buffs.Length > 0)
                {
                    buff buff = buffs[0];
                    BuffSystem.buffSystem.buff(
                        buff.buffKey,
                        enemyTransform,
                        buff.value,
                        buff.duration
                    );
                }

                currentDeletTime = (continued / 2.0f);
            }
        }


        currentDeletTime = 0;

        while (currentDeletTime < (continued / 2.0f))
        {
            yield return new WaitForSeconds(waitTime);
            currentDeletTime += waitTime;

            length -= lengthIncrement;
            end = start + direction * length;

            setGameObject(shoot, start, end);

            if ((shoot.position - start).magnitude < 1)
            {
                currentDeletTime = continued;
            }
        }

        shoot.gameObject.SetActive(false);
    }


    void setGameObject(Transform gameobject, Vector3 pos, Vector3 end)
    {
        gameobject.position = pos;
        var length = Vector3.Distance(pos, end);

        gameobject.localScale = new Vector3(1, 1, length);
        gameobject.position = (pos + end) / 2;
        gameobject.LookAt(end);
        gameobject.Rotate(0, 0, 0);
    }
}

[System.Serializable]
public class ShootData : ParentDiction
{
    public buff[] buff;
}
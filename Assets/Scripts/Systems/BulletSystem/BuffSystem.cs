using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    [Header("之後會歸0")]
    [SerializeField] buffData[] buffDatas;
    public Dictionary<string, float> buffDictions = new Dictionary<string, float>();
    public Dictionary<string, IEnumerator> buffroutine = new Dictionary<string, IEnumerator>();
    public static BuffSystem buffSystem;

    private void Awake()
    {
        buffSystem = this;

        for (int i = 0; i < buffDatas.Length; i++)
        {
            buffDictions.Add(buffDatas[i].key, buffDatas[i].value);
        }

        buffDatas = new buffData[0];
    }


    public void buff(string key, Transform targe, float power, float time)
    {
        if (buffDictions.TryGetValue(key, out var val))
        {
            if (key.Equals("p_bleed"))
            {
                if (val <= power)
                {
                    if (buffroutine.TryGetValue(key, out IEnumerator PreRoutine))
                    {
                        StopCoroutine(PreRoutine);
                        buffroutine.Remove(key);
                    }
                }

                IEnumerator NewRoutine = p_bleed(key, time, targe, power);
                buffroutine.Add(key, NewRoutine);
                StartCoroutine(NewRoutine);
            }
            else if (key.Equals("p_speed"))
            {
                if (val == 0)
                {
                    StartCoroutine(p_speed(key, time, power, targe));
                }
            }
            else if (key.Equals("p_poisoned"))
            {
                if (val <= power)
                {
                    if (buffroutine.TryGetValue(key, out IEnumerator PreRoutine))
                    {
                        StopCoroutine(PreRoutine);
                        buffroutine.Remove(key);
                    }
                }

                IEnumerator NewRoutine = p_poisoned(key, time, targe, power);
                buffroutine.Add(key, NewRoutine);
                StartCoroutine(NewRoutine);
            }
            else if (key.Equals("p_dizziness"))
            {
                if (val <= power)
                {
                    if (buffroutine.TryGetValue(key, out IEnumerator PreRoutine))
                    {
                        StopCoroutine(PreRoutine);
                        buffroutine.Remove(key);
                    }
                }

                IEnumerator NewRoutine = p_dizziness(key, time, targe, power);
                buffroutine.Add(key, NewRoutine);
                StartCoroutine(NewRoutine);
            }
            else if (key.Equals("p_knock"))
            {
                if (val == 0)
                {
                    StartCoroutine(p_knock(targe, power, time, key));
                }
            }
        }
    }

    IEnumerator p_bleed(string key, float time, Transform targe, float power)
    {
        buffDictions[key] = power;

        for (int i = 0; i < time; i++)
        {
            if (targe.TryGetComponent<playerController>(out playerController player))
            {
                player.HpUpdate(-power);
            }
            else if (targe.TryGetComponent<NPC>(out NPC Lolo))
            {
                BiologySystem.biologySystem.Lolo.UpdateLoloHp(-power);
            }
            yield return new WaitForSeconds(1);
        }

        buffDictions[key] = 0;
    }

    IEnumerator p_speed(string key, float time, float power, Transform targe)
    {
        buffDictions[key] = power;

        if (targe.TryGetComponent<playerController>(out var player))
        {
            playerController.playerController_.setSpeedBuff(-power);
            print("減速");
            //幾秒後恢復
            yield return new WaitForSeconds(time);
            print("恢復");
            playerController.playerController_.setSpeedBuff(power);
        }
        else if (targe.TryGetComponent<NPC>(out var Lolo))
        {
            Lolo.setSpeedBuff(-power);
            //幾秒後恢復
            yield return new WaitForSeconds(time);
            Lolo.setSpeedBuff(power);
        }

        //幾秒後才能再次觸發
        yield return new WaitForSeconds(5);
        buffDictions[key] = 0;
    }

    IEnumerator p_poisoned(string key, float time, Transform targe, float power)
    {
        buffDictions[key] = power;

        for (int i = 0; i < time; i++)
        {
            if (targe.TryGetComponent<playerController>(out playerController player))
            {
                player.HpUpdate(-power);
            }
            else if (targe.TryGetComponent<NPC>(out NPC Lolo))
            {
                BiologySystem.biologySystem.Lolo.UpdateLoloHp(-power);
            }
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(5);
        buffDictions[key] = 0;
    }

    IEnumerator p_dizziness(string key, float time, Transform targe, float power)
    {
        buffDictions[key] = power;

        if (targe.TryGetComponent<playerController>(out playerController player))
        {
            player.setSpeedDizziness(0);
            for (int i = 0; i < time; i++)
            {
                player.HpUpdate(-power);
                yield return new WaitForSeconds(1);
            }
            player.setSpeedDizziness(1);
        }
        else if (targe.TryGetComponent<NPC>(out NPC Lolo))
        {
            Lolo.setSpeedDizziness(0);
            for (int i = 0; i < time; i++)
            {
                BiologySystem.biologySystem.Lolo.UpdateLoloHp(-power);
                yield return new WaitForSeconds(1);
            }
            Lolo.setSpeedDizziness(1);
        }

        buffDictions[key] = 0;
    }


    IEnumerator p_knock(Transform targe, float power, float time, string key)
    {
        buffDictions[key] = power;
        if (targe.TryGetComponent<playerController>(out var player))
        {
            player.setKnock(true);

            player.rigi.AddForce(Vector3.up * power, ForceMode.Impulse);

            do
            {
                yield return new WaitForSeconds(time);
            } while (Physics.RaycastAll(targe.position, Vector3.down, 3f, LayerMask.GetMask("block")).Length <= 0);
            player.setKnock(false);
        }
        else
        {
            targe.GetComponent<Rigidbody>().AddForce(Vector3.up * power, ForceMode.Impulse);
        }

        buffDictions[key] = 0;
    }
}


[System.Serializable]
public class buffData
{
    public string key;
    public float value;
}
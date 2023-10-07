using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class NPC : MonoBehaviour
{
    Transform image;

    public float speed;
    float speedBuff = 1;
    float speedDizziness = 1;
    public Rigidbody rigi;


    #region 頭頂對話

    [Header("這邊是處理頭頂框的")]
    public Transform talkView;

    [SerializeField] TMP_Text content;
    public bool isFinshTalk = true;
    //僵直時間
    [SerializeField] float Stiff;

    #endregion

    #region 要接的任務
    [Header("可接的任務")]
    [SerializeField] taskItem taskItem;
    [Header("驚嘆號")]
    [SerializeField] Transform exclamation;

    #endregion

    public bool isTask;
    Vector3 prePos;
    Animator ani;

    [Space(50)]
    [SerializeField] NPCdata NPCdata;
    [Space(50)]
    [SerializeField] NPCstate NPCstate;


    [Header("other會用到")]
    Vector3 centerPos;
    [Header("之後會取data裡的range")]
    [SerializeField] int range;

    [Header("紀錄現在過的生活")]
    public int currentLive;


    [Header("測試用")]
    [SerializeField] int attState;

    floatData rest;
    floatData walk;



    private void Awake()
    {
        rest = new floatData()
        {
            current = 0,
            max = 10
        };

        walk = new floatData()
        {
            current = 0,
            max = 3
        };

        image = transform.GetChild(0);
        rigi = GetComponent<Rigidbody>();
        ani = image.GetChild(0).GetChild(0).GetComponent<Animator>();
        setCenter(centerPos);
    }



    //給DatSystem用的
    public void receivedSignal(int day, int start)
    {
        if (NPCdata.dictionary.TryGetValue(start, out int index))
        {
            //DIO
            if (NPCdata.NPCType == NPCType.DIO)
            {
                var activePos = NPCdata.npcOtherData.activePos[Random.Range(0, NPCdata.npcOtherData.activePos.Length)];

                setCenter(activePos.pos);
                range = activePos.range;

                NPCstate = NPCstate.WALK;
            }
            else
            {
                if (index == 0)
                {
                    if (NPCdata.NPCLives.Length > 1)
                    {
                        //來決定墨菲當天要做甚麼
                        int person = Random.Range(1, 101);
                        int currentperson = 0;
                        int temp = -1;

                        for (int i = 0; i < NPCdata.NPCLives.Length; i++)
                        {
                            //特定日字:洛洛
                            if (NPCdata.NPCLives[i].specialDay.Length > 0)
                            {
                                for (int j = 0; j < NPCdata.NPCLives[i].specialDay.Length; j++)
                                {
                                    if (NPCdata.NPCLives[i].specialDay[j] == day)
                                    {
                                        temp = i;
                                        break;
                                    }
                                }
                            }
                            //概率切換 :墨菲
                            else if (NPCdata.NPCLives[i].person != 0)
                            {
                                currentperson += NPCdata.NPCLives[i].person;
                                if (currentperson <= person)
                                {
                                    temp = i;
                                    break;
                                }
                            }
                            //特定日子以外的日子:洛洛
                            else
                            {
                                if (temp != -1)
                                {
                                    temp = i;
                                    break;
                                }
                            }
                        }
                    }
                }

                int RadomPerson = Random.Range(1, 101);
                int CurrentPerson = 0;
                //正常NPC都是執行這個

                LifePerson[] lives = NPCdata.NPCLives[currentLive].timePeriods[index].lifePerson;
                for (int i = 0; i < lives.Length; i++)
                {
                    CurrentPerson += lives[i].person;
                    if (RadomPerson <= CurrentPerson)
                    {
                        if (lives[i].life == NpcLifeType.HOME)
                        {
                            setCenter(NPCdata.homePs);
                            NPCstate = NPCstate.NONE;
                        }
                        else if (lives[i].life == NpcLifeType.WORK)
                        {
                            setCenter(NPCdata.ShopPos[0]);
                            NPCstate = NPCstate.WORK;
                        }
                        else if (lives[i].life == NpcLifeType.OTHER)
                        {
                            var length = NPCdata.hauntingPlace.Length;
                            ActivePos activePos = NPCdata.hauntingPlace[Random.Range(0, length)];

                            setCenter(activePos.pos);
                            range = activePos.range;
                            NPCstate = NPCstate.WALK;
                        }

                        //洛洛
                        else if (lives[i].life == NpcLifeType.MASS)
                        {
                            setCenter(NPCdata.ShopPos[1]);
                            NPCstate = NPCstate.MASS;
                        }


                        //墨菲
                        else if (lives[i].life == NpcLifeType.LOUNGE)
                        {
                            setCenter(NPCdata.npcOtherData.NPCspecialPlace[0].loungePlace[0]);
                            NPCstate = NPCstate.NONE;
                        }
                        else if (lives[i].life == NpcLifeType.FREE)
                        {
                            setCenter(NPCdata.npcOtherData.NPCspecialPlace[0].freePlace[0].pos);
                            NPCstate = NPCstate.WALK;
                        }
                        else if (lives[i].life == NpcLifeType.UNCHANGED)
                        {

                        }

                        break;
                    }
                }
            }
        }
    }

    void setCenter(Vector3 center)
    {
        // 0.8126594/
        center.y += 0.8126594f;
        this.centerPos = center;
    }

    //相當於start
    public void Init()
    {
        bool isNPClive = true;
        //如果是隨從
        if (NPCdata.NPCType == NPCType.ENTOURAGE)
        {
            isNPClive = false;
        }
        //洛洛
        else if (NPCdata.NPCType == NPCType.PARTNER)
        {

        }
        //肥啾
        else if (NPCdata.NPCType == NPCType.FATDOVE)
        {
            isNPClive = false;
        }
        //商人
        else if (NPCdata.NPCType == NPCType.MERCHANT)
        {

        }
        //墨菲
        else if (NPCdata.NPCType == NPCType.WEEKTASK)
        {

        }
        //迪奧
        else if (NPCdata.NPCType == NPCType.DIO)
        {
            isNPClive = false;

            int j = -2;
            for (int i = 0; i < 12; i++)
            {
                j += 2;
                NPCdata.dictionary.Add(j, i);
            }
        }

        if (isNPClive)
        {
            for (int i = 0; i < NPCdata.NPCLives.Length; i++)
            {
                for (int j = 0; j < NPCdata.NPCLives[i].timePeriods.Length; j++)
                {
                    int start = NPCdata.NPCLives[i].timePeriods[j].start;
                    if (!NPCdata.dictionary.TryGetValue(start, out var s))
                    {
                        NPCdata.dictionary.Add(NPCdata.NPCLives[i].timePeriods[j].start, j);
                    }

                }
            }
        }
    }


    //相當於update
    public void Runnig()
    {
        image.rotation = playerController.playerController_.transform.rotation;
        UpdateTaskUi();
        Ani();

        if (isTask)
        {
            return;
        }

        if (NPCstate == NPCstate.FOLLOW)
        {
            Vector3 playerPos = NPCdata.npcOtherData.FolowTransform.position;

            float distance = (NPCdata.npcOtherData.FolowTransform.position - transform.position).magnitude;

            if (distance > 20)
            {
                transform.position = playerPos;
            }
            else if (distance > 5)
            {
                LookAt(playerPos);
                walkFront();
            }
        }
        else if (NPCstate == NPCstate.WALK)
        {
            //如果沒被召喚則維持
            if (!summonCompanion())
            {
                RadomWalk();
            }
        }
        else if (NPCstate == NPCstate.MASS)
        {
            idle();
            if (NPCdata.NPCType == NPCType.PARTNER)
            {
                UpSkill();
                UpCool();
            }
        }
        else if (NPCstate == NPCstate.WORK)
        {
            idle();
            if (NPCdata.NPCType == NPCType.PARTNER)
            {
                UpSkill();
                UpCool();
            }
        }
        else if (NPCstate == NPCstate.NONE)
        {
            idle();
        }
        image.rotation = playerController.playerController_.transform.rotation;
    }

    void idle()
    {
        if ((transform.position - centerPos).magnitude > 5)
        {
            transform.position = centerPos;
        }
    }

    #region LOLO
    //BUFF=================
    public void setSpeedBuff(float buff)
    {
        speedBuff += buff;
    }

    public void setSpeedDizziness(float dizziness)
    {
        speedDizziness = dizziness;
    }

    //BUFF=================
    bool summonCompanion()
    {
        if (NPCdata.NPCType == NPCType.PARTNER)
        {
            var attSystem = BiologySystem.biologySystem.Lolo;
            if (attSystem.attMode && attSystem.canAtt(NPCstate))
            {
                Vector3 playerPos = playerController.playerController_.transform.position;
                float distance = (NPCdata.npcOtherData.FolowTransform.position - transform.position).magnitude;

                bool isAtt = attSystem.attTarge.Count > 0;

                if (distance > 15)
                {
                    transform.position = playerPos;
                    attSystem.attTarge.Clear();
                }
                else if (isAtt)
                {
                    if (attSystem.attTarge[0] != null || attSystem.attTarge[0].gameObject.activeSelf)
                    {
                        if ((attSystem.attTarge[0].position - transform.position).magnitude > 10)
                        {
                            LookAt(attSystem.attTarge[0].position);
                            walkFront();
                        }
                        Attack(attSystem.attTarge[0].position, attSystem.skillTime);
                    }
                    else
                    {
                        attSystem.attTarge.RemoveAt(0);
                    }
                }
                else if (distance > 5)
                {
                    LookAt(playerPos);
                    walkFront();
                }

                attSystem.Cooldown(isAtt);

                UpSkill();

                return true;
            }
            else
            {
                attSystem.CoolUp();
            }
        }
        return false;
    }

    void UpCool()
    {
        Lolo lolo = BiologySystem.biologySystem.Lolo;
        lolo.CoolUp();
    }

    void UpSkill()
    {
        Lolo lolo = BiologySystem.biologySystem.Lolo;
        for (int i = 0; i < lolo.skillTime.Length; i++)
        {
            if ((lolo.skillTime[i].current -= Time.deltaTime) <= 0)
            {
                lolo.skillTime[i].current = 0;
            }
        }
    }

    public void Attack(Vector3 monsterPos, floatData[] floatData)
    {
        if (attState == 0)
        {
            物理攻擊(monsterPos, floatData[0]);
        }
        else if (attState == 1)
        {
            攻_驅邪(floatData[1]);
        }
        else if (attState == 2)
        {
            攻_迪奧的祝福(floatData[2]);
        }
        else if (attState == 3)
        {
            補_神聖洗禮(floatData[3]);
        }
    }

    public void 物理攻擊(Vector3 monsterPos, floatData floatData)
    {
        // if (floatData.current <= 0)
        // {
        //     BullectSystem.bullectSystem.fire(NPCdata.npcOtherData.bullect[0], transform, monsterPos);
        //     floatData.current = floatData.max;
        // }
    }

    public void 攻_驅邪(floatData floatData)
    {

    }

    public void 攻_迪奧的祝福(floatData floatData)
    {

    }

    public void 補_神聖洗禮(floatData floatData)
    {

    }


    #endregion

    public void Ani()
    {
        if (transform.position != prePos)
        {
            ani.SetBool("WALK", true);
        }
        else
        {
            ani.SetBool("WALK", false);
        }


        prePos = transform.position;
    }


    public void RadomWalk()
    {

        if ((walk.current += Time.deltaTime) < walk.max)
        {
            float distance = (transform.position - centerPos).magnitude;
            if (distance > range + 10)
            {
                transform.position = centerPos;
            }
            else if (((transform.position + transform.forward) - centerPos).magnitude >= range)
            {
                transform.Rotate(new Vector3(0, Random.Range(10, 45), 0), Space.Self);
            }
            else if (distance < range)
            {
                if (Physics.Raycast(transform.position, transform.forward, 10))
                {
                    transform.Rotate(new Vector3(0, Random.Range(10, 45), 0), Space.Self);
                }
                else
                {
                    walkFront();
                }
            }

            image.rotation = playerController.playerController_.transform.rotation;
        }
        else if ((rest.current += Time.deltaTime) < rest.max)
        {
            //閒置
            transform.Rotate(new Vector3(0, Random.Range(-45, 45), 0), Space.Self);
        }
        else
        {
            //兩個跑完則重新再來一次
            walk.current = 0;
            rest.current = 0;
        }

    }

    public void LookAt(Vector3 endPOS)
    {
        Vector3 endLine = endPOS;
        Vector3 startLine = transform.position;
        transform.rotation =
        Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(
                new Vector3(endLine.x, 0, endLine.z) - new Vector3(startLine.x, 0, startLine.z)
            ),
            100
        );
    }

    public void walkFront()
    {
        Vector3 v = rigi.velocity;
        v.x = 0;
        v.z = 0;

        rigi.velocity = v;

        Vector3 localMovement = new Vector3(0, 0, 1);
        Vector3 worldMovement = transform.TransformDirection(localMovement);
        rigi.velocity = new Vector3(worldMovement.x * speed * speedBuff * speedDizziness, rigi.velocity.y, worldMovement.z * speed * speedBuff * speedDizziness);

        Flip();
    }

    public void Flip()
    {
        image.rotation = playerController.playerController_.transform.rotation;

        Vector3 x = transform.forward;
        Vector3 y = image.forward;
        y.y = x.y;

        // 计算叉积
        Vector3 crossProduct = Vector3.Cross(y, x);

        if (crossProduct.y < 0)
        {
            // y 到 x 的向量方向为负
            image.GetChild(0).localRotation = Quaternion.Euler(0f, 0, 0f);
        }
        else
        {
            // y 到 x 的向量方向为正或它们平行
            image.GetChild(0).localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }


    #region 頭頂對話

    public void setContent(string content)
    {
        if (isFinshTalk)
        {
            this.content.text = "";
            isFinshTalk = false;
            if (!talkView.gameObject.activeSelf)
            {
                Image image = talkView.GetComponent<Image>();
                var a = image.color;
                a.a = 0;
                image.color = a;
                // talkView.localPosition = Vector3.zero;

                Sequence se = DOTween.Sequence();

                talkView.gameObject.SetActive(true);
                // se.Append(talkView.DOLocalMoveY(2.3137f, 0.5f).SetEase(Ease.Flash));
                se.Append(image.DOFade(1, 0.5f).SetEase(Ease.InSine));


                se.Play();

                se.OnComplete(() =>
                {
                    StartCoroutine(talk(content));
                });
            }
            else
            {
                StartCoroutine(talk(content));
            }
        }
    }

    IEnumerator talk(string content)
    {
        for (int i = 0; i < content.Length; i++)
        {
            this.content.text += content[i];
            yield return new WaitForSeconds(0.05f);
        }

        this.content.text = content;
        yield return new WaitForSeconds(Stiff);
        isFinshTalk = true;
    }

    public void closeConetnt()
    {
        talkView.gameObject.SetActive(false);
    }
    #endregion


    #region 任務
    public void setTask(taskItem taskItem)
    {
        this.taskItem = taskItem;

    }

    public void UpdateTaskUi()
    {
        if (taskItem != null && !exclamation.gameObject.activeSelf)
        {
            exclamation.gameObject.SetActive(true);
        }
        else if (taskItem == null && exclamation.gameObject.activeSelf)
        {
            exclamation.gameObject.SetActive(false);
        }
    }
    #endregion

    #region NPC互動
    public void openInteractive()
    {
        if (taskItem != null)
        {
            taskSystem.taskSystem_.addTask(taskItem);
            taskItem = null;
        }
    }
    #endregion
}


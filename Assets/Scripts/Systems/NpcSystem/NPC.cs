using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class NPC : MonoBehaviour
{
    //是否正在攻擊
    [SerializeField] bool isAttIng;
    [Header("肥啾")]
    public Collider[] 碰撞體;
    [Header("肥啾")]
    public Transform[] 變身;
    Transform 變身Parent;

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



    floatData rest;
    floatData walk;

    public NPCdata NPCdata1 { get => NPCdata; set => NPCdata = value; }
    public Vector3 CenterPos { get => centerPos; set => centerPos = value; }
    public NPCstate NPCstate1 { get => NPCstate; set => NPCstate = value; }
    public Animator Ani1 { get => ani; set => ani = value; }
    public bool IsAttIng { get => isAttIng; set => isAttIng = value; }

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
        ani = image.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
        setCenter(CenterPos, true, 3);

        StartCoroutine(Ani());

        if (變身.Length > 0)
        {
            變身Parent = 變身[0].parent;
        }
    }

    //肥啾變身
    public void fatdoveTranslate(int i)
    {
        int j = (i == 0) ? 1 : 0;

        碰撞體[i].enabled = true;
        碰撞體[j].enabled = false;

        變身[j].SetParent(null);
        變身[j].gameObject.SetActive(false);
        變身[i].SetParent(變身Parent);
        變身[i].gameObject.SetActive(true);

        if (i == 0)
        {
            NPCstate = NPCstate.FOLLOW;
        }
        else
        {
            setCenter(NPCdata1.ShopPos[0], true, 3);
            NPCstate1 = NPCstate.WORK;
        }
    }


    //給DatSystem用的
    public void receivedSignal(int day, int start)
    {
        if (isTask == true)
        {
            return;
        }

        //如果是肥啾
        if (NPCdata1.NPCType == NPCType.FATDOVE)
        {
            if (NPCdata1.npcOtherData.FollowTransform.TryGetComponent<NPC>(out NPC nPC))
            {
                if (nPC.NPCstate1 == NPCstate.NONE || nPC.NPCstate1 == NPCstate.WALK)
                {
                    int person = Random.Range(1, 101);
                    if (person <= 60)
                    {
                        fatdoveTranslate(1);
                    }
                    else
                    {
                        fatdoveTranslate(0);
                    }
                }
                else
                {
                    int person = Random.Range(1, 101);
                    if (person <= 80)
                    {
                        fatdoveTranslate(0);
                    }
                    else
                    {
                        fatdoveTranslate(1);
                    }
                }
            }

        }
        else if (NPCdata1.dictionary.TryGetValue(start, out int index))
        {
            //DIO
            if (NPCdata1.NPCType == NPCType.DIO)
            {
                //var activePos = NPCdata1.npcOtherData.activePos[Random.Range(0, NPCdata1.npcOtherData.activePos.Length)];
                ActivePos activePos = NPCdata1.hauntingPlace[Random.Range(0, NPCdata1.hauntingPlace.Length)];
                setCenter(activePos.pos, false, activePos.range);
                range = activePos.range;

                NPCstate1 = NPCstate.WALK;
            }
            else
            {
                if (index == 0)
                {
                    if (NPCdata1.NPCLives.Length > 1)
                    {
                        //來決定墨菲當天要做甚麼
                        int person = Random.Range(1, 101);
                        int currentperson = 0;
                        int temp = -1;

                        for (int i = 0; i < NPCdata1.NPCLives.Length; i++)
                        {
                            //特定日字:洛洛
                            if (NPCdata1.NPCLives[i].specialDay.Length > 0)
                            {
                                for (int j = 0; j < NPCdata1.NPCLives[i].specialDay.Length; j++)
                                {
                                    if (NPCdata1.NPCLives[i].specialDay[j] == day)
                                    {
                                        temp = i;
                                        break;
                                    }
                                }
                            }
                            //概率切換 :墨菲
                            else if (NPCdata1.NPCLives[i].person != 0)
                            {
                                currentperson += NPCdata1.NPCLives[i].person;
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

                        if (temp != -1)
                        {
                            currentLive = temp;
                        }
                    }
                }

                int RadomPerson = Random.Range(1, 101);
                int CurrentPerson = 0;
                //正常NPC都是執行這個

                LifePerson[] lives = NPCdata1.NPCLives[currentLive].timePeriods[index].lifePerson;
                for (int i = 0; i < lives.Length; i++)
                {
                    CurrentPerson += lives[i].person;
                    if (RadomPerson <= CurrentPerson)
                    {
                        if (lives[i].life == NpcLifeType.HOME)
                        {
                            setCenter(NPCdata1.homePs, true, 3);
                            NPCstate1 = NPCstate.NONE;
                        }
                        else if (lives[i].life == NpcLifeType.WORK)
                        {
                            setCenter(NPCdata1.ShopPos[0], true, 3);
                            NPCstate1 = NPCstate.WORK;
                        }
                        else if (lives[i].life == NpcLifeType.OTHER)
                        {
                            var length = NPCdata1.hauntingPlace.Length;
                            ActivePos activePos = NPCdata1.hauntingPlace[Random.Range(0, length)];

                            setCenter(activePos.pos, false, activePos.range);
                            NPCstate1 = NPCstate.WALK;
                        }

                        //洛洛
                        else if (lives[i].life == NpcLifeType.MASS)
                        {
                            setCenter(NPCdata1.ShopPos[1], false, 3);
                            NPCstate1 = NPCstate.MASS;
                        }

                        //墨菲
                        else if (lives[i].life == NpcLifeType.LOUNGE)
                        {
                            setCenter(NPCdata1.npcOtherData.NPCspecialPlace[0].loungePlace[0], true, 3);
                            NPCstate1 = NPCstate.NONE;
                        }
                        else if (lives[i].life == NpcLifeType.FREE)
                        {
                            ActivePos activePos = NPCdata1.npcOtherData.NPCspecialPlace[0].freePlace[0];
                            setCenter(activePos.pos, true, activePos.range);
                            NPCstate1 = NPCstate.WALK;
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

    public void setCenter(Vector3 center, bool isHome, int range)
    {
        // 0.8126594/
        if (isHome)
        {
            center.y += 1;
            if (Physics.Raycast(center, Vector3.down, out RaycastHit hitInfo, 5))
            {
                this.CenterPos = hitInfo.point + new Vector3(0, 1, 0);
            }
        }
        else
        {
            center.y += 60;
            if (Physics.Raycast(center, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("block")))
            {
                this.CenterPos = hitInfo.point + new Vector3(0, 1, 0);
            }
        }

        this.range = range;
    }

    //相當於start
    public void Init()
    {
        bool isNPClive = true;
        //如果是隨從
        if (NPCdata1.NPCType == NPCType.ENTOURAGE)
        {
            isNPClive = false;
        }
        //洛洛
        else if (NPCdata1.NPCType == NPCType.PARTNER)
        {

        }
        //肥啾
        else if (NPCdata1.NPCType == NPCType.FATDOVE)
        {
            isNPClive = false;
        }
        //商人
        else if (NPCdata1.NPCType == NPCType.MERCHANT)
        {

        }
        //墨菲
        else if (NPCdata1.NPCType == NPCType.WEEKTASK)
        {

        }
        //迪奧
        else if (NPCdata1.NPCType == NPCType.DIO)
        {
            isNPClive = false;

            int j = -2;
            for (int i = 0; i < 12; i++)
            {
                j += 2;
                NPCdata1.dictionary.Add(j, i);
            }
        }

        if (isNPClive)
        {
            for (int i = 0; i < NPCdata1.NPCLives.Length; i++)
            {
                for (int j = 0; j < NPCdata1.NPCLives[i].timePeriods.Length; j++)
                {
                    int start = NPCdata1.NPCLives[i].timePeriods[j].start;
                    if (!NPCdata1.dictionary.TryGetValue(start, out var s))
                    {
                        NPCdata1.dictionary.Add(NPCdata1.NPCLives[i].timePeriods[j].start, j);
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


        if (isTask)
        {
            return;
        }

        if (NPCstate1 == NPCstate.FOLLOW)
        {
            Vector3 playerPos = NPCdata1.npcOtherData.FollowTransform.position;

            float distance = (NPCdata1.npcOtherData.FollowTransform.position - transform.position).magnitude;

            if (distance > 20)
            {
                transform.position = playerPos + new Vector3(0, 2.181707f, 0);
            }
            else if (distance > 5)
            {
                LookAt(playerPos);
                walkFront();
            }
            else
            {
                stopWalk();
            }
        }
        else if (NPCstate1 == NPCstate.WALK)
        {
            //如果沒被召喚則維持
            if (!summonCompanion())
            {
                RadomWalk();
            }
        }
        else if (NPCstate1 == NPCstate.MASS)
        {
            idle();
            if (NPCdata1.NPCType == NPCType.PARTNER)
            {
                UpSkill();
                UpCool();
            }
        }
        else if (NPCstate1 == NPCstate.WORK)
        {
            idle();
            if (NPCdata1.NPCType == NPCType.PARTNER)
            {
                UpSkill();
                UpCool();
            }

        }
        else if (NPCstate1 == NPCstate.NONE)
        {
            idle();
        }
        image.rotation = playerController.playerController_.transform.rotation;
    }

    void idle()
    {
        if ((transform.position - CenterPos).magnitude > 5)
        {
            transform.position = CenterPos;
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
        if (NPCdata1.NPCType == NPCType.PARTNER)
        {
            var attSystem = BiologySystem.biologySystem.Lolo;
            if (attSystem.attMode && attSystem.canAtt(NPCstate1))
            {
                bool isAtt = attSystem.attTarge.Count > 0;

                //如果正在攻擊就不做其他動作
                if (!IsAttIng)
                {
                    Vector3 playerPos = playerController.playerController_.transform.position;
                    float distance = (NPCdata1.npcOtherData.FollowTransform.position - transform.position).magnitude;

                    if (distance > 15)
                    {
                        transform.position = playerPos;
                        attSystem.attTarge.Clear();
                    }
                    else if (isAtt)
                    {
                        if (attSystem.attTarge[0] != null && attSystem.attTarge[0].gameObject.activeSelf)
                        {
                            if ((attSystem.attTarge[0].position - transform.position).magnitude > 10)
                            {
                                LookAt(attSystem.attTarge[0].position);
                                walkFront();
                            }
                            else
                            {
                                Attack(attSystem.attTarge[0], attSystem.skillTime);
                            }
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

            if ((lolo.skillTime[i].floatData.current -= Time.deltaTime) <= 0)
            {
                lolo.skillTime[i].floatData.current = 0;
            }

            if (lolo.skillTime[i].floatData.current == 0)
            {
                print(i);
            }
        }
    }

    public void Attack(Transform monster, LoloSkill[] loloSkills)
    {
        if (物理攻擊(monster, loloSkills[0].floatData))
        {

            ani.SetBool("SKILL1", true);
        }
        // else if (攻_驅邪(monster, loloSkills[1].floatData))
        // {
        //     IsAttIng = true;
        //     StartCoroutine(AttCold(loloSkills[1].continuedTime, 2));
        //     ani.SetBool("SKILL2", true);
        // }
        // else if (攻_迪奧的祝福(monster, loloSkills[2].floatData))
        // {
        //     IsAttIng = true;
        //     StartCoroutine(AttCold(loloSkills[2].continuedTime, 3));
        //     ani.SetBool("SKILL3", true);
        // }
        // else if (補_神聖洗禮(playerController.playerController_.transform, loloSkills[3].floatData))
        // {
        //     IsAttIng = true;
        //     StartCoroutine(AttCold(loloSkills[3].continuedTime, 4));
        //     ani.SetBool("SKILL4", true);
        // }
    }

    IEnumerator AttCold(float second, int SkillId)
    {
        yield return new WaitForSeconds(second);
        IsAttIng = false;
        ani.SetBool("SKILL" + SkillId, false);
    }

    public bool 物理攻擊(Transform monster, floatData floatData)
    {
        if (floatData.current <= 0)
        {
            IsAttIng = true;


            BullectSystem.bullectSystem.fire(NPCdata.npcOtherData.bullect[0], transform, monster.position, null, "monster", Vector3.zero);
            floatData.current = floatData.max;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool 攻_驅邪(Transform monster, floatData floatData)
    {
        if (floatData.current <= 0)
        {
            IsAttIng = true;

            if (monster.TryGetComponent<Biology>(out Biology biology))
            {
                biology.injuried(800, transform);
            }
            floatData.current = floatData.max;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool 攻_迪奧的祝福(Transform monster, floatData floatData)
    {
        if (floatData.current <= 0)
        {
            IsAttIng = true;

            BullectSystem.bullectSystem.fire(NPCdata.npcOtherData.bullect[1], transform, monster.position, monster, "monster", Vector3.zero);
            floatData.current = floatData.max;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool 補_神聖洗禮(Transform monster, floatData floatData)
    {
        if (floatData.current <= 0)
        {
            IsAttIng = true;

            BullectSystem.bullectSystem.fire(NPCdata.npcOtherData.bullect[2], transform, monster.position, monster, "monster", Vector3.zero);
            floatData.current = floatData.max;
            return true;
        }
        else
        {
            return false;
        }
    }


    #endregion

    IEnumerator Ani()
    {
        yield return new WaitForSeconds(0.3f);

        if ((transform.position - prePos).magnitude > 0.1f)
        {
            ani.SetBool("RUN", true);
        }
        else
        {
            ani.SetBool("RUN", false);
        }



        prePos = transform.position;


        StartCoroutine(Ani());
    }


    public void RadomWalk()
    {
        if ((walk.current += Time.deltaTime) < walk.max)
        {
            float distance = (transform.position - CenterPos).magnitude;
            if (distance > range + 10)
            {
                transform.position = CenterPos;
            }
            else if (((transform.position + transform.forward) - CenterPos).magnitude >= range)
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

    public void stopWalk()
    {
        if (rigi.useGravity == false)
        {
            Vector3 v = rigi.velocity;
            v.x = 0;
            v.z = 0;

            rigi.velocity = v;
        }
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

        ani.SetBool("TALK", true);


        for (int i = 0; i < content.Length; i++)
        {
            this.content.text += content[i];
            yield return new WaitForSeconds(0.05f);
        }

        this.content.text = content;
        yield return new WaitForSeconds(Stiff);
        isFinshTalk = true;

        ani.SetBool("TALK", false);
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
            return;
        }

        if (isTask == false && NPCstate1 == NPCstate.WORK && (NPCdata1.NPCType == NPCType.MERCHANT || NPCdata1.NPCType == NPCType.FATDOVE))
        {
            ShopSystem.shopSystem.OpenShopPanel(
                NPCdata1.npcOtherData.merchantData[0].buys,
                NPCdata1.npcOtherData.merchantData[0].sells
            );
        }
    }
    #endregion
}


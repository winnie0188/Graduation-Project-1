using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Biology : MonoBehaviour
{
    [SerializeField] Monster monster;
    [Space(50)]

    [SerializeField] Transform attckObject;

    float maxspeed;
    [SerializeField] float speed;
    [SerializeField] Vector3 center;

    [SerializeField] Transform eggGeneratePos;

    [Header("特殊季節改變攻擊模式")]
    public MonthBuff[] monthBuffs;

    //休息時間
    floatData rest;
    //走路時間
    floatData walk;

    Rigidbody rigi;
    Collider collider;
    Transform image;

    Transform egg;

    bool initCounterAttack;

    //初始化結束
    bool isInitFinsh;
    //技能結束並進入冷卻
    bool isSkillFinsh = true;
    //攻擊結束
    bool attFinsh;

    //未還原的重量
    Transform unrestoredWight;
    //是否有重力
    bool useGravity;

    // =========================

    //敵人跟自己的距離
    float eselfDistance;
    //敵人跟中心的距離
    float ecenterDistance;
    //自己跟鐘的距離
    float scenterDistance;
    //=============================

    private void Awake()
    {
        initCounterAttack = monster.counterAttack;

        rest = new floatData()
        {
            current = 0,
            max = 10
        };

        walk = new floatData()
        {
            current = 0,
            max = 1
        };

        maxspeed = speed;

        image = transform.GetChild(0);
        rigi = GetComponent<Rigidbody>();
        useGravity = rigi.useGravity;
        collider = GetComponent<Collider>();
    }

    public void setEgg(Transform egg)
    {
        this.egg = egg;
        egg.transform.position = eggGeneratePos.transform.position;
    }

    public Transform getEgg()
    {
        return egg;
    }


    public Monster GetMonster()
    {
        return monster;
    }

    public void init(Vector3 center)
    {

        speed = Random.Range(maxspeed - 1.5f, maxspeed + 1.5f);

        transform.gameObject.SetActive(true);
        this.center = center;

        if (monster.eggParent.Length > 0)
        {
            BiologySystem.biologySystem.addEgg(monster.eggParent, this);
        }

        isInitFinsh = true;
    }

    public void running()
    {
        //==========記得還原==========
        if (!isInitFinsh)
        {
            return;
        }

        image.rotation = playerController.playerController_.transform.rotation;

        eselfDistance = (playerController.playerController_.transform.position - transform.position).magnitude;
        ecenterDistance = (playerController.playerController_.transform.position - center).magnitude;
        scenterDistance = (transform.position - center).magnitude;

        if (eselfDistance > 100)
        {

        }
        else if (monster.Hp <= 0)
        {
            die();
        }
        else if (!isSkillFinsh)
        {

        }
        else
        {
            changeCountAtt();

            if (monster.counterAttack == true || eggAttack())
            {
                if (!hasAttckObject())
                {
                    if (ecenterDistance < monster.range)
                    {
                        attckObject = playerController.playerController_.transform;
                    }
                    else
                    {
                        RadomWalk();
                    }
                }
            }
            else
            {
                if (!hasAttckObject())
                {
                    RadomWalk();
                }
            }
        }

        image.rotation = playerController.playerController_.transform.rotation;
    }
    //這是給梅花鹿的
    void changeCountAtt()
    {
        if (monthBuffs.Length > 0)
        {
            for (int i = 0; i < monthBuffs.Length; i++)
            {
                if (monthBuffs[i].month.Length > 0)
                {
                    for (int j = 0; j < monthBuffs[i].month.Length; j++)
                    {
                        if (monthBuffs[i].month[j] == LightingManager.lightingManager.getMonth())
                        {
                            monster.counterAttack = !initCounterAttack;
                            return;
                        }
                    }
                }
            }
        }
    }

    void RadomWalk()
    {
        if ((walk.current += Time.deltaTime) < walk.max)
        {
            //走路
            if (scenterDistance > monster.range)
            {
                transform.position = center;
            }
            else if (((transform.position + transform.forward) - center).magnitude >= monster.range)
            {
                transform.Rotate(new Vector3(0, Random.Range(10, 45), 0), Space.Self);
            }
            else
            {
                if (Physics.Raycast(transform.position, transform.forward, 10))
                {
                    transform.Rotate(new Vector3(0, Random.Range(10, 45), 0), Space.Self);
                }
                else
                {
                    walkFront(speed);
                }
            }

            print("走路");
        }
        else if ((rest.current += Time.deltaTime) < rest.max)
        {
            //閒置
            transform.Rotate(new Vector3(0, Random.Range(-45, 45), 0), Space.Self);
            print("閒置");
        }
        else
        {
            //兩個跑完則重新再來一次
            walk.current = 0;
            rest.current = 0;
        }
    }

    bool hasAttckObject()
    {
        if (attckObject != null)
        {
            if (ecenterDistance > monster.range)
            {
                //不再追擊
                attckObject = null;
                print("清空");
            }
            else if (eselfDistance > monster.attactRange)
            {
                //朝著對象前進
                LookAt(attckObject.position);
                walkFront(speed);

                print("跑向敵人");
            }
            else
            {
                print("攻擊");
                attack();
            }

            return true;
        }
        return false;
    }

    void attack()
    {
        for (int i = 0; i < monster.monsterSkills.Length; i++)
        {
            if (isSkillFinsh && monster.monsterSkills[i].coolZero < monster.monsterSkills[i].monsterSkills.CoolingTime)
            {
                StartCoroutine(Skill(monster.monsterSkills[i], monster.monsterSkills[i].monsterSkills, attckObject));
                return;
            }
        }

        print("冷卻中");
    }


    IEnumerator Skill(SkillData skillData, MonsterSkill monsterSkill, Transform enemy)
    {
        isSkillFinsh = false;
        skillData.coolZero = monsterSkill.CoolingTime;

        for (int i = 0; i < monsterSkill.attackTime; i++)
        {
            attFinsh = false;
            StartCoroutine(SkillUpdate(monsterSkill, enemy));
            for (int j = 0; j < monsterSkill.continued; j++)
            {
                if (attFinsh == true)
                {
                    break;
                }
                yield return new WaitForSeconds(1);
            }
            attFinsh = true;
            yield return new WaitForSeconds(monsterSkill.STILL);
        }

        isSkillFinsh = true;
        //進入冷卻
        yield return new WaitForSeconds(monsterSkill.CoolingTime);
        skillData.coolZero = 0;
    }

    IEnumerator SkillUpdate(MonsterSkill monsterSkill, Transform enemy)
    {
        int y = y = (int)enemy.position.y;
        bool FromToTotch = false;
        int temp = 0;

        // print(Path.GetFileNameWithoutExtension(monsterSkill.name));

        //這邊只執行一次
        if (monsterSkill.skillType == SkillType.NORMAL)
        {
            LookAt(enemy.transform.position);
            Hit(monsterSkill.power, monster.attactRange, true, enemy);
        }
        else if (monsterSkill.skillType == SkillType.COLLISION)
        {
            LookAt(enemy.transform.position);
        }
        else if (monsterSkill.skillType == SkillType.SHOOT)
        {
            LookAt(enemy.transform.position);
            ShootSystem.shootSystem.shoot(
                monsterSkill.SKILL_SHOOT[0].shoot,
                transform,
                enemy.position,
                monsterSkill.power,
                monsterSkill.continued,
                enemy
            );
        }
        else if (monsterSkill.skillType == SkillType.ROLL)
        {
            LookAt(enemy.transform.position);
        }
        else if (monsterSkill.skillType == SkillType.BUFF)
        {
            LookAt(enemy.transform.position);

            if (Hit(monsterSkill.power, monster.attactRange, true, enemy))
            {
                foreach (var item in monsterSkill.SKILL_BUFF[0].buffs)
                {
                    BuffSystem.buffSystem.buff(item.buffKey, enemy.transform, item.value, item.duration);
                }
            }
        }
        else if (monsterSkill.skillType == SkillType.FIRE)
        {
            if (monsterSkill.SKILL_FIRE[0].isDive == true)
            {
                rigi.useGravity = false;
                collider.enabled = false;
            }
            else if (monsterSkill.SKILL_FIRE[0].bounce == true)
            {
                //往上跳
                rigi.AddForce(new Vector3(0, 10f, 0), ForceMode.Impulse);
            }
            else
            {
                fire(monsterSkill.SKILL_FIRE[0].bullect, enemy.transform.position, enemy);
            }
        }
        else if (monsterSkill.skillType == SkillType.RUNAWAY)
        {
            rigi.useGravity = false;
        }
        else if (monsterSkill.skillType == SkillType.HOVER)
        {
            rigi.useGravity = false;
        }
        else if (monsterSkill.skillType == SkillType.FROMTO)
        {
            rigi.useGravity = false;
        }
        else if (monsterSkill.skillType == SkillType.DEFENSE)
        {
            rigi.useGravity = false;
            collider.enabled = false;
        }



        float deltaTime;
        //這邊是迴圈
        while (!attFinsh)
        {
            deltaTime = Time.deltaTime;

            if (monsterSkill.skillType == SkillType.COLLISION)
            {
                walkFront(speed * monsterSkill.SKILL_COLLISION[0].speedUp);
                if (!FromToTotch && Hit(monsterSkill.power, 3, false, enemy))
                {
                    FromToTotch = true;
                }
            }
            else if (monsterSkill.skillType == SkillType.ROLL)
            {
                walkFront(speed * 1.05f);

                if (!FromToTotch && Hit(monsterSkill.power, 3, false, enemy))
                {
                    FromToTotch = true;
                }
            }
            else if (monsterSkill.skillType == SkillType.FIRE)
            {
                if (monsterSkill.SKILL_FIRE[0].isDive == true)
                {
                    if (temp != -1000 && transform.position.y > (y - 3))
                    {
                        transform.Translate(Vector3.down * speed * deltaTime);
                    }
                    else
                    {
                        if (temp != -1000)
                        {
                            temp = -1000;
                            yield return new WaitForSeconds(1);
                            transform.position = enemy.transform.position - new Vector3(0, 5, 0);
                        }
                        else
                        {
                            transform.Translate(Vector3.up * speed * 1.1f * deltaTime);
                        }
                    }
                }
                else if (monsterSkill.SKILL_FIRE[0].bounce)
                {
                    if (temp != -1000 && transform.position.y < (y + 1))
                    {

                    }
                    else
                    {
                        temp = -1000;

                        if (Physics.Raycast(transform.position, Vector3.down, 1, LayerMask.GetMask("block")))
                        {
                            fire(monsterSkill.SKILL_FIRE[0].bullect, enemy.transform.position, enemy);
                            attFinsh = true;
                        }
                    }

                    if ((transform.position - enemy.position).magnitude > 5)
                    {
                        LookAt(enemy.transform.position);
                        walkFront(speed * 1.1f);
                    }
                }
                else if (monsterSkill.SKILL_FIRE[0].isFireEndRunAwake)
                {
                    LookAt(transform.position + enemy.transform.forward);
                    walkFront(speed * 1.1f);
                }
            }
            else if (monsterSkill.skillType == SkillType.RUNAWAY)
            {
                if (transform.position.y < y + 1.5f)
                {
                    transform.Translate(Vector3.up * speed * 0.5f * deltaTime);
                }
                LookAt(transform.position + enemy.transform.forward);
                walkFront(speed * 1.1f);
            }
            else if (monsterSkill.skillType == SkillType.HOVER)
            {
                Vector3 my = transform.position;
                Vector3 end = enemy.position;

                my.y = end.y;

                if (monsterSkill.SKILL_HOVER[0].whereabouts == true)
                {
                    if (temp != -1000 && transform.position.y < y + 3)
                    {
                        LookAt(enemy.position);
                        walkFront(speed);
                        transform.Translate(Vector3.up * speed * 0.5f * deltaTime);
                    }
                    else if (!FromToTotch && (my - end).magnitude > 1)
                    {
                        temp = -1000;
                        LookAt(enemy.position);
                        walkFront(speed * 3f);
                    }
                    else
                    {
                        temp = -1000;
                        FromToTotch = true;
                        LookAt(enemy.position);
                        walkFront(speed * 3f);
                        transform.Translate(Vector3.down * speed * 3f * deltaTime);
                        Hit(monsterSkill.power, 3, true, enemy);
                    }
                }
                else
                {
                    if (Hit(monsterSkill.power, 3, false, enemy))
                    {
                        float intervalZero = 0;
                        while ((intervalZero += Time.deltaTime) <= monsterSkill.SKILL_HOVER[0].interval)
                        {
                            yield return null;
                            fly(Time.deltaTime);
                            my = transform.position;
                            end = enemy.position;

                            my.y = end.y;
                        }
                    }
                    else
                    {
                        fly(deltaTime);
                    }

                    void fly(float deltaTime)
                    {
                        if ((my - end).magnitude > 2)
                        {
                            LookAt(enemy.position);
                            walkFront(speed * 1.1f);
                        }

                        if (transform.position.y < y + 1.5f)
                        {
                            transform.Translate(Vector3.up * speed * 0.5f * deltaTime);
                        }
                        else if (transform.position.y > y + 2f)
                        {
                            transform.Translate(Vector3.down * speed * 0.5f * deltaTime);
                        }
                    }
                }
            }
            else if (monsterSkill.skillType == SkillType.FROMTO)
            {
                if (temp != -1000 && transform.position.y < y + 1.5f)
                {
                    transform.Translate(Vector3.up * speed * 0.9f * deltaTime);
                }
                else
                {
                    if (!FromToTotch)
                    {
                        temp = -1000;
                        LookAt(enemy.position);
                        walkFront(speed * 1.1f);

                        if (Hit(monsterSkill.power, 1, false, enemy))
                        {
                            FromToTotch = true;
                            enemy.GetComponent<Rigidbody>().useGravity = false;
                            unrestoredWight = enemy;
                        }
                    }
                    else
                    {
                        if (transform.position.y < y + monsterSkill.SKILL_FROMTO[0].height)
                        {
                            transform.Translate(Vector3.up * speed * 0.9f * deltaTime);
                            enemy.transform.position = transform.position + Vector3.down;
                        }
                        else
                        {
                            attFinsh = true;
                        }
                    }
                }
            }
            yield return null;
        }

        if (monsterSkill.skillType == SkillType.COLLISION)
        {
            rigi.velocity = Vector3.zero;
        }
        else if (monsterSkill.skillType == SkillType.FIRE)
        {
            if (monsterSkill.SKILL_FIRE[0].isDive == true)
            {
                fire(monsterSkill.SKILL_FIRE[0].bullect, enemy.transform.position, enemy);
                collider.enabled = true;
                rigi.useGravity = useGravity;
            }
        }
        else if (monsterSkill.skillType == SkillType.RUNAWAY)
        {
            rigi.useGravity = useGravity;
        }
        else if (monsterSkill.skillType == SkillType.HOVER)
        {
            rigi.useGravity = useGravity;
        }
        else if (monsterSkill.skillType == SkillType.FROMTO)
        {
            rigi.useGravity = useGravity;
            enemy.GetComponent<Rigidbody>().useGravity = true;
            unrestoredWight = null;
        }
        else if (monsterSkill.skillType == SkillType.DEFENSE)
        {
            rigi.useGravity = useGravity;
            collider.enabled = true;
        }
        else if (monsterSkill.skillType == SkillType.SUMMON)
        {
            Vector3[] positions = monsterSkill.SKILL_SUMMON[0].position;

            string monsterK = monsterSkill.SKILL_SUMMON[0].monsterKey;
            string bullectK = monsterSkill.SKILL_SUMMON[0].bullectKey;

            if (monsterK.Length > 0)
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    BiologySystem.biologySystem.addBiology(
                        monsterK,
                        transform.position + positions[i],
                        false
                    );
                }
            }
            else if (bullectK.Length > 0)
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    BullectSystem.bullectSystem.fire(
                        bullectK,
                        transform,
                        transform.position + positions[i],
                        enemy
                    );
                }
            }

        }
    }


    void fire(string key, Vector3 look, Transform enemy)
    {
        LookAt(look);
        BullectSystem.bullectSystem.fire(
            key,
            transform,
            look,
            enemy
        );
    }


    bool Hit(float power, float range, bool isEnd, Transform enemy)
    {
        if ((enemy.transform.position - transform.position).magnitude <= range)
        {
            if (enemy.transform.TryGetComponent<playerController>(out var player))
            {
                player.HpUpdate(power * -1);
            }
            else if (enemy.transform.TryGetComponent<NPC>(out var Lolo))
            {
                BiologySystem.biologySystem.Lolo.UpdateLoloHp(power * -1);
            }

            if (isEnd)
            {
                attFinsh = true;
            }
            return true;
        }
        else
        {
            return false;
        }
    }



    void die()
    {
        recycle();
    }

    void recycle()
    {
        StopAllCoroutines();

        if (unrestoredWight != null)
        {
            unrestoredWight.GetComponent<Rigidbody>().useGravity = true;
        }

        isInitFinsh = false;
        isSkillFinsh = true;
        rigi.useGravity = useGravity;
        collider.enabled = true;

        foreach (var item in monster.monsterSkills)
        {
            item.coolZero = 0;
        }

        transform.gameObject.SetActive(false);

        monster.Hp = monster.maxHp;

        //最後執行
        BiologySystem.biologySystem.removeBiology(
            BiologySystem.biologySystem.getKey(transform),
            transform
        );
    }


    public bool eggAttack()
    {
        if (monster.eggParent.Length > 0)
        {
            if (egg != null)
            {
                return true;
            }
        }
        return false;
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

    public void walkFront(float speed)
    {
        Vector3 v = rigi.velocity;
        v.x = 0;
        v.z = 0;

        rigi.velocity = v;

        Vector3 localMovement = new Vector3(0, 0, 1);
        Vector3 worldMovement = transform.TransformDirection(localMovement);
        rigi.velocity = new Vector3(worldMovement.x * speed, rigi.velocity.y, worldMovement.z * speed);

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
}





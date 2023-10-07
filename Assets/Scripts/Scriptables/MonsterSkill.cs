using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "monsterSkill", menuName = "skill/monsterSkill", order = 1)]
public class MonsterSkill : ScriptableObject
{
    [Header("持續時間")]
    public float continued;
    public float CoolingTime;
    public int power;

    [Header("攻擊次數")]
    public int attackTime;
    [Header("僵直時間")]
    public float STILL;

    public SkillType skillType;

    public SKILL_NONE[] SKILL_NONE;
    public SKILL_NORMAL[] SKILL_NORMAL;
    public SKILL_SHOOT[] SKILL_SHOOT;
    public SKILL_COLLISION[] SKILL_COLLISION;
    public SKILL_ROLL[] SKILL_ROLL;
    public SKILL_BUFF[] SKILL_BUFF;
    public SKILL_FIRE[] SKILL_FIRE;
    public SKILL_RUNAWAY[] SKILL_RUNAWAY;
    public SKILL_HOVER[] SKILL_HOVER;
    public SKILL_FROMTO[] SKILL_FROMTO;
    public SKILL_DEFENSE[] SKILL_DEFENSE;
    public SKILL_SUMMON[] SKILL_SUMMON;
}

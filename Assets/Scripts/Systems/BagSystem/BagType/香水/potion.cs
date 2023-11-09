using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class potion : BagItemObj
{
    public potionType potionType;
    public AttackPotion attackPotion;

    public void Attack()
    {
        Transform player = playerController.playerController_.transform;
        playerController.playerController_.setAniFunction(
            attackPotion.aniName,
            attackPotion.generationTime,
            attackPotion.second,
            attackPotion.bullectKey,
            player,
            player.position + player.forward * 20,
            null,
            attackPotion.generationIndex,
            attackPotion.time
        );
    }
}

public enum potionType
{
    ATTACK
}

[System.Serializable]
public class AttackPotion
{
    [Header("======攻擊型子彈=======")]
    public string bullectKey;
    [Header("攻擊動畫")]
    public string aniName;
    [Header("動畫持續時間")]
    public float second;

    [Header("生成時間")]
    public float generationTime;
    [Header("生成位置索引")]
    public int generationIndex;
    [Header("生成量")]
    public int time;
}
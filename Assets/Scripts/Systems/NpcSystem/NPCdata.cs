using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCdata
{
    public Dictionary<int, int> dictionary = new Dictionary<int, int>();
    public NPCType NPCType;
    public NPCLive[] NPCLives;

    [Header("Home")]
    public Vector3 homePs;
    [Header("店面位置")]
    public Vector3[] ShopPos;
    [Header("出沒地點")]
    public ActivePos[] hauntingPlace;

    public NPCotherData npcOtherData;

}

[System.Serializable]
public class NPCotherData
{
    [Header("精靈洛洛肥啾")]
    public Transform FolowTransform;
    [Header("DIO專用")]
    public ActivePos[] activePos;
    [Header("洛洛專用")]
    public string[] bullect;
    [Header("墨菲才要")]
    public NPCspecialPlace[] NPCspecialPlace;
    [Header("商人才要")]
    public merchantData[] merchantData;
}

[System.Serializable]
public class merchantData
{
    public List<merchantItem> buys;
    public List<merchantItem> sells;
}



[System.Serializable]
public class NPCLive
{
    [Header("對應日期:長度0代表每天")]
    public int[] specialDay;
    [Header("百分之幾做NPCLive")]
    public int person;



    [Header("時段")]
    public TimePeriod[] timePeriods;
}

public enum NPCType
{
    //隨從
    ENTOURAGE,
    //夥伴
    PARTNER,
    //商人
    MERCHANT,
    //周常
    WEEKTASK,
    //肥啾
    FATDOVE,
    //迪奧
    DIO
}

[System.Serializable]
public class NPCspecialPlace
{
    [Header("休息室地點")]
    public Vector3[] loungePlace;
    [Header("FREE地點")]
    public ActivePos[] freePlace;
}

[System.Serializable]
public class ActivePos
{
    public Vector3 pos;
    [Header("活動範圍")]
    [Range(10, 50)]
    public int range = 10; // 将默认值设置为 10
}


[System.Serializable]
public class TimePeriod
{
    [Header("開始時間")]
    public int start;

    [Header("結束時間")]
    public int end;

    [Header("百分之幾做TimePeriod")]
    public LifePerson[] lifePerson;
}

[System.Serializable]
public class LifePerson
{
    public int person;
    public NpcLifeType life;
}

public enum NpcLifeType
{
    //===================
    //在家休息
    HOME,
    //休息室
    LOUNGE,
    //==================
    //工作
    WORK,
    //==================
    //其他地點
    OTHER,
    //空閒
    FREE,
    //==================
    //不變
    UNCHANGED,
    //彌撒
    MASS
}
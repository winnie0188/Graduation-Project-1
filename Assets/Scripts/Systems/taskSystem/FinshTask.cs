using UnityEngine;
[System.Serializable]
public class FinshTask
{
    public talkContent talkContent;
    public taskItem taskItem;
    public BagItem[] bagItem;
    public BagItem[] removeBagItem;

    [Header("ai順移到某地")]
    public Teleport[] teleports;
    [Header("旋轉到特定方向，跟teleports搭配使用")]
    public float playerRotateY;
    public bool openFadePanel;
    [Header("搭配finshUI")]
    public string FinshContent;
    public bool openFinshUi;
}

//順移
[System.Serializable]
public class Teleport
{
    public string AiName;
    public Vector3 position;
}
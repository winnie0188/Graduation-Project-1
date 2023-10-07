[System.Serializable]
public class other : BagItemObj
{
    public otherType otherType = otherType.NONE;
    //-----------信---------------
    public otherLetter otherLetter;
    //-----------信---------------
}

[System.Serializable]
public class otherLetter
{
    // 信的路徑
    public string letterPath;
    // 觸發遠處npc的任務
    public string npc;
    public talkContent talkContent;
}
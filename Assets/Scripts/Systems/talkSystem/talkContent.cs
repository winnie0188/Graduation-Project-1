using UnityEngine;
using System.Collections.Generic;

// ღ
[System.Serializable]
[CreateAssetMenu(fileName = "talkContent", menuName = "talk/talkContent", order = 2)]
public class talkContent : ScriptableObject
{
    // Start is called before the first frame update

    [Header("劇本")]
    [SerializeField] TextAsset TextFile;
    // 內容拆解
    public List<TextDataFile> TextDataList = new List<TextDataFile>();

    public List<PeopleHeight> peopleHeights = new List<PeopleHeight>();

    [ContextMenu("push數據")]
    private void pushPeople()
    {
        talkSystem talkSystem_ = FindObjectOfType<talkSystem>();
        talkSystem_.peopleIcon.setPeopleHeights(peopleHeights);
    }

    [ContextMenu("抓取角色")]
    private void getPeople()
    {
        peopleHeights.Clear();
        List<string> peopleNames = new List<string>();
        for (int i = 0; i < TextDataList.Count; i++)
        {
            string peopleHeight = TextDataList[i].PeopleName;

            if (!peopleNames.Contains(peopleHeight))
            {
                peopleNames.Add(peopleHeight);
            }
        }

        for (int i = 0; i < peopleNames.Count; i++)
        {
            peopleHeights.Add(
                new PeopleHeight
                {
                    name = peopleNames[i]
                }
            );
        }
    }

    [ContextMenu("點我設定")]
    private void set()
    {
        talkSystem talkSystem_ = FindObjectOfType<talkSystem>();
        talkSystem_.peopleIcon.set();

        if (TextFile == null || talkSystem_ == null)
            return;


        TextDataList.Clear();

        var LineDate = TextFile.text.Split('\n');
        var index = 0;


        foreach (var line in LineDate)
        {

            if (TextDataList.Count <= index)
            {
                TextDataList.Add(new TextDataFile());
            }


            if (line.Trim().Length == 0)
            {

            }
            else if (line[0] == '"')
            {
                string image_text = line.Trim();

                int slashIndex = image_text.IndexOf('/');

                if (slashIndex != -1)
                {
                    // Substring(index,length)
                    string image_0 = image_text.Substring(1, slashIndex - 1);

                    string image_1 = image_text.Substring(slashIndex + 1, image_text.Length - slashIndex - 2);

                    TextDataList[index].PeopleIcon[0] = talkSystem_.setPeopleIcon(image_0);
                    TextDataList[index].PeopleIcon[1] = talkSystem_.setPeopleIcon(image_1);
                }
            }
            else if (line[0] == '&')
            {
                if (line[1] == 'A')
                {
                    TextDataList[index].PeopleName = "";
                }
                else
                {
                    TextDataList[index].PeopleName = line.Trim().Substring(1);
                }
            }

            else if (line[0] == '/')
            {
                TextDataList[index].sendMess.Add(line.Trim().Substring(1));
            }
            else if (line[0] == '|')
            {
                var branchs = line.Trim().Split('\\');
                BranchTalk branchTalk = new BranchTalk
                {
                    Triggevent = int.Parse(branchs[0].Substring(1)),
                    eventSelect = talkSystem_.setTalkContent(branchs[1]),
                    content = branchs[2]
                };
                TextDataList[index].branch.Add(branchTalk);
            }
            else
            {
                var content = line.Trim().Split('ღ');
                for (int i = 0; i < content.Length; i++)
                {
                    TextDataList[index].Text += content[i];
                    if (i != content.Length - 1)
                    {
                        TextDataList[index].Text += "\n";
                    }
                }
                index++;
            }
        }

        if (TextDataList[TextDataList.Count - 1].PeopleIcon[0] == null)
        {
            TextDataList.Remove(TextDataList[TextDataList.Count - 1]);
        }
    }

}

[System.Serializable]
public class TextDataFile
{

    // 兩位大頭照
    public Sprite[] PeopleIcon = new Sprite[2];

    // 當前說話的人
    public string PeopleName;

    // 內容拆解
    public string Text;
    // 觸發事件
    public List<string> sendMess = new List<string>();
    public List<BranchTalk> branch = new List<BranchTalk>();
}

[System.Serializable]
public class BranchTalk
{
    public int Triggevent;
    public talkContent eventSelect;
    public string content;
}


[System.Serializable]
public class PeopleHeight
{
    public string name;
    public float person;
}

using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
[CreateAssetMenu(fileName = "talkContent", menuName = "talk/talkContent", order = 2)]
public class talkContent : ScriptableObject
{
    // Start is called before the first frame update

    [Header("劇本")]
    [SerializeField] TextAsset TextFile;
    // 內容拆解
    public List<TextDataFile> TextDataList = new List<TextDataFile>();
    [SerializeField] bool reset;



    private void OnValidate()
    {
        talkSystem talkSystem_ = FindObjectOfType<talkSystem>();


        if (TextFile == null || !reset)
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


            if (line.Contains("【"))
            {
                TextDataList[index].Text = line.Trim();
                index++;
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
}

using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
[CreateAssetMenu(fileName = "peopleIcon", menuName = "talk/peopleIcon", order = 1)]
public class peopleIcon : ScriptableObject
{
    // Start is called before the first frame update
    public Dictionary<string, int> Icon_ = new Dictionary<string, int>();

    public Sprite[] Icon;

    public Dictionary<string, int> TextFile_ = new Dictionary<string, int>();

    public talkContent[] TextFile;
    public List<PeopleHeight> PeopleHeights = new List<PeopleHeight>();

    public void setPeopleHeights(List<PeopleHeight> datas)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            if (!PeopleHeights.Exists(x => x.name == datas[i].name))
            {
                PeopleHeights.Add(datas[i]);
            }
        }
    }

    public void set()
    {
        // 清空字典
        Icon_.Clear();
        TextFile_.Clear();

        // 將圖片存入字典
        for (int i = 0; i < Icon.Length; i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(Icon[i].name);
            if (!Icon_.ContainsKey(fileName))
            {
                Icon_.Add(fileName, i);
            }
        }

        // 將劇本存入字典
        for (int i = 0; i < TextFile.Length; i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(TextFile[i].name);
            if (!TextFile_.ContainsKey(fileName))
            {
                TextFile_.Add(fileName, i);
            }
        }
    }
}

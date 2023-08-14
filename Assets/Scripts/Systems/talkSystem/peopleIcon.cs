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


    [ContextMenu("點我設定")]
    private void set()
    {
        // 清空字典
        Icon_.Clear();

        // 將圖片存入字典
        for (int i = 0; i < Icon.Length; i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(Icon[i].name);
            if (!Icon_.ContainsKey(fileName))
            {
                Icon_.Add(fileName, i);
            }
        }


    }
}

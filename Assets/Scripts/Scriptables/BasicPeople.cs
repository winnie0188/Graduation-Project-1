using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "BasicPeople", menuName = "Scriptables/BasicPeople", order = 5)]
public class BasicPeople : ScriptableObject
{
    // Start is called before the first frame update
    [Header("名子")]
    public string peoplename;
    [Header("生日")]
    public string birthday;
    [Header("香味")]
    public string fragrance;
    [Header("性格")]
    public string personality;
    [Header("喜歡的物品")]
    public string favorite;
    [Header("討厭的物品")]
    public string hate;
}

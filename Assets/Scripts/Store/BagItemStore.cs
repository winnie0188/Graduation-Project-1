using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "BagItemStore", menuName = "Store/BagItemStore", order = 1)]
public class BagItemStore : ScriptableObject
{
    public BagItem[] BagItems;

    [ContextMenu("點我設定")]
    public void reset_()
    {
        for (int i = 0; i < BagItems.Length; i++)
        {
            BagItems[i].reset_(i);
        }
    }
}
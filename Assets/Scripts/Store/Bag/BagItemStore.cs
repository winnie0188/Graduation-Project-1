using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "BagItemStore", menuName = "Store/BagItemStore", order = 1)]
public class BagItemStore : ScriptableObject
{
    public BagItem[] BagItems;

    public int[] bagStoreIndex;


    public void reset_()
    {
        for (int i = 0; i < BagItems.Length; i++)
        {
            BagItems[i].reset_(i);
        }
    }


    public void setbagStoreIndex_()
    {
        bagStoreIndex = new int[9];

        for (int i = 0; i < BagItems.Length; i++)
        {
            bagStoreIndex[BagItems[i].bagSoreIndex]++;
        }
    }


    public GameObject setPrefab(int i)
    {
        if (BagItems.Length > i)
        {
            return BagItems[i].GetBlock().BlockPool._template;
        }
        else
        {
            return null;
        }

    }
}
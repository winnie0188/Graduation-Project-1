using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "BagItem", menuName = "Scriptables/BagItem", order = 2)]
public class BagItem : ScriptableObject
{
    public enum BagItemType
    {
        //物品欄右側裝備欄
        potion,//藥水0
        other,//其他1

        //左側裝備欄2
        clothe,//衣服3

        //無法手持
        material,//材料4

        //可以手持
        food,//食物5
        tool,//工具6
        block,//建築7
        Ingredients,//食材8
        teachBook//教學書9
    }


    [Header("基礎屬性")]
    public string BagItem_name;//物品名
    public Sprite BagItem_icon;//物品圖
    public string BagItem_info;//物品介紹

    public int sellPrice;//出售價格

    // 哪個背包
    [Header("哪個背包")]
    public int bagSoreIndex = 0;

    //
    [Header("裝備到哪")]
    public int keySoreIndex;

    [Header("道具ID")]
    public int id;

#pragma warning disable CS0414

    [Header("物品類型")]
    public BagItemType BagItemType_;//物品類型

    public potion potion;
    [SerializeField] other other;
    public clothe clothe;
    [SerializeField] material material;
    [SerializeField] food food;
    public tool tool;
    [SerializeField] block block;

    [SerializeField] Ingredients Ingredients;
    [SerializeField] teachBook teachBook;


    public void reset_(int i)
    {
        id = i;
        if (BagItemType_ != BagItemType.potion)
        {
            potion = null;
        }
        else
        {
            bagSoreIndex = 0;
        }

        if (BagItemType_ != BagItemType.other)
        {
            other = null;
        }
        else
        {
            bagSoreIndex = 1;
        }

        if (BagItemType_ != BagItemType.clothe)
        {
            clothe = null;
        }
        else
        {
            bagSoreIndex = 2;
        }

        if (BagItemType_ != BagItemType.material)
        {
            material = null;

        }
        else
        {
            bagSoreIndex = 3;
        }

        if (BagItemType_ != BagItemType.food)
        {
            food = null;
        }
        else
        {
            bagSoreIndex = 4;
        }

        if (BagItemType_ != BagItemType.tool)
        {
            tool = null;

        }
        else
        {
            bagSoreIndex = 5;
        }

        if (BagItemType_ != BagItemType.block)
        {
            block = null;
        }
        else
        {
            bagSoreIndex = 6;
        }

        if (BagItemType_ != BagItemType.Ingredients)
        {
            Ingredients = null;
        }
        else
        {
            bagSoreIndex = 7;
        }

        if (BagItemType_ != BagItemType.teachBook)
        {
            teachBook = null;
        }
        else
        {
            bagSoreIndex = 8;
        }
    }
#pragma warning restore CS0414
}


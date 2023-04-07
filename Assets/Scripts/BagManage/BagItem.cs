using UnityEngine;
using UnityEditor;

[System.Serializable]
[CreateAssetMenu(fileName = "BagItem", menuName = "Scriptables/BagItem", order = 2)]
public class BagItem : ScriptableObject
{
    public enum BagItemType
    {
        //物品欄右側裝備欄
        potion, equipment,//藥水,裝備

        //左側裝備欄
        clothe,//衣服

        //無法手持
        material,//材料

        //可以手持
        food, tool, block//食物,工具
    }
    [Header("基礎屬性")]
    public string BagItem_name;//物品名
    public Sprite BagItem_icon;//物品圖
    public string BagItem_info;//物品介紹

    public int sellPrice;//出售價格


#pragma warning disable CS0414

    [Header("物品類型")]
    public BagItemType BagItemType_;//物品類型

    [SerializeField] potion potion;
    [SerializeField] equipment equipment;
    public clothe clothe;
    [SerializeField] material material;
    [SerializeField] food food;
    [SerializeField] tool tool;
    [SerializeField] block block;


    private void OnValidate()
    {
        if (BagItemType_ != BagItemType.potion)
        {
            potion = null;
        }
        if (BagItemType_ != BagItemType.equipment)
        {
            equipment = null;
        }
        if (BagItemType_ != BagItemType.clothe)
        {
            clothe = null;
        }
        if (BagItemType_ != BagItemType.material)
        {
            material = null;
        }
        if (BagItemType_ != BagItemType.food)
        {
            food = null;
        }
        if (BagItemType_ != BagItemType.tool)
        {
            tool = null;
        }
        if (BagItemType_ != BagItemType.block)
        {
            block = null;
        }
    }
#pragma warning restore CS0414
}


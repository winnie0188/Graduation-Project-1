//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "merchantItem", menuName = "Scriptables/merchantItem", order = 1)]
public class merchantItem : ScriptableObject
{
    // Start is called before the first frame update
    public int price;//價格
    public BagItem merchantItem_BagItem;//販賣商品
    // public Sprite merchantItem_icon;
}

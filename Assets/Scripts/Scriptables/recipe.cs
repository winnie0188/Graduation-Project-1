using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "recipe", menuName = "Scriptables/recipe", order = 6)]
public class recipe : ScriptableObject
{
    [SerializeField] CookData[] bagItems = new CookData[9];
    [SerializeField] BagItem dishes;

    public CookData[] BagItems { get => bagItems; set => bagItems = value; }
    public BagItem Dishes { get => dishes; set => dishes = value; }
}

[System.Serializable]
public class CookData
{
    public BagItem bagItem;
    public int count;
}

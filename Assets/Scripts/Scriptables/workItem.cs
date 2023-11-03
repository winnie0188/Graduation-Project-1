using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "workItem", menuName = "Scriptables/workItem", order = 7)]
public class workItem : ScriptableObject
{
    [SerializeField] CookData[] bagItems = new CookData[9];
    [SerializeField] BagItem dishes;

    public CookData[] BagItems { get => bagItems; set => bagItems = value; }
    public BagItem Dishes { get => dishes; set => dishes = value; }
}



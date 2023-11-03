using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "workStore", menuName = "Store/workStore", order = 3)]
public class workStore : ScriptableObject
{
    [SerializeField] workItem[] workItems;

    public workItem[] WorkItems { get => workItems; set => workItems = value; }
}

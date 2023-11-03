using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "recipeStore", menuName = "Store/recipeStore", order = 2)]
public class recipeStore : ScriptableObject
{
    [SerializeField] recipe[] recipes;

    public recipe[] Recipes { get => recipes; set => recipes = value; }
}

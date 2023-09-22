using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanGreenObj : MonoBehaviour
{
    [SerializeField] Transform parentTransform;
    [ContextMenu("釋放空間")]
    public void Realse()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetChild(0).parent = parentTransform;
        }

    }
}

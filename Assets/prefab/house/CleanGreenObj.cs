using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanGreenObj : MonoBehaviour
{
    [SerializeField] Transform parentTransform;
    [SerializeField] Transform dynamicTransform;
    [ContextMenu("釋放空間")]
    public void Realse()
    {
        for (int i = 0; i < transform.childCount; i++)
        {

            if (transform.GetChild(i).GetComponent<blockData>().blockType == blockType.DEFAULT)
            {
                transform.GetChild(i).GetChild(0).parent = parentTransform;
            }
            else
            {
                transform.GetChild(i).GetComponent<blockData>().isDestory = true;
                transform.GetChild(i).parent = dynamicTransform;
                i = i - 1;
            }

        }
    }
}

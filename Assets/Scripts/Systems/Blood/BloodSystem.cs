using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSystem : MonoBehaviour
{
    [SerializeField] ParentDiction parentDiction;

    public static BloodSystem bloodSystem;
    public void Awake()
    {
        bloodSystem = this;
    }

    public void addBlood(Vector3 pos)
    {
        StartCoroutine(newBlood(pos));
    }

    IEnumerator newBlood(Vector3 pos)
    {
        GameObject addBlood = null;
        for (int i = 0; i < parentDiction.parent.childCount; i++)
        {
            if (parentDiction.parent.GetChild(i).gameObject.activeSelf == false)
            {
                addBlood = parentDiction.parent.GetChild(i).gameObject;
                addBlood.transform.position = pos;
                addBlood.SetActive(true);
                break;
            }
        }

        if (addBlood == null)
        {
            addBlood = Instantiate(parentDiction.prefab, pos, Quaternion.identity, parentDiction.parent);
        }

        yield return new WaitForSeconds(0.8f);

        addBlood.SetActive(false);
    }
}

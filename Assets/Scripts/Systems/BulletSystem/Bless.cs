using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bless : MonoBehaviour
{
    [SerializeField] float range;
    [SerializeField] float duration;
    [SerializeField] string key;
    [SerializeField] ParentDiction parentDiction;

    private void OnEnable()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.DOScale(new Vector3(range, range, range), duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Biology>(out Biology biology))
        {

            GameObject parent = addMagic(other.transform.position + new Vector3(0, 1, 0));

            BullectSystem.bullectSystem.fire(
                key,
                parent.transform,
                other.transform.position,
                other.transform,
                "monster",
                Vector3.zero
            );
        }
    }


    public GameObject addMagic(Vector3 pos)
    {
        GameObject magicRegion;
        for (int i = 0; i < parentDiction.parent.childCount; i++)
        {
            if (!parentDiction.parent.GetChild(i).gameObject.activeSelf)
            {
                //魔法陣
                magicRegion = parentDiction.parent.GetChild(i).gameObject;
                magicRegion.SetActive(true);
                magicRegion.transform.position = pos;


                return magicRegion;
            }
        }

        magicRegion = Instantiate(parentDiction.prefab, pos, Quaternion.identity);
        magicRegion.transform.parent = parentDiction.parent;

        return magicRegion;
    }
}

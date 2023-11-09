using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] bool isWear;

    public void wear(HotKey[] hotKeys)
    {
        if (hotKeys.Length > 0 && hotKeys[0].HotKey_Bag != -1 && hotKeys[0].HotKey_item != -1)
        {
            if (isWear == false)
            {
                isWear = true;
                StartCoroutine(running());
            }
        }
        else
        {
            relase();
        }
    }


    IEnumerator running()
    {
        yield return null;
        while (isWear)
        {
            yield return null;
            if (Input.GetKeyDown(playerController.playerController_.playerKeyCodes.summon))
            {
                BiologySystem.biologySystem.Lolo.attMode = !BiologySystem.biologySystem.Lolo.attMode;
            }
        }

        BiologySystem.biologySystem.Lolo.attMode = false;
    }

    public void summon()
    {
        BiologySystem.biologySystem.Lolo.attMode = !BiologySystem.biologySystem.Lolo.attMode;
    }

    public void relase()
    {
        isWear = false;

    }
}

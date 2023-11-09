using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    string currentBagItem;

    public void init(HotKey[] hotkeys)
    {
        if (hotkeys.Length > 0 && hotkeys[0].HotKey_Bag != -1 && hotkeys[0].HotKey_item != -1)
        {
            BagItem bagItem = BagManage.bagManage.bagSore[hotkeys[0].HotKey_Bag].BagItems[hotkeys[0].HotKey_item];
            if (currentBagItem != bagItem.BagItem_name)
            {
                //執行update
                StartCoroutine(running(bagItem.BagItem_name, bagItem.GetPtion()));
            }
        }
        else
        {
            currentBagItem = "";
        }
    }

    IEnumerator running(string thisBagItem, potion potion)
    {
        currentBagItem = thisBagItem;
        while (thisBagItem == currentBagItem)
        {
            yield return null;

            if (potion != null)
            {
                if (Input.GetKeyDown(playerController.playerController_.playerKeyCodes.useSkill))
                {
                    potion.Attack();
                    //print("attack");
                    yield return new WaitForSeconds(potion.attackPotion.second);
                }
            }

            //            print(currentBagItem);
        }
    }
}

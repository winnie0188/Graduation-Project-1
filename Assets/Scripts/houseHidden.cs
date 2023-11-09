using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class houseHidden : MonoBehaviour
{
    int i = 0;
    private void FixedUpdate()
    {
        if ((i < transform.childCount) == false)
        {
            i = 0;
        }


        if ((playerController.playerController_.transform.position - transform.GetChild(i).GetChild(2).position).magnitude > 100)
        {
            if (transform.GetChild(i).gameObject.activeSelf == true)
            {

                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            if (transform.GetChild(i).gameObject.activeSelf == false)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        i++;
    }
}

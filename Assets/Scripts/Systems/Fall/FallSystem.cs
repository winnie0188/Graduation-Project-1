using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallSystem : MonoBehaviour
{
    [SerializeField] Transform fallParent;
    [Header("之後會歸0")]
    [SerializeField] GameObject fallPrefab;

    [Header("幾秒後消失")]
    [SerializeField] float duration;
    public static FallSystem fallSystem;

    private void Awake()
    {
        fallSystem = this;
    }


    public void fall(BagItem bagItem, int count, Vector3 pos)
    {

        for (int i = 0; i < fallParent.childCount; i++)
        {
            if (!fallParent.GetChild(i).gameObject.activeSelf)
            {
                StartCoroutine(
                    fallUpdate(
                        bagItem,
                        fallParent.GetChild(i),
                        count,
                        pos
                    )
                );
                return;
            }
        }


        GameObject fall = Instantiate(fallPrefab);
        fall.transform.SetParent(fallParent);

        StartCoroutine(
            fallUpdate(
                bagItem,
                fall.transform,
                count,
                pos
            )
        );

    }

    IEnumerator fallUpdate(BagItem bagItem, Transform fallItem, int count, Vector3 pos)
    {
        fallItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = bagItem.BagItem_icon;
        float currentDeletTime = 0;
        float waitTime = 0.02f;
        Transform player = playerController.playerController_.transform;

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("block")))
        {
            pos = hit.point;
        }

        fallItem.transform.position = pos;
        fallItem.gameObject.SetActive(true);


        while (currentDeletTime < duration)
        {
            yield return new WaitForSeconds(waitTime);
            currentDeletTime += waitTime;
            if ((player.position - fallItem.position).magnitude < 1.5f)
            {
                currentDeletTime = duration;

                BagManage.bagManage.checkItem(
                    bagItem,
                    count,
                    false,
                    true
                );

            }
        }

        fallItem.gameObject.SetActive(false);
    }
}


[System.Serializable]
public class fallDatas : MonsterDinary
{
    public BagItem bagItem;
}
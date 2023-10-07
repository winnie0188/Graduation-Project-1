
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIinit : MonoBehaviour
{
    // Start is called before the first frame update
    #region initButton

    public void initSlot(int slotCount, GameObject slotPrefab, Transform slotContent, float size, int child)
    {
        //int length = 1;

        //創建slot
        for (int i = 0; i < slotCount; i++)
        {
            var index = i; // 保存当前索引值
            var temp = Instantiate(slotPrefab, slotContent.position, Quaternion.identity, slotContent);

            temp.transform.localScale = new Vector3(size, size, 1);

            if (child == -1)
            {
                StartCoroutine(AddListener(temp.GetComponent<Button>(), index));
            }
            else
            {
                StartCoroutine(AddListener(temp.transform.GetChild(child).GetComponent<Button>(), index));
            }
        }


        //length = (int)(slotContent.parent.GetComponent<RectTransform>().sizeDelta.x / size);

        //slotContent.position -= new Vector3(0, 1000, 0);
        //slotContent.GetComponent<GridLayoutGroup>().constraintCount = length;
    }

    IEnumerator AddListener(Button btn, int i)
    {
        btn.name = "" + i;
        btn.onClick.AddListener(() => slot_event(i));
        yield return null;
    }

    public virtual void slot_event(int i)
    {
        return;
    }


    public void initSlot2(int slotCount, GameObject slotPrefab, Transform slotContent, float size, int child)
    {
        //int length = 1;

        //創建slot
        for (int i = 0; i < slotCount; i++)
        {
            var index = i; // 保存当前索引值
            var temp = Instantiate(slotPrefab, slotContent.position, Quaternion.identity, slotContent);

            temp.transform.localScale = new Vector3(size, size, 1);

            if (child == -1)
            {
                StartCoroutine(AddListener2(temp.GetComponent<Button>(), index));
            }
            else
            {
                StartCoroutine(AddListener2(temp.transform.GetChild(child).GetComponent<Button>(), index));
            }
        }


        //length = (int)(slotContent.parent.GetComponent<RectTransform>().sizeDelta.x / size);

        //slotContent.position -= new Vector3(0, 1000, 0);
        //slotContent.GetComponent<GridLayoutGroup>().constraintCount = length;
    }

    IEnumerator AddListener2(Button btn, int i)
    {
        btn.name = "" + i;
        btn.onClick.AddListener(() => slot_event2(i));
        yield return null;
    }

    public virtual void slot_event2(int i)
    {
        return;
    }


    #endregion
}

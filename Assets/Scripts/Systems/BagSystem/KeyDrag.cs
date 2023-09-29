using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyDrag : MonoBehaviour
{

    Transform DragItem;
    Transform newDrag;
    HotKey hotKey;

    public void hotdrag(Transform DragItem, Transform newDrag, HotKey hotKey)
    {
        this.DragItem = DragItem;
        this.newDrag = newDrag;
        this.hotKey = hotKey;

        dragStart();
        StartCoroutine(hotdraging());
    }

    IEnumerator hotdraging()
    {
        yield return null;
        DragItem.position = Input.mousePosition;
        StartCoroutine(hotdraging());
    }

    //正常結束
    public void hotdragEnd(int state)
    {
        StopCoroutine(hotdraging());

        if (state == 0)
        {
            hotKey.HotKey_Bag = -1;
            hotKey.HotKey_item = -1;
            BagManage.bagManage.Refresh_HotKey();
        }

        dragCoverWait();
    }

    //道具交換位置
    public void switchSlot(Transform end, HotKey endkey)
    {
        if (newDrag.transform.parent != end.transform.parent)
        {
            hotdragEnd(1);
        }
        else
        {
            var temp = endkey;
            endkey.HotKey_Bag = hotKey.HotKey_Bag;
            endkey.HotKey_item = hotKey.HotKey_item;

            hotKey.HotKey_Bag = temp.HotKey_Bag;
            hotKey.HotKey_item = temp.HotKey_item;

            BagManage.bagManage.Refresh_HotKey();
            hotdragEnd(1);
        }
    }



    //=======================================================
    public void itemdrag(Transform DragItem, Transform newDrag)
    {

        this.DragItem = DragItem;
        this.newDrag = newDrag;

        dragStart();
        StartCoroutine(itemdraging());
    }

    IEnumerator itemdraging()
    {
        yield return null;

        DragItem.position = Input.mousePosition;

        StartCoroutine(itemdraging());

    }

    public void itemdragEnd()
    {
        StopCoroutine(itemdraging());
        dragCoverWait();
    }



    #region 重複出線的
    void dragStart()
    {
        DragItem.position = Input.mousePosition;
        DragItem.GetComponent<Image>().sprite = newDrag.GetChild(0).GetComponent<Image>().sprite;
        DragItem.gameObject.SetActive(true);

        newDrag.GetChild(0).GetComponent<Image>().color -= new Color(0, 0, 0, 1);
        newDrag.GetComponent<CanvasGroup>().blocksRaycasts = false;//射線阻擋關閉
    }
    void dragCoverWait()
    {
        DragItem.gameObject.SetActive(false);

        newDrag.GetChild(0).GetComponent<Image>().color += new Color(0, 0, 0, 1);
        newDrag.GetComponent<CanvasGroup>().blocksRaycasts = true;//射線阻擋

        BagManage.bagManage.HotKeyState = DragState.WAIT;
    }

    #endregion

    //強制取消 
    //我放在背包背景ui click
    public void clean()
    {
        if (BagManage.bagManage.HotKeyState == DragState.BAGITEMDRAG)
        {
            itemdragEnd();
        }
        else if (BagManage.bagManage.HotKeyState == DragState.HOTKEYDRAG)
        {
            hotdragEnd(1);
        }
    }

}

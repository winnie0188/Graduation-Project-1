using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BagItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Start is called before the first frame update
    public void OnBeginDrag(PointerEventData eventData)
    {
        BagManage.bagManage.initDrag(transform);
        GetComponent<CanvasGroup>().blocksRaycasts = false;//射線阻擋關閉

    }

    public void OnDrag(PointerEventData eventData)
    {
        BagManage.bagManage.DragItem.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool isToch = false;

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {

            if (eventData.pointerCurrentRaycast.gameObject.tag == "HotKeyTag")
            {
                isToch = true;
            }
        }


        BagManage.bagManage.DragEnd(transform, eventData.pointerCurrentRaycast.gameObject.transform, isToch);
        BagManage.bagManage.DragItem.gameObject.SetActive(false);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

/// <summary>
/// 新手引导动画
/// </summary>

public class Guide : MonoBehaviour
{
    public Image target;
    [SerializeField] Transform arrow;
    [SerializeField] Text content;
    public Transform maskPanel;
    public guideDataList[] guideList;

    private Material material;
    private float diameter; // 直径
    private float current = 0f;
    public Canvas canvas1;

    Vector3[] corners = new Vector3[4];


    public static Guide guide;

    bool isEnd = true;

    Tween arrowAni;



    void Awake()
    {
        guide = this;

    }

    IEnumerator setTargetCot(guideDataList guideDataList, int index, GameObject removeObject)
    {
        if (removeObject == null)
        {
            guideList[index].unityActions.Clear();
        }

        target = null;
        var guideDatas = guideDataList.guideData;

        if (guideDatas.Count > index)
        {
            if (guideDatas[index].image != null)
            {
                target = guideDatas[index].image;
            }

            if (guideDatas[index].imagePath.Length > 0)
            {
                GameObject temp = null;
                while (temp == null)
                {
                    yield return null;

                    temp = GameObject.Find(guideDatas[index].imagePath);


                    if (temp != null && temp.TryGetComponent<Image>(out var ima))
                    {

                        target = ima;
                    }
                }
            }

            //UISystem/BagPanel/bagBackground/contentmask/background/slotContentParent/服裝/1
            // 设置事件透传对象
            maskPanel.GetChild(0).gameObject.GetComponent<EventPermeate>().target = target.gameObject;

            Canvas canvas = canvas1;
            target.rectTransform.GetWorldCorners(corners);
            diameter = Vector2.Distance(WordToCanvasPos(canvas, corners[0]), WordToCanvasPos(canvas, corners[2])) / 2f;

            float x = corners[0].x + ((corners[3].x - corners[0].x) / 2f);
            float y = corners[0].y + ((corners[1].y - corners[0].y) / 2f);

            Vector3 center = new Vector3(x, y, 0f);
            Vector2 position = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, center, canvas.GetComponent<Camera>(), out position);

            center = new Vector4(position.x, position.y, 0f, 0f);
            material = maskPanel.GetChild(0).GetComponent<Image>().material;
            material.SetVector("_Center", center);

            (canvas.transform as RectTransform).GetWorldCorners(corners);
            for (int i = 0; i < corners.Length; i++)
            {
                current = Mathf.Max(Vector3.Distance(WordToCanvasPos(canvas, corners[i]), center), current);
            }

            material.SetFloat("_Silder", diameter);

            arrow.position = target.transform.position;
            this.content.text = guideDatas[index].content;
            var contentBack = this.content.transform.parent.GetComponent<RectTransform>();


            if (arrow.GetComponent<RectTransform>().anchoredPosition.x > 0)
            {

                Vector3 oldPos = contentBack.anchoredPosition;
                oldPos.x = -330;
                contentBack.anchoredPosition = oldPos;

            }
            else if (arrow.GetComponent<RectTransform>().anchoredPosition.x <= 0)
            {

                Vector3 oldPos = contentBack.anchoredPosition;
                oldPos.x = 330;
                contentBack.anchoredPosition = oldPos;
            }
            if (arrow.GetComponent<RectTransform>().anchoredPosition.y > 0)
            {
                Vector3 oldPos = contentBack.anchoredPosition;
                oldPos.y = -130;
                contentBack.anchoredPosition = oldPos;


                var arrow = this.arrow.GetChild(0).GetComponent<RectTransform>();
                oldPos = arrow.anchoredPosition;
                oldPos.y = -130;
                arrow.anchoredPosition = oldPos;

                arrow.rotation = Quaternion.Euler(180, 0, 0);
                arrowAni = arrow.GetChild(0).DOLocalMoveY(-10, 0.3f).SetLoops(-1, LoopType.Yoyo);
            }
            else if (arrow.GetComponent<RectTransform>().anchoredPosition.y <= 0)
            {
                Vector3 oldPos = contentBack.anchoredPosition;
                oldPos.y = 130;
                contentBack.anchoredPosition = oldPos;

                var arrow = this.arrow.GetChild(0).GetComponent<RectTransform>();

                oldPos = arrow.anchoredPosition;
                oldPos.y = 130;
                arrow.anchoredPosition = oldPos;

                arrow.rotation = Quaternion.Euler(0, 0, 0);
                arrowAni = arrow.GetChild(0).DOLocalMoveY(10, 0.3f).SetLoops(-1, LoopType.Yoyo);
            }

            if (guideDataList.guideData.Count >= index)
            {
                guideDataList.unityActions.Add(target.transform, () => setTarget(guideDataList, index + 1, target.gameObject));
                target.GetComponent<Button>().onClick.AddListener(guideDataList.unityActions[target.transform]);
            }
        }
        else
        {
            guideDataList.currentIndex = index;
        }

        isEnd = true;

        if (removeObject != null)
        {
            yield return null;
            removeObject.GetComponent<Button>().onClick.RemoveListener(guideDataList.unityActions[removeObject.transform]);
        }
    }



    public void setTarget(guideDataList guideList, int index, GameObject removeObject)
    {
        if (isEnd == true)
        {
            isEnd = false;
            StartCoroutine(setTargetCot(guideList, index, removeObject));
        }
    }


    Vector2 WordToCanvasPos(Canvas canvas, Vector3 world)
    {
        Vector2 position = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, world, canvas.GetComponent<Camera>(), out position);
        return position;
    }

}



[System.Serializable]
public class guideData
{
    public Image image;
    public string imagePath;
    public string content;
}

[System.Serializable]
public class guideDataList
{
    public List<guideData> guideData = new List<guideData>();
    public Dictionary<Transform, UnityAction> unityActions = new Dictionary<Transform, UnityAction>();
    public int currentIndex;
}

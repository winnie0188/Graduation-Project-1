using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigMapSystem : MonoBehaviour
{
    // Start is called before the first frame update
    #region 地圖邊界
    [SerializeField] Transform controlBorder;
    [SerializeField] Transform Border;
    #endregion


    [SerializeField] Transform bigCamera;
    #region 攝影機位置
    // 初始位置
    Vector3 initialMousePosition;

    // 地圖基準點
    Vector3 initialMapPosition;

    bool isDragging;
    #endregion


    #region slider控制大小
    [SerializeField] Slider slider;
    float sliderValue = 5;
    #endregion

    //隱藏語顯示
    [SerializeField] Sprite[] showHide;
    [SerializeField] Image[] TextIconImage;
    [SerializeField] Transform[] TextIcon;

    #region 地圖標記
    [SerializeField] Transform markerEditPanel;
    [SerializeField] Transform[] markerPrefab;
    GameObject TempMarker;
    // 以標記數量
    int markCount;

    // [SerializeField] Transform PlayerMark;

    #endregion

    //縮放
    float size = 5;


    [SerializeField] Transform MapShader;


    public static BigMapSystem bigMapSystem;


    private void Awake()
    {
        bigMapSystem = this;

        // // 註冊 Slider 的事件監聽器
        // slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void Update_MapLoad()
    {
        StartCoroutine(Update_mapLoad());
    }

    IEnumerator Update_mapLoad()
    {
        GameObject BigMapPanel = PanelManage.panelManage.panels.BigMapPanel.gameObject;

        bigCamera.gameObject.SetActive(true);

        do
        {
            yield return null;
            DragMap();
            markMode();
            detectBorder();

        } while (BigMapPanel.activeSelf);

        bigCamera.gameObject.SetActive(false);
    }

    public void detectBorder()
    {
        float x = 0;
        float y = 0;
        //左
        if (controlBorder.GetChild(0).position.x < Border.GetChild(0).position.x)
        {
            x = -1000;
        }
        //右
        else if (controlBorder.GetChild(1).position.x > Border.GetChild(1).position.x)
        {
            x = 1000;
        }

        //下
        if (controlBorder.GetChild(2).position.y < Border.GetChild(2).position.y)
        {
            y = -1000;
        }
        //上
        else if (controlBorder.GetChild(3).position.y > Border.GetChild(3).position.y)
        {
            y = 1000;
        }

        MapShader.transform.GetChild(0).Translate(x * Time.deltaTime, y * Time.deltaTime, 0);
    }



    // 脫拽地圖
    public void DragMap()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 滑鼠按下時，記錄初始點位置和地圖位置
            initialMousePosition = Input.mousePosition;
            initialMapPosition = MapShader.transform.GetChild(0).position;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // 釋放滑鼠時，停止拖拽
            isDragging = false;
        }

        if (isDragging)
        {
            // 計算滑鼠位移量
            Vector3 mouseDelta = Input.mousePosition - initialMousePosition;

            // 將位移量應用到地圖上
            MapShader.transform.GetChild(0).position = initialMapPosition + mouseDelta;
        }
    }

    // 放大縮小
    // private void OnSliderValueChanged(float value)
    // {
    //     isDragging = false;
    //     sliderValue = value;

    //     MapShader.transform.localScale = new Vector3(1, 1, 1) * value / 5.0f;
    // }

    public void addSize(int value)
    {
        if (size + value > 100)
        {
            size = 100;
        }
        else if (size + value < 5)
        {
            size = 5;
        }
        else
        {
            size += value;
        }

        MapShader.transform.localScale = new Vector3(1, 1, 1) * size / 5.0f;
    }



    #region 地圖標記
    public void markMode()
    {
        if (Input.GetMouseButtonDown(1))
        {
            markPos();
        }
    }

    public void markPos()
    {
        // -320 -180
        if (markerPrefab.Length <= markCount)
            return;

        for (int i = 0; i < markerPrefab.Length; i++)
        {
            if (markerPrefab[i].gameObject.activeSelf == false)
            {
                markerPrefab[i].gameObject.SetActive(true);
                markerPrefab[i].position = Input.mousePosition;
                break;
            }
        }

        markCount += 1;
        // 關閉標記模式
    }

    //移除標記==========================================
    public void openMarkEditPanel(GameObject marker)
    {
        markerEditPanel.gameObject.SetActive(true);
        TempMarker = marker;
    }

    public void closeMarkerEditPanel()
    {
        markerEditPanel.gameObject.SetActive(false);
    }

    public void destoryMarker()
    {
        markCount -= 1;
        TempMarker.SetActive(false);
        markerEditPanel.gameObject.SetActive(false);
    }

    #endregion


    #region 隱藏與顯示
    public void TextShow()
    {
        if (TextIcon[0].gameObject.activeSelf)
        {
            TextIcon[0].gameObject.SetActive(false);
            TextIconImage[0].sprite = showHide[1];
        }
        else
        {
            TextIcon[0].gameObject.SetActive(true);
            TextIconImage[0].sprite = showHide[0];
        }
    }

    public void IconShow()
    {
        if (TextIcon[1].gameObject.activeSelf)
        {
            TextIcon[1].gameObject.SetActive(false);
            TextIconImage[1].sprite = showHide[1];
        }
        else
        {
            TextIcon[1].gameObject.SetActive(true);
            TextIconImage[1].sprite = showHide[0];
        }
    }
    #endregion
}

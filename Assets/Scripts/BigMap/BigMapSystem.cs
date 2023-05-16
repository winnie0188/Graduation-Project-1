using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigMapSystem : MonoBehaviour
{
    // Start is called before the first frame update
    #region 攝影機位置
    // 初始位置
    Vector3 initialMousePosition;

    // 地圖基準點
    Vector3 initialMapPosition;

    bool isDragging;
    #endregion


    #region slider控制攝影機y
    public Slider slider;
    public GameObject targetObject;
    float sliderValue = 50;
    #endregion

    public static BigMapSystem bigMapSystem;

    private void Awake()
    {
        bigMapSystem = this;

        // 註冊 Slider 的事件監聽器
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }


    #region 放大地圖
    private void OnSliderValueChanged(float value)
    {
        isDragging = false;
        sliderValue = value;

        // 將 Slider 的值應用到目標物件的 posY
        Vector3 newPosition = targetObject.transform.position;
        newPosition.y = value * 10;
        targetObject.transform.position = newPosition;
    }
    #endregion

    #region 移動地圖
    public void Update_Map_Void()
    {
        StartCoroutine(Update_Map());
    }

    IEnumerator Update_Map()
    {
        yield return null;
        GameObject BigMapPanel = PanelManage.panelManage.panels.BigMapPanel.gameObject;
        while (BigMapPanel.activeSelf)
        {
            yield return null;
            MoveMap();
        }
    }


    void MoveMap()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 滑鼠按下時，記錄初始點位置和地圖位置
            initialMousePosition = Input.mousePosition;
            initialMapPosition = targetObject.transform.position;
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
            targetObject.transform.position = initialMapPosition + new Vector3(-mouseDelta.x, 0, -mouseDelta.y) * (sliderValue / 1000.0f) * 5;
        }
    }
    #endregion
}

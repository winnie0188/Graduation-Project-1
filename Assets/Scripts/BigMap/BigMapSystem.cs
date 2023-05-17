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


    #region slider控制大小
    [SerializeField] Slider slider;
    float sliderValue = 5;
    #endregion

    #region 地圖標記
    [SerializeField] Transform[] markerPrefab;
    // 以標記數量
    int markCount;

    [SerializeField] Transform PlayerMark;

    #endregion


    [SerializeField] Transform MapShader;


    public static BigMapSystem bigMapSystem;




    private void Awake()
    {
        bigMapSystem = this;

        // 註冊 Slider 的事件監聽器
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void Update_MapLoad()
    {
        StartCoroutine(Update_mapLoad());
    }



    IEnumerator Update_mapLoad()
    {
        GameObject BigMapPanel = PanelManage.panelManage.panels.BigMapPanel.gameObject;

        do
        {
            yield return null;
            PlayerPosMap();
            DragMap();
            markMode();

        } while (BigMapPanel.activeSelf);
    }

    // 更新玩家在地圖上的位置
    public void PlayerPosMap()
    {
        PlayerMark.localPosition = new Vector3(playerController.playerController_.transform.position.x / 2.0f, playerController.playerController_.transform.position.z / 2.0f, PlayerMark.localPosition.z);
    }

    // 脫拽地圖
    public void DragMap()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 滑鼠按下時，記錄初始點位置和地圖位置
            initialMousePosition = Input.mousePosition;
            initialMapPosition = MapShader.transform.position;
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
            MapShader.transform.position = initialMapPosition + mouseDelta;
        }
    }

    // 放大縮小
    private void OnSliderValueChanged(float value)
    {
        isDragging = false;
        sliderValue = value;

        MapShader.transform.localScale = new Vector3(1, 1, 1) * value / 5.0f;
    }

    #region 地圖標記
    public void markMode()
    {
        if (Input.GetMouseButtonDown(1))
        {
            markPos(markCount);
        }
    }

    public void markPos(int i)
    {
        // -320 -180
        if (markerPrefab.Length <= markCount)
            return;
        markerPrefab[i].gameObject.SetActive(true);

        markerPrefab[i].position = Input.mousePosition;


        markCount = i + 1;
        // 關閉標記模式
    }
    #endregion
}

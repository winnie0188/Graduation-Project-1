using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class taskSystem : UIinit
{
    // Start is called before the first frame update
    // 任務列表
    [SerializeField] task[] tasks;

    #region slot
    [SerializeField] Transform taskContentPanel;
    [SerializeField] int slotCount;
    [SerializeField] GameObject slotPrefab;

    #endregion

    // 遊玩時顯示在右上角的panel
    [SerializeField] Transform BasicUiTaskPanel;


    void Awake()
    {
        initSlot(slotCount, slotPrefab, taskContentPanel, 200.0f, 2);


        UpdateTaskUI();
        taskContentPanel.position -= new Vector3(0, 2000, 0);
    }

    #region 更新UI
    void UpdateTaskUI()
    {
        for (int i = 0; i < taskContentPanel.childCount; i++)
        {
            if (i < tasks.Length)
            {
                taskContentPanel.GetChild(i).GetChild(0).GetComponent<Text>().text = tasks[i].task_Title;
                taskContentPanel.GetChild(i).GetChild(1).GetComponent<Text>().text = tasks[i].task_Content;
            }
        }

        // 更新玩家能看到的任務
        BasicUiTaskPanel.GetChild(0).GetComponent<Text>().text = tasks[0].task_Title;
        BasicUiTaskPanel.GetChild(1).GetComponent<Text>().text = tasks[1].task_Content;
    }
    #endregion



    #region 更新釘選
    public override void slot_event(int i)
    {
        // 當前位置置頂
        var temp = tasks[0];
        tasks[0] = tasks[i];
        tasks[i] = temp;

        // 更新UI
        UpdateTaskUI();
    }
    #endregion

    #region 展開/隱藏
    // 展開置頂或隱藏
    public void ShowAHide()
    {
        if (BasicUiTaskPanel.gameObject.activeSelf)
        {
            BasicUiTaskPanel.gameObject.SetActive(false);
        }
        else
        {
            BasicUiTaskPanel.gameObject.SetActive(true);
        }
    }
    #endregion
}

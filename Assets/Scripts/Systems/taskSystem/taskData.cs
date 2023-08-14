using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class taskData : MonoBehaviour
{
    // 數據交換時，需要這個id
    [SerializeField] int id;
    [SerializeField] Text TaskTitle;
    [SerializeField] Text TaskContent;
    [SerializeField] Text TaskNeed;
    [SerializeField] Text TaskGet;
    [SerializeField] Image TaskTop;


    // 一行最多幾個字
    [SerializeField] int Line;

    public void setID(int id)
    {
        this.id = id;
    }


    public void setTaskTitle(string TaskTitle)
    {
        this.TaskTitle.text = "任務標題:" + TaskTitle;
    }

    public void setTaskContent(string TaskContent)
    {
        string TaskContentln;

        if (TaskContent.Length > Line)
        {
            TaskContentln = "";
            while (TaskContent.Length > Line)
            {

                TaskContentln += TaskContent.Substring(0, Line) + "\n";

                TaskContent = TaskContent.Substring(Line - 1, TaskContent.Length - Line + 1);
            }

            TaskContentln += TaskContent;
        }
        else
        {
            TaskContentln = TaskContent;
        }

        this.TaskContent.text = "任務說明:\n" + TaskContentln;
    }
    public void setTaskNeed(string TaskNeed)
    {
        string TaskNeedln;

        if (TaskNeed.Length > Line)
        {
            TaskNeedln = "";
            while (TaskNeed.Length > Line)
            {
                TaskNeedln += TaskNeed.Substring(0, Line) + "\n";

                TaskNeed = TaskNeed.Substring(Line - 1, TaskNeed.Length - Line + 1);
            }

            TaskNeedln += TaskNeed;
        }
        else
        {
            TaskNeedln = "任務要求:" + TaskNeed;
        }


        this.TaskNeed.text = TaskNeedln;
    }
    public void setTaskGet(string TaskGet)
    {
        string TaskGetln;

        if (TaskGet.Length > Line)
        {
            TaskGetln = "";
            while (TaskGet.Length > Line)
            {
                TaskGetln += TaskGet.Substring(0, Line) + "\n";

                TaskGet = TaskGet.Substring(Line - 1, TaskGet.Length - Line + 1);
            }

            TaskGetln += TaskGet;
        }
        else
        {
            TaskGetln = TaskGet;
        }

        this.TaskGet.text = "任務獎勵:\n" + TaskGetln;
    }


    public Text GetTaskTitle()
    {
        return TaskTitle;
    }

    public Text GetTaskContent()
    {
        return TaskContent;
    }

    public Text GetTaskNeed()
    {
        return TaskNeed;
    }

    public Text GetTaskGet()
    {
        return TaskGet;
    }

    public void setTaskTop(Sprite TopBtnIcon)
    {
        TaskTop.sprite = TopBtnIcon;
    }


    // 置頂 呼叫System進行數據交換
    public void inputTop()
    {
        taskSystem.taskSystem_.switchCurrentTaskArray(id);
    }
}

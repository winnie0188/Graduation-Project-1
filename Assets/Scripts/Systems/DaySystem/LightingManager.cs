using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// [ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Camera DirectionalLight;
    [SerializeField] LightintPreset Preset;
    [SerializeField, Range(0, 24)] float TimeOfDay;

    // 指針
    [SerializeField] Transform DayPoint;
    [SerializeField] Text DayTime;


    // 紀錄要旋轉的Z值
    float[] timeOfDayToZ;
    private float timer;
    // 更新時鐘需要的時間
    [SerializeField] float updateInterval;

    #region 紀錄日期


    [SerializeField] DayData dayData;
    int preHour;
    #endregion

    public static LightingManager lightingManager;

    public void addTime()
    {
        // if (i == 1)
        // {
        //     TimeOfDay = 12;
        // }
        // else
        // {
        //     TimeOfDay = 24;
        // }
        TimeOfDay += 1;
    }

    private void Awake()
    {
        lightingManager = this;

        timeOfDayToZ = new float[30 * 60];

        for (int i = 360; i < 1800; i++)
        {
            float z = Mathf.Lerp(90f, -90f, Mathf.InverseLerp(360, 1800, i));
            timeOfDayToZ[i] = z;
        }

        int hour = (int)TimeOfDay;
        preHour = hour;
        UpdateLighting(TimeOfDay / 24.0f);
    }


    private void Update()
    {
        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime / 75.0f;
            TimeOfDay %= 24;

            // 每updateInterval秒更新
            timer += Time.deltaTime;
            if (timer > updateInterval)
            {
                // 在這裡執行程式碼
                rotationPoint();
                timer = 0.0f;
            }


            // UpdateLighting(TimeOfDay / 24.0f);
        }
        // else
        // {
        //     UpdateLighting(TimeOfDay / 24.0f);
        // }
    }

    #region 選轉陽光
    private void UpdateLighting(float timePercent)
    {
        // RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        // RenderSettings.fogColor = Preset.Fogcolor.Evaluate(timePercent);

        if (DirectionalLight != null)
        {
            DirectionalLight.backgroundColor = Preset.DirectionalColor.Evaluate(timePercent);
            //DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170, 0));
        }
    }
    #endregion

    #region 轉動指針以及更新時間
    void rotationPoint()
    {
        int hour = (int)TimeOfDay;
        int minute = Mathf.FloorToInt((TimeOfDay - hour) * 60);

        DayTime.text = string.Format("{0:D2}:{1:D2}", hour, minute);
        float z;

        if (hour < 6)
        {
            z = timeOfDayToZ[(hour + 24) * 60 + minute];
            DayPoint.eulerAngles = new Vector3(DayPoint.eulerAngles.x, DayPoint.eulerAngles.y, z);
        }
        else
        {
            z = timeOfDayToZ[(hour) * 60 + minute];
            DayPoint.eulerAngles = new Vector3(DayPoint.eulerAngles.x, DayPoint.eulerAngles.y, z);
        }

        if (preHour != hour)
        {
            if (hour == 0)
            {
                int year = dayData.year;
                int day = dayData.day;
                int month = dayData.month;

                day += 1;
                if (day == 31)
                {
                    day = 1;
                    month += 1;
                    if (month == 13)
                    {
                        month = 1;
                        year += 1;
                    }
                }

                int week = ((dayData.week + 1) + WeekStr.week.Length) % WeekStr.week.Length;


                setDate(month, day);
                setYear(year);
                setWeek(week);
            }


            NpcFactory.npcFactory.getDateSign(dayData.day, hour);
            UpdateLighting(TimeOfDay / 24.0f);
        }

        preHour = hour;
    }


    public void setYear(int year)
    {
        dayData.year = year;
        dayData.yearPanel.text = "地球歷" + year + "年";
    }

    public void setDate(int month, int day)
    {
        dayData.month = month;
        dayData.day = day;

        dayData.datePanel.text = month + "月" + day + "日";
    }

    public int getMonth()
    {
        return dayData.month;
    }

    public void setWeek(int week)
    {
        dayData.week = week;
        dayData.weekPanel.text = WeekStr.week[week];
    }


    #endregion



}

[System.Serializable]
public class DayData
{
    //預設517
    public int year;
    //1
    public int month;
    //1
    public int day;
    //2
    public int week;

    public Text yearPanel;
    public Text datePanel;
    public Text weekPanel;
}

public static class WeekStr
{
    public static readonly string[] week ={
        "星期一",
        "星期二",
        "星期三",
        "星期四",
        "星期五",
        "星期六",
        "星期日"
    };
}

// #region 選轉陽光
// private void UpdateLighting(float timePercent)
// {
//     RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
//     RenderSettings.fogColor = Preset.Fogcolor.Evaluate(timePercent);

//     if (DirectionalLight != null)
//     {
//         DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

//         DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170, 0));
//     }
// }
// #endregion

// private void OnValidate()
// {
//     if (DirectionalLight != null)
//     {
//         return;
//     }
//     if (RenderSettings.sun != null)
//     {
//         DirectionalLight = RenderSettings.sun;
//     }
//     else
//     {
//         Light[] lights = GameObject.FindObjectsOfType<Light>();
//         foreach (Light light in lights)
//         {
//             if (light.type == LightType.Directional)
//             {
//                 DirectionalLight = light;
//                 return;
//             }
//         }
//     }
// }
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Light DirectionalLight;
    [SerializeField] LightintPreset Preset;
    [SerializeField, Range(0, 24)] float TimeOfDay;

    // 指針
    [SerializeField] Transform DayPoint;
    [SerializeField] Text DayTime;


    // 紀錄要旋轉的Z值
    Dictionary<float, float> timeOfDayToZ;

    private void Awake()
    {
        timeOfDayToZ = new Dictionary<float, float>();

        for (int i = 6; i < 30; i++)
        {
            float z = Mathf.Lerp(90f, -90f, Mathf.InverseLerp(6f, 30f, i));
            timeOfDayToZ[i] = z;
        }
    }


    private void Update()
    {
        if (Preset == null)
        {
            return;
        }
        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime / 75.0f;
            TimeOfDay %= 24;

            int hour = (int)TimeOfDay;
            int minute = Mathf.FloorToInt((TimeOfDay - hour) * 60);

            DayTime.text = string.Format("{0:D2}:{1:D2}", hour, minute);


            float z;
            if (timeOfDayToZ.TryGetValue(hour, out z))
            {
                DayPoint.eulerAngles = new Vector3(DayPoint.eulerAngles.x, DayPoint.eulerAngles.y, z);
            }




            UpdateLighting(TimeOfDay / 24.0f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24.0f);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.Fogcolor.Evaluate(timePercent);

        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170, 0));
        }
    }

    private void OnValidate()
    {
        if (DirectionalLight != null)
        {
            return;
        }
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
}

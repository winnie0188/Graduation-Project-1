
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Scriptables/Lighting preset", order = 3)]
public class LightintPreset : ScriptableObject
{
    // Start is called before the first frame update
    public Gradient AmbientColor;
    public Gradient DirectionalColor;
    public Gradient Fogcolor;
}

using UnityEngine;

public class MapSlot : MonoBehaviour
{
    public void openInfo()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
    public void closeInfo()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}

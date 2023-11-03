using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMark : MonoBehaviour
{
    public void openPanel()
    {
        BigMapSystem.bigMapSystem.openMarkEditPanel(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicItem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int index;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "player")
        {
            if (MusicSystem.musicSystem != null)
            {
                MusicSystem.musicSystem.switchMusic(index);
            }
            else
            {
                FindObjectOfType<MusicSystem>().switchMusic(index);
            }
        }

    }
}

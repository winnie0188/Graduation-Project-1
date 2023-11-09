using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSystem : MonoBehaviour
{
    AudioSource audioSource;

    [Header("音量")]
    public static float musicVolume;

    public int thisIndex;

    [SerializeField] AudioClip[] audioClip;

    public static MusicSystem musicSystem;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        musicSystem = this;
    }

    public void switchMusic(int index)
    {
        if (thisIndex != index)
        {
            thisIndex = index;
            audioSource.clip = audioClip[index];
            audioSource.Play();
        }
    }
}

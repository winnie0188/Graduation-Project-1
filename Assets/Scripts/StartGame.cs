using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartG()
    {
        SceneManager.LoadScene("myHw");
    }

    public void Exit()
    {
        Application.Quit();
    }
}

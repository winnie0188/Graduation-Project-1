using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinshUi : MonoBehaviour
{
    public Transform uiCanvas;
    [SerializeField] GameObject itemPrefab;
    List<GameObject> gameObjects = new List<GameObject>();
    [SerializeField] Transform ListParent;

    [SerializeField] FinshTalk finshTalk;
    [SerializeField] FinshTask finshTask;

    public static FinshUi finshUi;

    private void Awake()
    {
        finshUi = this;
    }
    public void openCanvas(List<Sprite> sprites, FinshTalk talk, FinshTask task)
    {
        finshTalk = talk;
        finshTask = task;

        uiCanvas.gameObject.SetActive(true);

        for (int i = 0; i < ListParent.childCount; i++)
        {
            ListParent.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < sprites.Count; i++)
        {
            if (gameObjects.Count > i)
            {
                gameObjects[i].SetActive(true);
            }
            else
            {
                GameObject temp = Instantiate(itemPrefab);
                temp.transform.SetParent(ListParent);

                gameObjects.Add(temp);
            }

            gameObjects[i].GetComponent<Image>().sprite = sprites[i];
        }
    }

    public void closeCanvas()
    {
        uiCanvas.gameObject.SetActive(false);

        if (finshTalk != null)
        {
            talkSystem.talkSystem_.finshTalk(finshTalk, false);
        }
        if (finshTask != null)
        {
            taskSystem.taskSystem_.finshTask(finshTask, false);
        }
    }
}

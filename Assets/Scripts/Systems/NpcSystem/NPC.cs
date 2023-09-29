using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class NPC : MonoBehaviour
{
    public NPCstate NPCstate = NPCstate.WAIT;
    public NPCtype NPCtype;
    public float speed;
    public Rigidbody rigi;


    #region 頭頂對話

    [Header("這邊是處理頭頂框的")]
    public Transform talkView;

    [SerializeField] TMP_Text content;
    public bool isFinshTalk = true;
    //僵直時間
    [SerializeField] float Stiff;
    #endregion

    #region 要接的任務
    [Header("可接的任務")]
    [SerializeField] taskItem taskItem;
    [Header("驚嘆號")]
    [SerializeField] Transform exclamation;
    #endregion


    private void Awake()
    {
        rigi = GetComponent<Rigidbody>();
    }

    //相當於start
    public void Init(NPCstate state)
    {
        NPCstate = state;
        switch (NPCtype)
        {
            case NPCtype.ENTOURAGE:
                entourageInit();
                break;
            case NPCtype.MERCHANT:
                merchantInit();
                break;
            default:
                break;
        }
    }

    //相當於update
    public void Runnig()
    {
        transform.GetChild(0).rotation = playerController.playerController_.transform.rotation;

        UpdateTaskUi();

        switch (NPCtype)
        {
            case NPCtype.ENTOURAGE:
                entourageUpdate();
                break;
            case NPCtype.MERCHANT:
                merchantUpdate();
                break;
            default:
                break;
        }
    }
    #region 每個類型npc的init
    void entourageInit()
    {
        switch (NPCstate)
        {
            case NPCstate.TASK:
                break;
            case NPCstate.WAIT:
                break;
            case NPCstate.WALK:
                break;
            default:
                break;
        }
    }
    void merchantInit()
    {
        switch (NPCstate)
        {
            case NPCstate.TASK:
                break;
            case NPCstate.WAIT:
                break;
            case NPCstate.WALK:
                break;
            default:
                break;
        }
    }
    #endregion

    #region 每個類型npc的update
    void entourageUpdate()
    {
        switch (NPCstate)
        {
            case NPCstate.TASK:
                break;
            case NPCstate.WAIT:
                break;
            case NPCstate.WALK:
                break;
            default:
                break;
        }
    }

    void merchantUpdate()
    {
        switch (NPCstate)
        {
            case NPCstate.TASK:
                break;
            case NPCstate.WAIT:
                break;
            case NPCstate.WALK:
                walk();
                break;
            default:
                break;
        }
    }
    #endregion


    public void walk()
    {
        //之後再補判斷牆壁旋轉

        walkFront();
    }

    public void LookAt(Vector3 endPOS)
    {
        Vector3 endLine = endPOS;
        Vector3 startLine = transform.position;
        transform.rotation =
        Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(
                new Vector3(endLine.x, 0, endLine.z) - new Vector3(startLine.x, 0, startLine.z)
            ),
            100
        );
    }

    public void walkFront()
    {
        Vector3 v = rigi.velocity;
        v.x = 0;
        v.z = 0;

        rigi.velocity = v;

        Vector3 localMovement = new Vector3(0, 0, 1);
        Vector3 worldMovement = transform.TransformDirection(localMovement);
        rigi.velocity = new Vector3(worldMovement.x * speed, rigi.velocity.y, worldMovement.z * speed);
    }

    #region 頭頂對話
    public void setContent(string content)
    {
        if (isFinshTalk)
        {
            this.content.text = "";
            isFinshTalk = false;
            if (!talkView.gameObject.activeSelf)
            {
                Image image = talkView.GetComponent<Image>();
                var a = image.color;
                a.a = 0;
                image.color = a;
                // talkView.localPosition = Vector3.zero;

                Sequence se = DOTween.Sequence();

                talkView.gameObject.SetActive(true);
                // se.Append(talkView.DOLocalMoveY(2.3137f, 0.5f).SetEase(Ease.Flash));
                se.Append(image.DOFade(1, 0.5f).SetEase(Ease.InSine));


                se.Play();

                se.OnComplete(() =>
                {
                    StartCoroutine(talk(content));
                });
            }
            else
            {
                StartCoroutine(talk(content));
            }
        }
    }

    IEnumerator talk(string content)
    {
        for (int i = 0; i < content.Length; i++)
        {
            this.content.text += content[i];
            yield return new WaitForSeconds(0.05f);
        }

        this.content.text = content;
        yield return new WaitForSeconds(Stiff);
        isFinshTalk = true;
    }

    public void closeConetnt()
    {
        talkView.gameObject.SetActive(false);
    }
    #endregion


    #region 任務
    public void setTask(taskItem taskItem)
    {
        this.taskItem = taskItem;
    }
    public void UpdateTaskUi()
    {
        if (taskItem != null && !exclamation.gameObject.activeSelf)
        {
            exclamation.gameObject.SetActive(true);
        }
        else if (taskItem == null && exclamation.gameObject.activeSelf)
        {
            exclamation.gameObject.SetActive(false);
        }
    }
    #endregion

    #region NPC互動
    public void openInteractive()
    {
        if (taskItem != null)
        {
            taskSystem.taskSystem_.addTask(taskItem);
            taskItem = null;
        }
    }
    #endregion
}

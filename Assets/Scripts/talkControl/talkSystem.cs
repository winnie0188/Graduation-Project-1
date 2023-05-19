using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class talkSystem : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("對話時顯示人名")]
    [SerializeField] Text peopleName;
    [Header("對話時會產生的文字")]
    [SerializeField] Text UItext;
    [Header("對話會呈現的圖片,0是玩家,1是npc")]
    [SerializeField] Image[] TalkIcon = new Image[2];


    [Header("存所有的大頭照")]
    public peopleIcon peopleIcon;

    List<TextDataFile> currentTextDataList;

    // 判斷是否結束
    bool TextFinsh;

    // 索引
    int index;

    bool cancelTyping;//取消打字

    // 打字速度
    float speed;


    public static talkSystem talkSystem_;
    private void Awake()
    {
        talkSystem_ = this;
    }


    public Sprite setPeopleIcon(string substring2)
    {
        return peopleIcon.Icon[peopleIcon.Icon_[substring2]];
    }

    public void openTalk(List<TextDataFile> currentTextDataList)
    {
        if (PanelManage.panelManage.panels.talkPanel.gameObject.activeSelf)
        {
            return;
        }
        PanelManage.panelManage.panels.talkPanel.gameObject.SetActive(true);

        if (currentTextDataList != null)
            this.currentTextDataList = currentTextDataList;

        index = 0;

        speed = 0.05f;

        StartCoroutine(TextIndex());
    }

    // 基礎按鈕功能-------------------------------------------------------------
    public void next()
    {
        if (currentTextDataList.Count <= index)
        {
            PanelManage.panelManage.panels.talkPanel.gameObject.SetActive(false);
            return;
        }

        if (TextFinsh && !cancelTyping)
        {
            StartCoroutine(TextIndex());
        }
        else if (!TextFinsh && !cancelTyping)
        {
            cancelTyping = true;
        }
    }

    // 增加打字速度
    public void addSpeed()
    {
        speed = 0.025f;
    }

    // 結束對話
    public void EndChat()
    {
        StopAllCoroutines();
        PanelManage.panelManage.panels.talkPanel.gameObject.SetActive(false);
    }

    // 回顧劇情
    public void feedbackChat()
    {
        EndChat();
        openTalk(null);
    }

    // -------------------------------------------------------------


    IEnumerator TextIndex()
    {
        TextFinsh = false;
        UItext.text = "";

        peopleName.text = "";


        for (int i = 0; i < 2; i++)
        {
            TalkIcon[i].sprite = currentTextDataList[index].PeopleIcon[i];
        }

        if (currentTextDataList[index].PeopleName == "")
        {
            whoTalk("2");

            peopleName.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            var Name = currentTextDataList[index].PeopleName;

            whoTalk(Name[Name.Length - 1] + "");

            peopleName.transform.parent.gameObject.SetActive(true);
            peopleName.text = Name.Substring(0, Name.Length - 1);
        }

        var sendMess = currentTextDataList[index].sendMess;

        for (int i = 0; i < sendMess.Count; i++)
        {
            if (sendMess[i] != "")
            {
                int slashIndex = sendMess[i].IndexOf('(');

                if (slashIndex != -1)
                {
                    SendMessage(sendMess[i].Substring(0, slashIndex), sendMess[i].Substring(slashIndex + 1, sendMess[i].Length - slashIndex - 2));
                }
                else
                {
                    SendMessage(sendMess[i]);
                }
            }
        }


        int letter = 0;
        var Text = currentTextDataList[index].Text;


        while (!cancelTyping && letter < Text.Length - 1)
        {
            yield return new WaitForSeconds(speed);
            UItext.text += Text[letter];
            letter++;

        }

        UItext.text = Text;


        index++;
        cancelTyping = false;
        TextFinsh = true;
    }


    // 誰在說話.........................................................
    public void whoTalk(string i)
    {
        int index = int.Parse(i);

        if (index == 2)
        {
            ChangeColor(TalkIcon[0], true);
            ChangeColor(TalkIcon[1], true);

            return;
        }
        else if (index == 0)
        {
            ChangeColor(TalkIcon[1], true);
        }
        else if (index == 1)
        {
            ChangeColor(TalkIcon[0], true);
        }



        ChangeColor(TalkIcon[index], false);
    }

    public void ChangeColor(Image a, bool isDark)
    {
        if (a.color.r == 0)
        {
            return;
        }

        var RGB = 1.0f;
        if (isDark)
        {
            RGB = 0.8f;
        }

        a.color = new Color(RGB, RGB, RGB, 1);
    }

    // 誰在說話.........................................................

    #region 事件
    // 圖片變黑
    public void Img_block(string i)
    {
        TalkIcon[int.Parse(i)].color = new Color(0, 0, 0, 1);
    }
    // 圖片辯白
    public void Img_white(string i)
    {
        TalkIcon[int.Parse(i)].color = new Color(1, 1, 1, 1);
    }
    // 抖動-----------------------------------------------------------

    public void StartJitter(string i)
    {
        StartCoroutine(JitterCoroutine(i));
    }
    private System.Collections.IEnumerator JitterCoroutine(string i)
    {
        int index = int.Parse(i);

        Vector3 originalPosition = TalkIcon[index].transform.localPosition;

        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            float x = Random.Range(-1f, 1f) * 10f;
            float y = Random.Range(-1f, 1f) * 10f;

            TalkIcon[index].transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        TalkIcon[index].transform.localPosition = originalPosition;
    }
    // 抖動-----------------------------------------------------------
    #endregion
}

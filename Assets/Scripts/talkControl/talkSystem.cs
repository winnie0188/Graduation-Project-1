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


    public static talkSystem talkSystem_;
    private void Awake()
    {
        talkSystem_ = this;
    }


    public Sprite setPeopleIcon(string substring2)
    {
        return peopleIcon.Icon[peopleIcon.Icon_[substring2]];
    }

    public void next()
    {
        if (currentTextDataList.Count <= index)
        {
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

    public void openTalk(List<TextDataFile> currentTextDataList)
    {
        if (PanelManage.panelManage.panels.talkPanel.gameObject.activeSelf)
        {
            return;
        }
        PanelManage.panelManage.panels.talkPanel.gameObject.SetActive(true);

        this.currentTextDataList = currentTextDataList;

        index = 0;

        StartCoroutine(TextIndex());
    }


    IEnumerator TextIndex()
    {
        TextFinsh = false;
        UItext.text = "";

        peopleName.text = "";
        peopleName.transform.parent.gameObject.SetActive(false);


        for (int i = 0; i < 2; i++)
        {
            TalkIcon[i].sprite = currentTextDataList[index].PeopleIcon[i];
        }

        if (currentTextDataList[index].PeopleName == "")
        {
            peopleName.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            peopleName.transform.parent.gameObject.SetActive(true);
            peopleName.text = currentTextDataList[index].PeopleName;
        }


        int letter = 0;
        var Text = currentTextDataList[index].Text;


        while (!cancelTyping && letter < Text.Length - 1)
        {
            yield return new WaitForSeconds(0.05f);
            UItext.text += Text[letter];
            letter++;

        }

        UItext.text = Text;


        index++;
        cancelTyping = false;
        TextFinsh = true;
    }
}

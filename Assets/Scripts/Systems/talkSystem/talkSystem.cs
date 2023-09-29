using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class talkSystem : MonoBehaviour
{
    // Start is called before the first frame update
    #region  people
    // 是否觸發對話
    public bool isToch;
    // 觸發物件
    public Transform TriggerObj;
    #endregion


    [Header("對話時顯示人名")]
    [SerializeField] Text peopleName;
    [Header("對話時會產生的文字")]
    [SerializeField] Text UItext;
    [Header("對話會呈現的圖片,0是玩家,1是npc")]
    [SerializeField] Image[] TalkIcon = new Image[2];


    [Header("存所有的大頭照")]
    public peopleIcon peopleIcon;

    talkContent currentTalkContent;

    // 判斷是否結束
    bool TextFinsh;

    // 索引
    int index;

    bool cancelTyping;//取消打字

    // 打字速度
    float speed;


    // 用來回復eventNum變數

    #region new
    [SerializeField] Transform selectTransform;
    [SerializeField] Transform nextPanel;

    #endregion


    public static talkSystem talkSystem_;
    private void Awake()
    {
        talkSystem_ = this;
    }

    //響應式頭像
    public void setImage(Sprite img, Image peopleIcon, float height)
    {
        // 原本寬度
        float originalWidth = img.rect.width;
        // 原本高度
        float originalHeight = img.rect.height;



        Sprite scaledSprite;

        if (height > 0.5f)
        {
            float scaleFactor = 925.86f * height / originalHeight;
            float scaledWidth = originalWidth * scaleFactor;

            // 创建新的Sprite，按比例缩放
            scaledSprite = Sprite.Create(img.texture, img.rect, new Vector2(0.5f, 0.5f), scaleFactor);

            peopleIcon.sprite = scaledSprite;
            peopleIcon.rectTransform.sizeDelta = new Vector2(scaledWidth, 925.86f * height);
            peopleIcon.rectTransform.anchoredPosition = new Vector3(0.0002441406f, 925.86f * height - 925.86f, 0);

        }
        else
        {
            // 計算
            float scaleFactor = 500f / originalWidth;
            float scaledHeight = originalHeight * scaleFactor;
            // 创建新的Sprite，按比例缩放
            scaledSprite = Sprite.Create(img.texture, img.rect, new Vector2(0.5f, 0.5f), scaleFactor);

            peopleIcon.sprite = scaledSprite;
            peopleIcon.rectTransform.sizeDelta = new Vector2(500f, scaledHeight);
            peopleIcon.rectTransform.anchoredPosition = new Vector3(0.0002441406f, 0, 0);
        }
    }
    public void print_(string a)
    {
        print(a);
    }

    public Sprite setPeopleIcon(string substring2)
    {
        return peopleIcon.Icon[peopleIcon.Icon_[substring2]];
    }

    public talkContent setTalkContent(string substring2)
    {
        return peopleIcon.TextFile[peopleIcon.TextFile_[substring2]];
    }

    public void openTalk(talkContent currentTalkContent)
    {
        if (PanelManage.panelManage.panels.talkPanel.gameObject.activeSelf)
        {
            return;
        }
        Img_white("0");
        Img_white("1");
        PanelManage.panelManage.panels.talkPanel.gameObject.SetActive(true);

        if (currentTalkContent != null)
            this.currentTalkContent = currentTalkContent;

        index = 0;

        speed = 0.05f;

        StartCoroutine(TextIndex());
    }

    // 基礎按鈕功能-------------------------------------------------------------
    public void next()
    {
        if (currentTalkContent.TextDataList.Count <= index)
        {
            PanelManage.panelManage.panels.talkPanel.gameObject.SetActive(false);


            closeSelect();
            finshTalk(currentTalkContent.finshTalk, currentTalkContent.finshTalk.openFinshUi);
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

    #region new
    void StopAll()
    {
        StopAllCoroutines();
        PanelManage.panelManage.panels.talkPanel.gameObject.SetActive(false);
        closeSelect();
    }
    #endregion

    // 增加打字速度
    public void addSpeed()
    {
        speed = 0.025f;
    }

    // 結束對話
    public void EndChat()
    {
        StopAll();
        finshTalk(currentTalkContent.finshTalk, currentTalkContent.finshTalk.openFinshUi);
    }

    // 回顧劇情
    public void feedbackChat()
    {
        StopAll();
        openTalk(null);
    }

    // -------------------------------------------------------------


    IEnumerator TextIndex()
    {
        List<TextDataFile> currentTextDataList = new List<TextDataFile>();
        currentTextDataList = currentTalkContent.TextDataList;
        TextFinsh = false;
        UItext.text = "";

        peopleName.text = "";

        #region 設定圖片
        float height = 1;
        for (int i = 0; i < 2; i++)
        {
            string imageName = Path.GetFileNameWithoutExtension(currentTextDataList[index].PeopleIcon[i].name);
            for (int j = 0; j < peopleIcon.PeopleHeights.Count; j++)
            {
                if (imageName.Contains(peopleIcon.PeopleHeights[j].name))
                {
                    height = peopleIcon.PeopleHeights[j].person;
                }
            }

            //設置圖
            setImage(currentTextDataList[index].PeopleIcon[i], TalkIcon[i], height);
            //TalkIcon[i].sprite = currentTextDataList[index].PeopleIcon[i];
            height = 1;
        }
        #endregion

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

        //有選項的
        if (index == currentTextDataList.Count - 1)
        {
            var branch = currentTextDataList[index].branch;

            for (int i = 0; i < branch.Count; i++)
            {

                if (i == 3)
                {
                    break;
                }

                var endIndex = branch[i];

                var selectItemBtn = selectTransform.GetChild(i).GetChild(1).GetComponent<Button>();
                selectItemBtn.onClick.RemoveAllListeners();
                selectItemBtn.onClick.AddListener(() => branchEvent(endIndex.Triggevent, endIndex.eventSelect));
                selectTransform.GetChild(i).gameObject.SetActive(true);

                selectTransform.GetChild(i).GetChild(0).GetComponent<Text>().text = endIndex.content;
                opneSelectPanel();

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
            RGB = 0.5f;
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

    #region 選項處理
    void opneSelectPanel()
    {
        selectTransform.gameObject.SetActive(true);
        nextPanel.gameObject.SetActive(false);
    }

    public void branchEvent(int Triggevent, talkContent talkContent)
    {
        closeSelect();
        switch (Triggevent)
        {
            case 0:
                // 繼續
                next();
                break;
            case 1:
                // 進入新的對話
                StopAll();
                openTalk(talkContent);
                break;
        }


    }

    public void closeSelect()
    {
        selectTransform.gameObject.SetActive(false);
        nextPanel.gameObject.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            selectTransform.GetChild(i).gameObject.SetActive(false);
        }
    }

    #endregion

    #region 對話結束
    public void finshTalk(FinshTalk finshTalk, bool openFinshUi)
    {
        if (openFinshUi)
        {
            List<Sprite> sprites = new List<Sprite>();

            for (int i = 0; i < finshTalk.bagItem.Length; i++)
            {
                sprites.Add(finshTalk.bagItem[i].BagItem_icon);
            }

            FinshUi.finshUi.openCanvas(sprites, finshTalk, null);
        }
        else
        {
            if (finshTalk.newTask != null)
            {
                taskSystem.taskSystem_.addTask(finshTalk.newTask);
            }
            if (finshTalk.bagItem.Length != 0)
            {
                for (int i = 0; i < finshTalk.bagItem.Length; i++)
                {
                    BagManage.bagManage.checkItem(
                        finshTalk.bagItem[i], 1, false, true
                    );
                }
            }
            if (finshTalk.NPCtask.taskItem != null)
            {
                Transform npc = NpcFactory.npcFactory.factorys[finshTalk.NPCtask.npcName];
                if (npc.TryGetComponent<NPC>(out var n))
                {
                    n.setTask(finshTalk.NPCtask.taskItem);
                }
            }
        }
    }
    #endregion
}

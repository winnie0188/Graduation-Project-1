using UnityEngine;
using UnityEngine.UI;

public class ESCsystem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform ExitPanel;
    [SerializeField] Transform SettingPanel;
    [SerializeField] Transform SavePanel;

    #region 好感度渲染
    [SerializeField] peopleTxt peopleTxt;
    [SerializeField] Transform loveValuePanel;
    [SerializeField] int peopleOutIndex;
    [SerializeField] int peopleindex;
    #endregion

    [Header("人物父物件")]
    [SerializeField] Transform peopleParent;

    public static ESCsystem ESCsystem_;

    private void Awake()
    {
        ESCsystem_ = this;
    }

    public void hiddeESCchild()
    {
        closeExitPanel();
        closeSettingPanel();
        closeloveValueSettingPanel();
        closeSavePanel();
    }

    public void openPanel(Transform panel)
    {
        hiddeESCchild();

        panel.gameObject.SetActive(true);
        peopleOutIndex = 0;
        peopleindex = 0;
        printloveValue();
        SaveMange.saveMange.showSave();
    }

    #region Exit

    public void Exit_(int i)
    {
        if (i == 0)
        {
            openPanel(SavePanel);
            SaveMange.saveMange.setIsSave(true);
        }
        else if (i == 1)
        {
            Application.Quit();
        }
        else if (i == 2)
        {
            closeExitPanel();
        }

    }
    public void closeExitPanel()
    {
        ExitPanel.gameObject.SetActive(false);
    }
    #endregion

    #region setting
    public void closeSettingPanel()
    {
        SettingPanel.gameObject.SetActive(false);
    }
    #endregion

    #region loveValue
    public void closeloveValueSettingPanel()
    {
        loveValuePanel.gameObject.SetActive(false);
    }

    public void printloveValue()
    {
        var people = peopleParent.GetChild(peopleOutIndex).GetChild(peopleindex).GetComponent<people>();
        peopleTxt.peopleName.text = people.basicPeople.peoplename;
        peopleTxt.birthday.text = people.basicPeople.birthday;
        peopleTxt.fragrance.text = people.basicPeople.fragrance;
        peopleTxt.personality.text = people.basicPeople.personality;
        peopleTxt.peopleLoveValue.text = people.peopleLoveValue + "";
        peopleTxt.favorite.text = people.basicPeople.favorite;
        peopleTxt.hate.text = people.basicPeople.hate;
    }

    public void pre()
    {
        if (peopleindex - 1 < 0)
        {
            if (peopleOutIndex - 1 < 0)
            {
                peopleOutIndex = peopleParent.childCount - 1;
            }
            else
            {
                peopleOutIndex = peopleOutIndex - 1;
            }
            peopleindex = peopleParent.GetChild(peopleOutIndex).childCount - 1;
        }
        else
        {
            peopleindex = peopleindex - 1;
        }

        printloveValue();
    }

    public void next()
    {
        if (peopleindex + 1 > peopleParent.GetChild(peopleOutIndex).childCount - 1)
        {
            if (peopleOutIndex + 1 > peopleParent.childCount - 1)
            {
                peopleOutIndex = 0;
            }
            else
            {
                peopleOutIndex = peopleOutIndex + 1;
            }
            peopleindex = 0;
        }
        else
        {
            peopleindex = peopleindex + 1;
        }

        printloveValue();
    }

    #endregion

    #region Save
    public void closeSavePanel()
    {
        SavePanel.gameObject.SetActive(false);
    }
    #endregion
}
[System.Serializable]
public class peopleTxt
{
    public Text peopleName;
    public Text birthday;
    public Text fragrance;
    public Text personality;
    public Text peopleLoveValue;
    public Text favorite;
    public Text hate;
}
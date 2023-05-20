using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveMange : MonoBehaviour
{
    [SerializeField] GameData gameData = new GameData();
    SaveData saveData = new SaveData();
    [SerializeField] Transform LoadParent;
    [Header("0:未存,1:有存")]
    [SerializeField] Sprite[] SaveImg;
    [SerializeField] Text pageTxt;

    public static bool load;
    public static int loadID;

    int page;

    [SerializeField] BagItemStore bagItemStore;

    [SerializeField] bool dontSelect;
    [Header("禁用圖片")]
    [SerializeField] Image dontSelectImg;

    public static SaveMange saveMange;

    private void Start()
    {
        saveMange = this;


        defalutLoadGame();


        // 幫按鈕加事件
        for (int i = 0; i < LoadParent.childCount; i++)
        {
            StartCoroutine(manualSaveAddListener(LoadParent.GetChild(i).GetChild(0).GetComponent<Button>(), i));
        }
    }

    public void Select()
    {
        if (dontSelect)
        {
            dontSelect = false;
            dontSelectImg.color = new Color(1, 1, 1, 0);
        }
        else
        {
            dontSelect = true;
            dontSelectImg.color = new Color(1, 1, 1, 1);
        }
    }

    public void prePage()
    {
        if (page - 1 < 0)
        {
            page = 4;
        }
        else
        {
            page -= 1;
        }
        showSave();
    }
    public void nextPage()
    {
        if (page + 1 > 4)
        {
            page = 0;
        }
        else
        {
            page += 1;
        }
        showSave();
    }

    // 顯示存檔
    public void showSave()
    {
        for (int i = 0; i < LoadParent.childCount; i++)
        {
            LoadParent.GetChild(i).GetComponent<Text>().text = saveData.DateTime[i + page * 6];
            if (saveData.SaveState[i + page * 6] != 0)
            {
                LoadParent.GetChild(i).GetChild(0).GetComponent<Image>().sprite = SaveImg[1];
            }
            else
            {
                LoadParent.GetChild(i).GetChild(0).GetComponent<Image>().sprite = SaveImg[0];
            }
        }

        pageTxt.text = page + 1 + "/5";
    }

    IEnumerator manualSaveAddListener(Button btn, int i)
    {
        btn.name = "" + i;
        btn.onClick.AddListener(() => manualSave(i));
        yield return null;
    }

    // 設定數據
    public void setGameData(int saveID)
    {
        // ---------------------------------存player資料---------------------------------------------------
        playerController playerController = playerController.playerController_;
        gameData.playerPos = playerController.transform.position;
        gameData.playerMaxHp = playerController.maxHp;
        gameData.playerReduceHp = playerController.Hp - gameData.playerMaxHp;
        gameData.playerMaxHungry = playerController.maxHungry;
        gameData.playerReduceHungry = playerController.Hungry - gameData.playerMaxHungry;
        gameData.playermaxBlackening = playerController.maxBlackening;
        gameData.playerReduceBlackening = playerController.Blackening - gameData.playermaxBlackening;
        // ---------------------------------存player資料---------------------------------------------------


        // ---------------------------------存背包資料---------------------------------------------------
        var sum = 0;
        for (int i = 0; i < BagManage.bagManage.bagSore.Length; i++)
        {
            sum += BagManage.bagManage.bagSore[i].BagItems.Count;
        }

        gameData.itemID = new int[sum];
        gameData.itemCount = new int[sum];
        gameData.iswear = new bool[sum];

        var Allcategory = BagManage.bagManage.slotContentParent.GetChild(BagManage.bagManage.slotContentParent.childCount - 1);
        for (int i = 0; i < sum; i++)
        {
            var IJindex = Allcategory.GetChild(i).GetChild(2).GetComponent<Text>().text.Split('/');

            int indexI = int.Parse(IJindex[0]);
            int indexJ = int.Parse(IJindex[1]);

            gameData.itemID[i] = BagManage.bagManage.bagSore[indexI].BagItems[indexJ].id;
            gameData.itemCount[i] = BagManage.bagManage.bagSore[indexI].ItemCount[indexJ];
            gameData.iswear[i] = BagManage.bagManage.bagSore[indexI].isWear[indexJ];
        }
        // ---------------------------------存背包資料---------------------------------------------------

        // ---------------------------------存快捷鍵資料---------------------------------------------------
        sum = 0;
        var bagManage = BagManage.bagManage;
        sum = bagManage.hotKeyStore.HotKeys.Length + bagManage.hotKeyStore.HotKeys_potion.Length + bagManage.hotKeyStore.HotKeys_Clothe.Length + bagManage.hotKeyStore.HotKeys_equip.Length;

        gameData.keyBag = new int[sum];

        gameData.keyItem = new int[sum];

        int index = 0;
        index = setKetData(bagManage.hotKeyStore.HotKeys, index);
        index = setKetData(bagManage.hotKeyStore.HotKeys_potion, index);
        index = setKetData(bagManage.hotKeyStore.HotKeys_Clothe, index);
        index = setKetData(bagManage.hotKeyStore.HotKeys_equip, index);

        int setKetData(HotKey[] hotKeys, int index)
        {
            for (int i = 0; i < hotKeys.Length; i++)
            {
                gameData.keyBag[index] = hotKeys[i].HotKey_Bag;
                gameData.keyItem[index] = hotKeys[i].HotKey_item;
                index++;
            }
            return index;
        }

        // ---------------------------------存快捷鍵資料---------------------------------------------------

        PlayerPrefs.SetString("GameData" + saveID, JsonUtility.ToJson(gameData));
        PlayerPrefs.SetString("SaveData", JsonUtility.ToJson(saveData));
        PlayerPrefs.Save();

        showSave();
    }

    public void GetGameData()
    {
        // ---------------------------------提取player資料---------------------------------------------------
        playerController playerController = playerController.playerController_;
        playerController.transform.position = gameData.playerPos;
        playerController.maxHp = gameData.playerMaxHp;
        playerController.HpUpdate(gameData.playerReduceHp);
        playerController.maxHungry = gameData.playerMaxHungry;
        playerController.HungryUpdate(gameData.playerReduceHungry);
        playerController.maxBlackening = gameData.playermaxBlackening;
        playerController.BlackeningUpdate(gameData.playerReduceBlackening);
        // ---------------------------------提取player資料---------------------------------------------------


        // ---------------------------------提取背包資料---------------------------------------------------
        for (int i = 0; i < gameData.itemCount.Length; i++)
        {
            BagManage.bagManage.checkItem(bagItemStore.BagItems[gameData.itemID[i]], gameData.itemCount[i], gameData.iswear[i]);
        }
        // ---------------------------------提取背包資料---------------------------------------------------


        // ---------------------------------提取快捷鍵資料---------------------------------------------------
        var hotKeyStore = BagManage.bagManage.hotKeyStore;
        int index = 0;
        index = getKetData(hotKeyStore.HotKeys, index);
        index = getKetData(hotKeyStore.HotKeys_potion, index);
        index = getKetData(hotKeyStore.HotKeys_Clothe, index);
        index = getKetData(hotKeyStore.HotKeys_equip, index);


        int getKetData(HotKey[] hotKeys, int index)
        {
            for (int i = 0; i < hotKeys.Length; i++)
            {
                hotKeys[i].HotKey_Bag = gameData.keyBag[index];
                hotKeys[i].HotKey_item = gameData.keyItem[index];
                index++;
            }
            return index;
        }
        BagManage.bagManage.Refresh_HotKey();

        // ---------------------------------提取快捷鍵資料---------------------------------------------------


    }


    #region 預設讀取檔案
    public void defalutLoadGame()
    {
        string SaveData = PlayerPrefs.GetString("SaveData");

        if (SaveData != "")
        {
            saveData = JsonUtility.FromJson<SaveData>(SaveData);
        }
        //--------------------------------------------------------------------

        if (load == true && saveData.SaveState[loadID] != 0)
        {
            gameData = JsonUtility.FromJson<GameData>(PlayerPrefs.GetString("GameData" + loadID));
            GetGameData();
            load = false;
        }
        else
        {
            load = false;
        }
    }
    #endregion

    public void loadGame(int i)
    {
        i += page * 6;
        if (saveData.SaveState[i] != 0)
        {
            loadID = i;
            load = true;
            SceneManager.LoadScene("myHw");
        }
    }


    #region 自動存檔
    public void autoSave()
    {
        if (dontSelect)
        {
            return;
        }

        for (int i = 0; i < saveData.SaveState.Length; i++)
        {
            if (saveData.SaveState[i] == 0 || saveData.SaveState[i] == 1)
            {
                saveData.SaveState[i] = 1;
                saveData.DateTime[i] = System.DateTime.Now.ToString();
                setGameData(i);
                break;
            }
        }
    }
    #endregion


    #region 手動存檔
    public void manualSave(int i)
    {
        i += page * 6;

        saveData.SaveState[i] = 2;
        saveData.DateTime[i] = System.DateTime.Now.ToString();
        setGameData(i);
    }
    #endregion

}
[System.Serializable]
public class SaveData
{
    public string[] DateTime = new string[30];
    // 未存檔，已自動，已手動
    public int[] SaveState = new int[30];
}

[System.Serializable]
public class GameData
{
    [Header("玩家屬性")]
    public Vector3 playerPos;
    public float playerMaxHp;
    public float playerReduceHp;
    public float playerMaxHungry;
    public float playerReduceHungry;
    public float playermaxBlackening;
    public float playerReduceBlackening;
    [Header("背包資料")]
    public int[] itemID;
    public int[] itemCount;
    public bool[] iswear;
    [Header("快捷鍵資料")]
    public int[] keyBag;
    public int[] keyItem;
}

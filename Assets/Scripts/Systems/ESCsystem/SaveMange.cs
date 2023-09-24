using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SaveMange : MonoBehaviour
{
    [Header("存檔數據")]
    [SerializeField] GameData gameData = new GameData();
    SaveData saveData = new SaveData();
    [SerializeField] Transform LoadParent;

    [Header("0:未存,1:有存")]
    [SerializeField] Sprite[] SaveImg;
    [SerializeField] Text pageTxt;

    public static bool load;
    public static int loadID;


    [Header("當前頁和單位格")]

    int page;
    int slotID;


    [SerializeField] bool dontSelect;
    [Header("禁用圖片")]
    [SerializeField] Image dontSelectImg;

    public static SaveMange saveMange;

    [Header("是否存檔")]
    public bool isSave;
    public Transform checkSavePanel;


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



    #region 禁用管理

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
    #endregion

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
        gameData.playerSave.playerPos = playerController.transform.position;
        gameData.playerSave.playerMaxHp = playerController.maxHp;
        gameData.playerSave.playerReduceHp = playerController.Hp - gameData.playerSave.playerMaxHp;
        gameData.playerSave.playerMaxHungry = playerController.maxHungry;
        gameData.playerSave.playerReduceHungry = playerController.Hungry - gameData.playerSave.playerMaxHungry;
        gameData.playerSave.playermaxBlackening = playerController.maxBlackening;
        gameData.playerSave.playerReduceBlackening = playerController.Blackening - gameData.playerSave.playermaxBlackening;
        // ---------------------------------存player資料---------------------------------------------------


        // ---------------------------------存背包資料---------------------------------------------------
        var sum = 0;
        for (int i = 0; i < BagManage.bagManage.bagSore.Length; i++)
        {
            sum += BagManage.bagManage.bagSore[i].BagItems.Count;
        }

        gameData.bagSave.itemID = new int[sum];
        gameData.bagSave.itemCount = new int[sum];
        gameData.bagSave.iswear = new bool[sum];

        var Allcategory = BagManage.bagManage.slotContentParent.GetChild(BagManage.bagManage.slotContentParent.childCount - 1);
        for (int i = 0; i < sum; i++)
        {
            var IJindex = Allcategory.GetChild(i).GetChild(2).GetComponent<Text>().text.Split('/');

            int indexI = int.Parse(IJindex[0]);
            int indexJ = int.Parse(IJindex[1]);

            gameData.bagSave.itemID[i] = BagManage.bagManage.bagSore[indexI].BagItems[indexJ].id;
            gameData.bagSave.itemCount[i] = BagManage.bagManage.bagSore[indexI].ItemCount[indexJ];
            gameData.bagSave.iswear[i] = BagManage.bagManage.bagSore[indexI].isWear[indexJ];
        }
        // ---------------------------------存背包資料---------------------------------------------------

        // ---------------------------------存快捷鍵資料---------------------------------------------------
        sum = 0;
        var bagManage = BagManage.bagManage;
        sum = bagManage.hotKeyStore.HotKeys.Length + bagManage.hotKeyStore.HotKeys_potion.Length + bagManage.hotKeyStore.HotKeys_Clothe.Length + bagManage.hotKeyStore.HotKeys_equip.Length;

        gameData.hotKeySave.keyBag = new int[sum];

        gameData.hotKeySave.keyItem = new int[sum];

        int index = 0;
        index = setKetData(bagManage.hotKeyStore.HotKeys, index);
        index = setKetData(bagManage.hotKeyStore.HotKeys_potion, index);
        index = setKetData(bagManage.hotKeyStore.HotKeys_Clothe, index);
        index = setKetData(bagManage.hotKeyStore.HotKeys_equip, index);

        int setKetData(HotKey[] hotKeys, int index)
        {
            for (int i = 0; i < hotKeys.Length; i++)
            {
                gameData.hotKeySave.keyBag[index] = hotKeys[i].HotKey_Bag;
                gameData.hotKeySave.keyItem[index] = hotKeys[i].HotKey_item;
                index++;
            }
            return index;
        }

        // ---------------------------------存快捷鍵資料---------------------------------------------------

        //----------------------------------存建築資料-----------------------------------------------------
        List<BuildSave> buildSaveList = new List<BuildSave>();


        foreach (var blockPair in BuildSystem.buildSystem.GetBuildData())
        {
            BuildSave buildSave = new BuildSave
            {
                block = blockPair.Key.position,
                blockRotate = blockPair.Key.rotation.eulerAngles.y,
                blockID = blockPair.Value
            };

            buildSaveList.Add(buildSave);
        }

        gameData.buildSaves = buildSaveList.ToArray();

        //----------------------------------存建築資料-----------------------------------------------------

        PlayerPrefs.SetString("GameData" + saveID, JsonUtility.ToJson(gameData));
        PlayerPrefs.SetString("SaveData", JsonUtility.ToJson(saveData));
        PlayerPrefs.Save();


        showSave();
    }

    void GetGameData()
    {
        // ---------------------------------提取player資料---------------------------------------------------
        playerController playerController = playerController.playerController_;
        playerController.transform.position = gameData.playerSave.playerPos;
        playerController.maxHp = gameData.playerSave.playerMaxHp;
        playerController.HpUpdate(gameData.playerSave.playerReduceHp);
        playerController.maxHungry = gameData.playerSave.playerMaxHungry;
        playerController.HungryUpdate(gameData.playerSave.playerReduceHungry);
        playerController.maxBlackening = gameData.playerSave.playermaxBlackening;
        playerController.BlackeningUpdate(gameData.playerSave.playerReduceBlackening);
        // ---------------------------------提取player資料---------------------------------------------------

        if (PanelManage.panelManage.getIsCreateMode() == false)
        {
            // ---------------------------------提取背包資料---------------------------------------------------

            for (int i = 0; i < gameData.bagSave.itemCount.Length; i++)
            {
                BagManage.bagManage.checkItem(
                    StoreSetting.storeSetting.GetBagItemStore().BagItems[gameData.bagSave.itemID[i]],
                     gameData.bagSave.itemCount[i],
                      gameData.bagSave.iswear[i],
                      false
                );
            }
            BagManage.bagManage.Update_AllBag();



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
                    hotKeys[i].HotKey_Bag = gameData.hotKeySave.keyBag[index];
                    hotKeys[i].HotKey_item = gameData.hotKeySave.keyItem[index];
                    index++;
                }
                return index;
            }
            BagManage.bagManage.Refresh_HotKey();
        }

        // ---------------------------------提取快捷鍵資料---------------------------------------------------

        //---------------------------------提取建築資料---------------------------------------------------

        for (int i = 0; i < gameData.buildSaves.Length; i++)
        {
            StoreSetting.storeSetting.GetBagItemStore().
            BagItems[gameData.buildSaves[i].blockID].
            GetBlock().
            createNewBlock(gameData.buildSaves[i].block, false, gameData.buildSaves[i].blockRotate);
        }
        //---------------------------------提取建築資料---------------------------------------------------

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
            FindObjectOfType<Gameprocess>().init();
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


    // #region 自動存檔
    // public void autoSave()
    // {
    //     if (dontSelect)
    //     {
    //         return;
    //     }

    //     for (int i = 0; i < saveData.SaveState.Length; i++)
    //     {
    //         if (saveData.SaveState[i] == 0 || saveData.SaveState[i] == 1)
    //         {
    //             saveData.SaveState[i] = 1;
    //             saveData.DateTime[i] = System.DateTime.Now.ToString();
    //             setGameData(i);
    //             break;
    //         }
    //     }
    // }
    // #endregion


    #region 手動存檔
    public void manualSave(int i)
    {
        i += page * 6;
        slotID = i;

        if (isSave)
        {
            checkSavePanel.gameObject.SetActive(true);
        }
        else
        {
            loadGame(slotID);
        }
    }

    public void YesSave()
    {
        saveData.SaveState[slotID] = 2;
        saveData.DateTime[slotID] = System.DateTime.Now.ToString();
        setGameData(slotID);
        checkSavePanel.gameObject.SetActive(false);
    }
    public void NoSave()
    {
        checkSavePanel.gameObject.SetActive(false);
    }

    #endregion

    public void setIsSave(bool isSave)
    {
        this.isSave = isSave;
        if (isSave)
        {
            dontSelectImg.gameObject.SetActive(true);
        }
        else
        {
            dontSelectImg.gameObject.SetActive(false);
        }
    }

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
    public PlayerSave playerSave;
    [Header("背包資料")]
    public BagSave bagSave;

    [Header("快捷鍵資料")]
    public HotKeySave hotKeySave;

    [Header("方塊資料")]
    public BuildSave[] buildSaves;
}

[System.Serializable]
public class PlayerSave
{
    //玩家位置
    public Vector3 playerPos;
    //玩家最大血量
    public float playerMaxHp;
    //玩家減少血量
    public float playerReduceHp;
    //玩家最大飢餓
    public float playerMaxHungry;
    //玩家減少飢餓
    public float playerReduceHungry;
    //玩家最大黑化
    public float playermaxBlackening;
    //玩家減少黑化
    public float playerReduceBlackening;
}

[System.Serializable]
public class BagSave
{
    //物件id
    public int[] itemID;
    //物件數量
    public int[] itemCount;
    //是否裝備
    public bool[] iswear;
}

[System.Serializable]
public class HotKeySave
{
    //對應背包
    public int[] keyBag;
    //對應物件
    public int[] keyItem;
}


[System.Serializable]
public class BuildSave
{
    //方塊位置
    public Vector3 block;
    //對應方塊id
    public int blockID;

    public float blockRotate;
}
using UnityEngine;
using QFSW.MOP2;
using UnityEngine.EventSystems;

[System.Serializable]
public class block : BagItemObj
{
    [Header("對應物件池")]
    [SerializeField] ObjectPool blockPool;

    GameObject currentGameObject = null;


    // 道具id
    int id;


    #region 封裝屬性
    // Id
    public int Id
    {
        get => id;
        set => id = value;
    }
    // 當前綠色物件
    public GameObject CurrentGameObject
    {
        get => currentGameObject;
        set => currentGameObject = value;
    }
    public ObjectPool BlockPool
    {
        get => blockPool;
        set => blockPool = value;
    }
    #endregion


    #region 主function

    public override void Create()
    {
        BlockPool.Initialize();

        InitializeWithObject();
    }

    public override void UseIng()
    {
        // 如果在ui上則返回
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        postBlock();
        blockMove();
    }

    public override void Relese()
    {
        if (CurrentGameObject != null)
        {
            BlockPool.Release(CurrentGameObject);
        }
    }

    #endregion

    #region 一般function

    // 判斷能不能放方塊
    void postBlock()
    {
        //如果按左鍵
        if (Input.GetKeyDown(playerController.playerController_.playerKeyCodes.UseProps))
        {
            //如果滑鼠有碰到block
            Vector3 MousePos = BuildSystem.buildSystem.GetMouseWoldPosition(-1);
            if (MousePos != new Vector3(0, -1000, 0))
            {
                createNewBlock(CurrentGameObject.transform.position, true, CurrentGameObject.transform.rotation.eulerAngles.y);
            }
            else
            {
                //........放置失敗
            }
        }
        else if (Input.GetKeyDown(playerController.playerController_.playerKeyCodes.RotateBlock))
        {
            BuildSystem.buildSystem.rotate(CurrentGameObject.transform);
        }
    }

    //新增新的方塊
    public void createNewBlock(Vector3 Pos, bool isDelete, float Rotate)
    {
        GameObject Block = BlockPool.GetObject();
        Block.transform.position = Pos;
        Block.transform.SetParent(BuildSystem.buildSystem.GetObjectParent(Block.transform.position));
        Block.transform.rotation = Quaternion.Euler(
            Block.transform.rotation.eulerAngles.x,
             Rotate,
            Block.transform.rotation.eulerAngles.z
        );

        int arm = playerController.playerController_.GetArm();
        int Bag = BagManage.bagManage.hotKeyStore.HotKeys[arm].HotKey_Bag;
        int item = BagManage.bagManage.hotKeyStore.HotKeys[arm].HotKey_item;

        if (Block.transform.GetChild(0).gameObject.activeSelf == false)
        {
            InitializeWithBlock(Block.transform);
        }

        if (isDelete)
        {
            // 減1道具量
            BagManage.bagManage.deleteItemInBag(
                BagManage.bagManage.bagSore[Bag],
                BagManage.bagManage.bagSore[Bag].BagItems[item],
                -1,
                true
            );
        }
        BuildSystem.buildSystem.setBuildData(Block.transform, Id);
    }

    public void destoryOldBlock(GameObject gameObject, BagItem bagItem)
    {
        BuildSystem.buildSystem.GetBuildData().Remove(gameObject.transform);
        BlockPool.Release(gameObject);

        if (bagItem != null)
        {
            // +1道具量
            BagManage.bagManage.checkItem(
                bagItem,
                1,
                false,
                true
            );
        }
    }


    // 初始化手持顯示的綠色可視物
    void InitializeWithObject()
    {
        CurrentGameObject = BlockPool.GetObject();
        BuildSystem.buildSystem.combine(CurrentGameObject.transform);
        CurrentGameObject.transform.rotation = Quaternion.Euler(
            CurrentGameObject.transform.rotation.x,
             0,
              CurrentGameObject.transform.rotation.z
            );
    }
    //透明方塊移動
    void blockMove()
    {
        if (Input.GetKey(playerController.playerController_.playerKeyCodes.AltBlock))
        {
            CurrentGameObject.transform.position = BuildSystem.buildSystem.GetMouseWoldPosition(0);
        }
        else if (Input.GetKey(playerController.playerController_.playerKeyCodes.ShiftBlock))
        {
            CurrentGameObject.transform.position = BuildSystem.buildSystem.GetMouseWoldPosition(1);
        }
        else if (Input.GetKey(playerController.playerController_.playerKeyCodes.CtrlBlock))
        {
            CurrentGameObject.transform.position = BuildSystem.buildSystem.GetMouseWoldPosition(2);
        }
        else
        {
            Vector3 position = BuildSystem.buildSystem.GetMouseWoldPosition(-1);
            CurrentGameObject.transform.position = BuildSystem.buildSystem.SnapCoordinateToGrid(position);
        }


    }

    //復原方塊
    void InitializeWithBlock(Transform gameObject)
    {
        BuildSystem.buildSystem.dismantle(gameObject);
    }

    #endregion
}

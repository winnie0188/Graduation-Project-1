using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class playerController : MonoBehaviour
{
    #region Player Basic
    playerDirection playerDirection = playerDirection.LEFT;
    [Header("會變動的屬性")]
    // 血量
    public float maxHp;//最大血量
    public float Hp;
    [SerializeField] Text HpTxt;
    [SerializeField] Transform HpLine;
    // 
    // 飢餓
    public float maxHungry;//最大飢餓
    public float Hungry;
    [SerializeField] Text HungryTxt;
    [SerializeField] Transform HungryLine;
    // 

    // 黑化
    public float maxBlackening;//最大黑化
    public float Blackening;
    [SerializeField] Transform BlackeningLine;
    // 

    //按鍵設定
    [Header("玩家按鍵")]
    public PlayerKeyCode playerKeyCodes;//玩家按鍵

    [Header("移動速度")]
    [SerializeField] float speed;//移動速度
    float speedBuff = 1;
    //如果暈眩歸0
    float speedDizziness = 1;
    float[] moveItem = { 0, 0 };//移動數值
    bool isWalk;

    [Header("跳躍")]
    bool isJump;//是否跳躍
    public Rigidbody rigi;
    public Collider collider;
    [SerializeField] float Jumpspeed;//跳躍速度
    bool isKnock = false;
    //是否懸空
    bool ishits;
    //是否播放動畫
    playerJumpState jumpState = playerJumpState.NONE;
    [SerializeField] Animator ani;
    float jumpTime = 0.0f;

    string aniStr;

    #endregion

    #region 是否坐著
    public playerSitState sitState = playerSitState.NONE;
    public Vector3 sitPos;
    #endregion

    #region RunVar
    //奔跑處理
    private float lastClickTime;//最後點擊時間
    private float doubleTapTimeThreshold = 0.3f;//0.3秒內按兩次則奔跑
    [SerializeField] bool isRunning;//是否奔跑
    KeyCode tempCode;//判斷按的按鍵與上一次按的是否相同

    #endregion

    #region  Camera旋轉

    [SerializeField] MouseLook m_MouseLook;
    public Transform m_Camera;
    bool isLock = false;

    // 是否可以旋轉
    bool isCanRotate = true;

    [SerializeField] Transform airCamera;

    #endregion


    #region  手持
    // 快捷鍵幾號
    [SerializeField] int arm = 0;
    // 對應背包
    int Bag;
    // 對應物件
    int item;
    // 是否使用中
    bool canUse;
    //要釋放背包道具
    BagItem bagItem;
    BagItemObj emptyItem = new BagItemObj();

    #endregion


    //

    public static playerController playerController_;//唯一性

    private void Awake()
    {
        playerController_ = this;
        rigi = GetComponent<Rigidbody>();
        initKecode();

        m_MouseLook.Init(transform, m_Camera.transform);


        StartCoroutine(JumpState());
        // 執行arm相關的update
        StartCoroutine(armUpdate());


    }

    void initKecode()
    {
        KeyCode[] keys = keyCodes();
        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i] == KeyCode.None)
            {
                switch (i)
                {
                    case 0:
                        playerKeyCodes.leftMove = KeyCode.A;
                        break;
                    case 1:
                        playerKeyCodes.rightMove = KeyCode.D;
                        break;
                    case 2:
                        playerKeyCodes.frontMove = KeyCode.W;
                        break;
                    case 3:
                        playerKeyCodes.BackMove = KeyCode.S;
                        break;
                    case 4:
                        playerKeyCodes.Jump = KeyCode.K;
                        break;
                }
            }
        }
    }

    public KeyCode[] keyCodes()
    {
        KeyCode[] keys = { playerKeyCodes.leftMove, playerKeyCodes.rightMove, playerKeyCodes.frontMove, playerKeyCodes.BackMove, playerKeyCodes.Jump };
        return keys;
    }

    private void FixedUpdate()
    {
        Vector3 playerPos = transform.position;
        airCamera.transform.position = new Vector3(playerPos.x, 100, playerPos.z);

        if (PanelManage.panelManage.includeAllState() || isKnock)
        {
            rigi.velocity = new Vector3(0, rigi.velocity.y, 0);
            return;
        }


        //如果坐著，不執行
        if (sitState != playerSitState.NONE)
        {
            this.transform.position = sitPos;
            return;
        }


        Vector3 playerEye = this.transform.GetChild(0).position;
        bool tempIshits = false;



        if (Physics.RaycastAll(playerEye, Vector3.down, 2f, LayerMask.GetMask("block")).Length > 0)
        {
            tempIshits = true;
        }

        if (isJump)
        {
            //跳躍時
            if (tempIshits && speedDizziness == 1)
            {
                rigi.AddForce(Vector3.up * Jumpspeed, ForceMode.Impulse);
                jumpState = playerJumpState.JUMPUP;
            }

            isJump = false;
        }

        float maxJumpSpeed = 0;

        if (jumpState != playerJumpState.JUMPUP)
        {
            if (tempIshits != ishits)
            {
                ishits = tempIshits;
                if (ishits)
                {
                    jumpState = playerJumpState.JUMPDOWN;
                    StartCoroutine(jumpDown());
                }
            }

            if (ishits && jumpState == playerJumpState.JUMPING)
            {
                jumpState = playerJumpState.NONE;
            }
        }
        else
        {
            maxJumpSpeed = 6;
            if ((jumpTime += Time.deltaTime) >= 0.7f)
            {
                jumpTime = 0;
                jumpState = playerJumpState.JUMPING;
            }
        }

        Vector3 localMovement = new Vector3(moveItem[0], 0, moveItem[1]);
        Vector3 worldMovement = transform.TransformDirection(localMovement);
        rigi.velocity = new Vector3(worldMovement.x * speed * (isRunning ? 2f : 1.0f) * speedBuff * speedDizziness, rigi.velocity.y > maxJumpSpeed + 0.1f ? maxJumpSpeed : rigi.velocity.y, worldMovement.z * speed * (isRunning ? 2f : 1.0f) * speedBuff * speedDizziness);
    }

    private void Update()
    {
        updateAni();
        CameraLook();
        Move();
    }

    #region 更新動畫
    IEnumerator jumpDown()
    {
        yield return new WaitForSeconds(0.7f);
        if (jumpState == playerJumpState.JUMPDOWN)
        {
            jumpState = playerJumpState.NONE;
        }
    }

    void updateAni()
    {
        string tempStr = playerAniStr.aniArray[0];
        if (!PanelManage.panelManage.includeAllState())
        {
            if (sitState == playerSitState.SITUP)
            {
                tempStr = playerAniStr.aniArray[1];
            }
            else if (sitState == playerSitState.SITDOWN)
            {
                tempStr = playerAniStr.aniArray[2];
                if (playerDirection == playerDirection.LEFT)
                {
                    ani.transform.parent.localPosition = new Vector3(-0.4f, 0, 0);
                }
                else
                {
                    ani.transform.parent.localPosition = new Vector3(0.4f, 0, 0);
                }
            }
            else
            {
                if (isRunning || isWalk)
                {
                    if (isRunning)
                    {
                        tempStr = playerAniStr.aniArray[3];
                    }
                    else if (isWalk)
                    {
                        tempStr = playerAniStr.aniArray[4];
                    }
                }
                else if (jumpState == playerJumpState.JUMPUP)
                {
                    tempStr = playerAniStr.aniArray[5];
                }
                else if (jumpState == playerJumpState.JUMPDOWN)
                {
                    tempStr = playerAniStr.aniArray[6];
                }
                else if (jumpState == playerJumpState.JUMPING)
                {
                    tempStr = playerAniStr.aniArray[7];
                }

            }
        }


        if (aniStr != tempStr)
        {
            aniStr = tempStr;
            ani.SetTrigger(aniStr);
        }
    }

    #endregion

    #region 基礎數值變動

    public void setSpeedBuff(float buff)
    {
        speedBuff += buff;
    }

    public void setSpeedDizziness(float dizziness)
    {
        speedDizziness = dizziness;
    }

    public void setKnock(bool isKnock)
    {
        this.isKnock = isKnock;
    }

    //更新血量 
    public void HpUpdate(float hp)
    {
        if (Hp + hp > maxHp)
        {
            Hp = maxHp;
        }
        else if (Hp + hp < 0)
        {
            Hp = 0;
        }
        else
        {
            Hp += hp;
        }


        float HpPerson = Hp / maxHp;

        HpTxt.text = (int)(HpPerson * 100) + "%";
        HpLine.localScale = new Vector3(1 - HpPerson, 1, 1);
    }

    //更新飢餓
    public void HungryUpdate(float hungry)
    {
        if (Hungry + hungry > maxHungry || Hungry + hungry < 0)
        {
            return;
        }

        Hungry += hungry;

        float HungryPerson = Hungry / maxHungry;

        HungryTxt.text = (HungryPerson * 100) + "%";
        HungryLine.localScale = new Vector3(1 - HungryPerson, 1, 1);
    }

    //更新飢餓
    public void BlackeningUpdate(float blackening)
    {
        if (Blackening + blackening > maxBlackening || Blackening + blackening < 0)
        {
            return;
        }

        Blackening += blackening;

        float BlackeningPerson = Blackening / maxBlackening;

        BlackeningLine.localScale = new Vector3(1 - BlackeningPerson, 1, 1);
    }

    #endregion

    public void setCanRotateCamera(bool iscan)
    {
        isCanRotate = iscan;
    }

    // 視角轉動
    void CameraLook()
    {
        m_MouseLook.InternalLockUpdate();

        if (isCanRotate)
        {
            if (Input.GetKeyDown(playerKeyCodes.RotateCamera))
            {
                isLock = !isLock;

                if (isLock)
                {
                    // merchantShop.merchantShop_.Rotation_merchant();
                }
                else
                {
                    // merchantShop.merchantShop_.SetMisRotation(false);
                }
            }
        }
        else
        {
            isLock = false;
            // merchantShop.merchantShop_.SetMisRotation(false);
        }


        if (isLock)
        {
            m_MouseLook.openLock();
            m_MouseLook.LookRotation(transform, m_Camera.transform);
        }
        else
        {
            m_MouseLook.closeLock();
        }
    }

    public void taskRotate(float y)
    {
        transform.rotation = Quaternion.Euler(0, y, 0);
        m_MouseLook.setPos(transform, m_Camera.transform);
    }

    IEnumerator JumpState()
    {
        yield return null;
        if (Input.GetKeyDown(playerKeyCodes.Jump))
        {
            isJump = true;
        }

        StartCoroutine(JumpState());
    }

    // 移動
    void Move()
    {
        bool isRunx = true;
        bool isRunY = true;

        if (Input.GetKey(playerKeyCodes.leftMove))
        {
            moveItem[0] = -1;
            if (sitState == playerSitState.NONE)
            {
                playerDirection = playerDirection.LEFT;
                ani.transform.parent.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        else if (Input.GetKey(playerKeyCodes.rightMove))
        {
            moveItem[0] = 1;

            if (sitState == playerSitState.NONE)
            {
                playerDirection = playerDirection.RIGHT;
                ani.transform.parent.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }
        }
        else
        {
            moveItem[0] = 0;

            isRunx = false;
        }

        if (Input.GetKey(playerKeyCodes.frontMove))
        {
            moveItem[1] = 1;
        }
        else if (Input.GetKey(playerKeyCodes.BackMove))
        {
            moveItem[1] = -1;
        }
        else
        {
            moveItem[1] = 0;

            isRunY = false;
        }

        Run(isWalk = isRunx || isRunY);

        if (isRunx && isRunY)
        {
            moveItem[0] /= 2;
            moveItem[1] /= 2;
        }
    }
    // 奔跑
    void Run(bool isWalk)
    {
        bool isDownTouch = true;
        KeyCode nowKecode = KeyCode.None;

        if (Input.GetKeyDown(playerKeyCodes.leftMove))
        {
            nowKecode = playerKeyCodes.leftMove;
        }
        else if (Input.GetKeyDown(playerKeyCodes.rightMove))
        {
            nowKecode = playerKeyCodes.rightMove;
        }
        else if (Input.GetKeyDown(playerKeyCodes.frontMove))
        {
            nowKecode = playerKeyCodes.frontMove;
        }
        else if (Input.GetKeyDown(playerKeyCodes.BackMove))
        {
            nowKecode = playerKeyCodes.BackMove;
        }
        else
        {
            isDownTouch = false;
        }


        if (isRunning)
        {
            if (!isWalk)
            {
                isRunning = false;
            }
        }
        else if (isDownTouch)
        {
            if (Time.time - lastClickTime < doubleTapTimeThreshold && nowKecode == tempCode)
            {
                isRunning = true;
            }
            else
            {
                isRunning = false;
            }
            lastClickTime = Time.time;

            tempCode = nowKecode;
        }


    }


    #region 手持道具Update

    // Update
    IEnumerator armUpdate()
    {
        switchArm();

        if (canUse)
        {
            try
            {
                BagManage.bagManage.bagSore[Bag].BagItems[item].UseIng();
            }
            catch (System.Exception e)
            {
                print(e);
            }
        }
        else
        {

            emptyItem.UseIng();

        }

        yield return null;

        StartCoroutine(armUpdate());
    }




    // 切換手持
    public void switchArm()
    {
        if (Input.GetKeyDown(playerKeyCodes.Num[0]))
        {
            SetBag_Item(0);
        }
        else if (Input.GetKeyDown(playerKeyCodes.Num[1]))
        {
            SetBag_Item(1);
        }
        else if (Input.GetKeyDown(playerKeyCodes.Num[2]))
        {
            SetBag_Item(2);
        }
        else if (Input.GetKeyDown(playerKeyCodes.Num[3]))
        {
            SetBag_Item(3);
        }
        else if (Input.GetKeyDown(playerKeyCodes.Num[4]))
        {
            SetBag_Item(4);
        }
        else if (Input.GetKeyDown(playerKeyCodes.Num[5]))
        {
            SetBag_Item(5);
        }
        else if (Input.GetKeyDown(playerKeyCodes.Num[6]))
        {
            SetBag_Item(6);
        }
        else if (Input.GetKeyDown(playerKeyCodes.Num[7]))
        {
            SetBag_Item(7);
        }
        else
        {
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheel != 0f)
            {
                // 定义滚轮切换的速度
                float scrollSpeed = 10f;

                // 计算新的道具索引
                int newPropIndex = arm - (int)(scrollWheel * scrollSpeed);

                // 使用取余运算确保道具索引在 0 到 totalProps - 1 之间循环
                SetBag_Item((newPropIndex + 8) % 8);
            }
        }
    }

    public void SetBag_Item(int arm)
    {
        // 獲得當前快捷鍵
        Transform BasicPanel = PanelManage.panelManage.panels.HotKeyPanel.GetChild(1);

        // 預設大小
        float defaultScale = 6.341853f;
        //新大小
        float newScale = 7f;

        if (arm != this.arm)
        {
            // ====動畫=====
            // 放大當前快捷鍵
            BasicPanel.GetChild(arm).DOScale(new Vector3(newScale, newScale, newScale), 0.25f);
            //縮小舊的快捷鍵
            BasicPanel.GetChild(this.arm).DOScale(new Vector3(defaultScale, defaultScale, defaultScale), 0.25f);
            //==============
        }


        //全域arm等於區域arm
        this.arm = arm;


        // 釋放物件觸發事件
        BagItem relase = bagRelase();

        // 如果當前裝備非空
        if (BagManage.bagManage.hotKeyStore.HotKeys[arm].HotKey_item != -1)
        {
            // 道具使用中
            canUse = true;

            //如果切換的道具和舊的一樣，就不用初始化了
            if (relase == bagItem)
            {
                return;
            }

            // 如果不一樣，就設成relase
            bagItem = relase;

            // 初始化
            try
            {
                BagManage.bagManage.bagSore[Bag].BagItems[item].Create();
            }
            catch (System.Exception)
            {

                print("失敗");
            }

        }
        // 如果當前裝備為空
        else
        {
            // 當前手持為空並且不使用道具
            bagItem = null;
            canUse = false;
        }

    }

    BagItem bagRelase()
    {
        Bag = BagManage.bagManage.hotKeyStore.HotKeys[arm].HotKey_Bag;
        item = BagManage.bagManage.hotKeyStore.HotKeys[arm].HotKey_item;

        if (Bag != -1 && item != -1)
        {
            // 如果是同物件就別釋放了
            if (bagItem == BagManage.bagManage.bagSore[Bag].BagItems[item])
            {
                return BagManage.bagManage.bagSore[Bag].BagItems[item];
            }
            else
            {
                // 釋放
                if (bagItem != null)
                {
                    bagItem.Release();
                }

                return BagManage.bagManage.bagSore[Bag].BagItems[item];
            }
        }
        else
        {
            // 釋放
            if (bagItem != null)
            {
                bagItem.Release();
            }

            return null;
        }

    }

    // 獲取手持值
    public int GetArm()
    {
        return arm;
    }
    #endregion
}



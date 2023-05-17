
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    #region Player Basic
    [Header("會變動的屬性")]
    // 血量
    [SerializeField] float maxHp;//最大血量
    [SerializeField] float Hp;
    [SerializeField] Text HpTxt;
    [SerializeField] Transform HpLine;
    // 
    // 飢餓
    [SerializeField] float maxHungry;//最大飢餓
    [SerializeField] float Hungry;
    [SerializeField] Text HungryTxt;
    [SerializeField] Transform HungryLine;
    // 

    // 黑化
    [SerializeField] float maxBlackening;//最大黑化
    [SerializeField] float Blackening;
    [SerializeField] Transform BlackeningLine;
    // 

    //按鍵設定
    [Header("玩家按鍵")]
    public PlayerKeyCode playerKeyCodes;//玩家按鍵

    [Header("移動速度")]
    [SerializeField] float speed;//移動速度
    float[] moveItem = { 0, 0 };//移動數值

    [Header("跳躍")]
    bool isJump;//是否跳躍
    Rigidbody rigi;
    [SerializeField] float Jumpspeed;//跳躍速度

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

    #endregion


    //

    public static playerController playerController_;//唯一性


    private void Awake()
    {
        playerController_ = this;
        rigi = GetComponent<Rigidbody>();
        initKecode();

        m_MouseLook.Init(transform, m_Camera.transform);
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
        if (isJump)
        {
            if (rigi.velocity.y == 0)
            {
                rigi.AddForce(Vector3.up * Jumpspeed, ForceMode.Impulse);
            }
            isJump = false;
        }

        Vector3 localMovement = new Vector3(moveItem[0], 0, moveItem[1]);
        Vector3 worldMovement = transform.TransformDirection(localMovement);
        rigi.MovePosition(rigi.position + worldMovement * speed * Time.fixedDeltaTime * (isRunning ? 2.0f : 1.0f));


    }

    private void Update()
    {
        CameraLook();

        Move();
    }

    #region 基礎數值變動

    //更新血量 
    public void HpUpdate(float hp)
    {
        if (Hp + hp > maxHp || Hp + hp < 0)
        {
            return;
        }

        Hp += hp;

        float HpPerson = Hp / maxHp;

        HpTxt.text = (HpPerson * 100) + "%";
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
                    merchantShop.merchantShop_.Rotation_merchant();
                }
                else
                {
                    merchantShop.merchantShop_.SetMisRotation(false);
                }
            }
        }
        else
        {
            isLock = false;
            merchantShop.merchantShop_.SetMisRotation(false);

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

    // 移動
    void Move()
    {
        bool isRunx = true;
        bool isRunY = true;

        if (Input.GetKeyDown(playerKeyCodes.Jump))
        {
            isJump = true;

        }

        if (Input.GetKey(playerKeyCodes.leftMove))
        {
            moveItem[0] = -1;
        }
        else if (Input.GetKey(playerKeyCodes.rightMove))
        {
            moveItem[0] = 1;
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

        Run(isRunx || isRunY);

        if (isRunx && isRunY)
        {
            moveItem[0] /= 2;
            moveItem[1] /= 1.1f;
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
            nowKecode = playerKeyCodes.leftMove;
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

}


[System.Serializable]
public class PlayerKeyCode
{
    public KeyCode leftMove;
    public KeyCode rightMove;
    public KeyCode frontMove;
    public KeyCode BackMove;
    public KeyCode Jump;
    public KeyCode OpenShop;
    public KeyCode OpenBag;
    public KeyCode OpenMap;
    public KeyCode RotateCamera;
}

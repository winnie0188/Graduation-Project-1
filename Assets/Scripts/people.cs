using UnityEngine;
using System.Collections.Generic;
using NPBehave;

public class people : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("好感度")]
    public int peopleLoveValue;
    [Header("介紹")]
    public BasicPeople basicPeople;

    [Header("動畫")]
    [SerializeField] Animator ani;


    [Header("隨機位置")]
    [SerializeField] List<Vector3> purpos;
    // 是否可互動
    [SerializeField] bool interactive;

    [Header("NPC對話")]
    public talkContent currentTextDataList;



    // 分享數據
    private Blackboard sharedBlackboard;
    // 自己數據
    private Blackboard ownBlackboard;
    // 行為樹
    private Root behaviorTree;



    private void Start()
    {
        // 建立行為樹
        initBehaviorTree();
    }

    #region ===行為樹建立===
    void initBehaviorTree()
    {
        sharedBlackboard = UnityContext.GetSharedBlackboard("NPC");
        ownBlackboard = new Blackboard(sharedBlackboard, UnityContext.GetClock());
        behaviorTree = CreateBehaviourTree();
#if UNITY_EDITOR
        Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = behaviorTree;
#endif
        behaviorTree.Start();
    }

    private Root CreateBehaviourTree()
    {
        return new Root(ownBlackboard,
            new Service(0.125f, UpdateBlackboard, Layout_1())
        );
    }
    #endregion

    #region ===行為樹節點===
    public Selector Layout_1()
    {
        return new Selector(
            new BlackboardCondition("SHIFT", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                new Sequence(
                    new Action(() => n_shift()) { Label = "shift" },
                    new WaitUntilStopped()
                )
            ),
            Layout_2()
        );
    }
    public Selector Layout_2()
    {
        return new Selector(
            new BlackboardCondition("TALK", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                new Sequence(
                    new Action(() => n_idle()) { Label = "idle" },
                    new WaitUntilStopped()
                )
            ),
            Layout_3()
        );
    }
    public Selector Layout_3()
    {
        return new Selector(
            new BlackboardCondition("MOVE", Operator.IS_EQUAL, true, Stops.BOTH,
                Layout_4()
            ),
            Layout_5()
        );
    }
    public Selector Layout_4()
    {
        return new Selector(
            new BlackboardCondition("PLAYERINRANGE", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                new Sequence(
                    new Action(() => n_light()) { Label = "light" },
                    new Action(() => ani_move()) { Label = "ani_move" },
                    new Action((bool _shouldCancel) =>
                            {
                                if (!_shouldCancel)
                                {
                                    n_move();
                                    return Action.Result.PROGRESS;
                                }
                                else
                                {
                                    return Action.Result.FAILED;
                                }
                            })
                    { Label = "move" }
                )
            ),
            new Sequence(
                new Action(() => ani_move()) { Label = "ani_move" },
                new Action((bool _shouldCancel) =>
                            {
                                if (!_shouldCancel)
                                {
                                    n_move();
                                    return Action.Result.PROGRESS;
                                }
                                else
                                {
                                    return Action.Result.FAILED;
                                }
                            })
                { Label = "move" }
            )
        );
    }
    public Selector Layout_5()
    {
        return new Selector(
            new BlackboardCondition("PLAYERINRANGE", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                new Sequence(
                    new Action(() => n_light()) { Label = "light" },
                    new Action(() => n_idle()) { Label = "idle" },
                    new WaitUntilStopped()
                )
            ),
            new Sequence(
                new Action(() => n_idle()) { Label = "idle" },
                new WaitUntilStopped()
            )
        );
    }
    #endregion

    void UpdateBlackboard()
    {
        Vector3 playerLocalPos = this.transform.InverseTransformPoint(GameObject.FindGameObjectWithTag("Player").transform.position);

        // 若距離玩家x距離則PLAYERINRANGE為true
        ownBlackboard["PLAYERINRANGE"] = playerLocalPos.magnitude < 10f;

        // 若距離玩家2f則視為碰撞
        bool isToch = playerLocalPos.magnitude < 2f;

        // INTERACTIVE變數轉換確保只有一個物件被碰撞
        if (sharedBlackboard.Get<int>("INTERACTIVE") < 1 && isToch)
        {
            sharedBlackboard["INTERACTIVE"] = sharedBlackboard.Get<int>("INTERACTIVE") + 1;
            // 可互動設為true
            interactive = true;
            // 設置對話系統
            talkSystem.talkSystem_.isToch = true;
            talkSystem.talkSystem_.TriggerObj = transform;
        }

        // 沒有碰到則解除可互動
        if (!isToch && interactive)
        {
            sharedBlackboard["INTERACTIVE"] = sharedBlackboard.Get<int>("INTERACTIVE") - 1;
            interactive = false;
        }

        // 如果開啟對話框且可互動物件為自己則TALK為true，否則false
        if (interactive && PanelManage.panelManage.panels.talkPanel.gameObject.activeSelf)
        {
            ownBlackboard["TALK"] = true;
        }
        else
        {
            ownBlackboard["TALK"] = false;
        }

        boardEvent();
    }

    public virtual void boardEvent()
    {

    }

    void ani_idle()
    {
        ani.SetTrigger("IDLE");
    }

    void ani_move()
    {
        ani.SetTrigger("MOVE");
    }

    void n_move()
    {
        Debug.Log("move");
    }
    void n_idle()
    {
        ani_idle();
        Debug.Log("idle");
    }
    void n_light()
    {
        Debug.Log("light");
    }
    void n_shift()
    {
        Debug.Log("shift");

    }
}

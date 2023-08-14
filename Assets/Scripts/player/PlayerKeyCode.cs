using UnityEngine;

[System.Serializable]
public class PlayerKeyCode
{
    // 基礎控制
    public KeyCode leftMove;
    public KeyCode rightMove;
    public KeyCode frontMove;
    public KeyCode BackMove;
    public KeyCode Jump;
    public KeyCode RotateCamera;
    // 道具
    public KeyCode UseProps;
    public KeyCode cancel;
    public KeyCode RotateBlock;
    public KeyCode AltBlock;
    public KeyCode ShiftBlock;
    public KeyCode CtrlBlock;
    // 互動
    public KeyCode OpenShop;
    public KeyCode OpenBag;
    public KeyCode OpenMap;
    public KeyCode OpenESC;

    // 快捷鍵1~8
    public KeyCode[] Num = new KeyCode[8];

}

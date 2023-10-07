using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class tool : BagItemObj
{
    [SerializeField] ToolType toolType;

    [Header("斧頭和十字搞專用")]

    [SerializeField] float power;

    [SerializeField] float maxCooltime;

    [SerializeField] float ArmCooltime;
    //============================================


    #region 主function
    public override void UseIng()
    {
        if (toolType == ToolType.LOGGING)
        {
            UseProps(SuppliesType.TREE);
        }
        else if (toolType == ToolType.DIGGING)
        {
            UseProps(SuppliesType.STONE);
        }
    }

    #endregion


    void UseProps(SuppliesType suppliesType)
    {
        if (Input.GetKey(playerController.playerController_.playerKeyCodes.UseProps))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }


            GameObject Block = BuildSystem.buildSystem.GetMouseTochGameobject();

            if (Block != null && Block.TryGetComponent<Supplies>(out Supplies supplies))
            {

                if (ArmCooltime <= 0)
                {
                    ArmCooltime = maxCooltime;
                    if (supplies.SuppliesType == suppliesType && SuppliesType.TREE == suppliesType)
                    {
                        logTree(supplies);
                    }
                    else if (supplies.SuppliesType == suppliesType && SuppliesType.STONE == suppliesType)
                    {
                        digStone(supplies);
                    }
                }
            }
        }

        if (ArmCooltime > 0)
        {
            ArmCooltime -= Time.deltaTime;
        }
    }

    void logTree(Supplies supplies)
    {
        supplies.UpdateHp(power);
    }

    void digStone(Supplies supplies)
    {
        supplies.UpdateHp(power);
    }
}

public enum ToolType
{
    //伐木
    LOGGING,
    //挖石頭
    DIGGING,
    //手環
    WRISTBAND
}
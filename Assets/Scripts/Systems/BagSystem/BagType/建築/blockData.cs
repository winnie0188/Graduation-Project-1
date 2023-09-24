using UnityEngine;

public class blockData : MonoBehaviour
{
    //對應物件池
    public BagItem backitem;
    public blockType blockType;
    public Transform position;
    public void setBlockType(blockType type)
    {
        blockType = type;
    }

}

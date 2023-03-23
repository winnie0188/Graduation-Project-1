using UnityEngine;

public class BagItemObj : MonoBehaviour
{ }


[System.Serializable]
public class potion
{
    [Range(1, 100)] public int bloodHP;//補血量
}


[System.Serializable]
public class equipment
{
    [Range(1, 100)] public int defense;//防禦
}


[System.Serializable]
public class clothe
{
    public enum clotheType
    {
        clothe
    }

    public clotheType clotheType_;
}

[System.Serializable]
public class material
{ }

[System.Serializable]
public class food
{
    [Range(1, 100)] public int bloodHungry;//補飽食
    public Sprite ArmSprite;//手持圖片
}

[System.Serializable]
public class tool
{
    [Header("手持物件")]
    public Sprite ArmSprite;
}

[System.Serializable]
public class block
{
    [Header("手持物件")]
    public Sprite ArmSprite;
}
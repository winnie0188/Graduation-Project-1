
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class BagItemObj
{
    // 初始化...
    public virtual void Create()
    {
        //相當於start
    }
    // 道具使用中...
    public virtual void UseIng()
    {
        // 拿著的時候發生什麼事
        deleteBlcok();
    }

    // 切換手持時，可能會釋放些東西
    public virtual void Relese()
    {

    }


    public void deleteBlcok()
    {
        if (Input.GetKeyDown(playerController.playerController_.playerKeyCodes.UseProps))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }


            GameObject Block = BuildSystem.buildSystem.GetMouseTochGameobject();
            if (Block == null)
            {
                return;
            }

            RecursiveCheckBlock();
            RecursiveCheckBlock();

            void RecursiveCheckBlock()
            {
                if (Block.transform.parent != null)
                {
                    Block = Block.transform.parent.gameObject;

                    if (checkBlockData())
                    {
                        return;
                    }
                }
                return;
            }


            bool checkBlockData()
            {
                if (Block.TryGetComponent<blockData>(out blockData pool))
                {
                    pool.backitem.GetBlock().destoryOldBlock(Block, pool.backitem);

                    return true;
                }
                return false;
            }

        }

    }
}


[System.Serializable]
public class potion : BagItemObj
{

}


[System.Serializable]
public class other : BagItemObj
{

}


[System.Serializable]
public class clothe : BagItemObj
{

}

[System.Serializable]
public class material : BagItemObj
{

}

[System.Serializable]
public class food : BagItemObj
{

}

[System.Serializable]
public class tool : BagItemObj
{

}


[System.Serializable]
public class taskList : BagItemObj
{

}

[System.Serializable]
public class teachBook : BagItemObj
{

}
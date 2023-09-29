using DG.Tweening;
using UnityEngine;
using System.Collections;

public class playerInteractive : MonoBehaviour
{
    // Start is called before the first frame update
    #region 
    RaycastHit[] Interactives;
    GameObject interactiveObject;
    GameObject interactiveParent;
    Transform playerEyes;
    playerController playerController;
    #endregion


    void Update()
    {
        if (playerController.playerController_ != null)
        {
            if (playerController == null)
            {
                playerController = playerController.playerController_;
            }

            if (playerEyes == null)
            {
                playerEyes = playerController.playerController_.transform;
            }
            userInteractive();

            if (Input.GetKeyDown(playerController.playerKeyCodes.OpenShop) && playerController.sitState == playerSitState.SITDOWN)
            {
                StartCoroutine(sitState());
            }
            else if (Input.GetKeyDown(playerController.playerKeyCodes.OpenShop) && interactiveObject)
            {
                if (interactiveObject.layer == LayerMask.NameToLayer("NPC"))
                {
                    interactiveNPC();
                }
                else if (interactiveObject.layer == LayerMask.NameToLayer("block"))
                {
                    if (!RecursiveCheckBlock(interactiveObject.transform))
                    {
                        RecursiveCheckBlock(interactiveParent.transform);
                    }
                }
            }
        }
    }

    void userInteractive()
    {
        Vector3 rayDirection = playerEyes.TransformDirection(Vector3.forward);

        // 使用Physics.RaycastAll来投射射线
        Interactives = Physics.RaycastAll(
            playerEyes.position - new Vector3(0, 0.3f, 0),
            rayDirection,
            3.5f);
        Debug.DrawRay(playerEyes.position - new Vector3(0, 0.35f, 0), rayDirection * 10f, Color.red);

        if (Interactives.Length > 0)
        {
            interactiveObject = Interactives[0].transform.gameObject;
        }
        else
        {
            interactiveObject = null;
        }

    }

    #region 互動BLOCK

    bool RecursiveCheckBlock(Transform iobject)
    {
        if (iobject.parent != null)
        {
            interactiveParent = iobject.parent.gameObject;

            if (interactiveParent.TryGetComponent<blockData>(out blockData pool))
            {
                checkType(pool);
                return true;
            }
        }

        return false;
    }

    void checkType(blockData pool)
    {
        if (pool.blockType.Equals(blockType.DOOR))
        {
            plyerDoorAni(pool, blockType.OPENDOOR, -90);
        }
        else if (pool.blockType.Equals(blockType.OPENDOOR))
        {
            plyerDoorAni(pool, blockType.DOOR, 90);
        }
        else if (pool.blockType.Equals(blockType.CHAIR))
        {
            playerController.sitPos = pool.position.position + Vector3.up * 1f;
            playerController.sitState = playerSitState.SITDOWN;
            playerController.collider.enabled = false;
            playerController.rigi.useGravity = false;
        }
    }

    #region 門互動
    void plyerDoorAni(blockData pool, blockType newType, int rotate)
    {
        pool.setBlockType(blockType.DEFAULT);

        Transform door = pool.transform.GetChild(0);
        Tween rotationTween = door.DORotate(
            door.transform.rotation.eulerAngles + new Vector3(0, 0, rotate),
            0.5f, RotateMode.Fast
            );
        // 動畫結束後執行
        rotationTween.OnComplete(() =>
        {

            // 切換type
            pool.setBlockType(newType);
        });
    }
    #endregion

    IEnumerator sitState()
    {
        playerController.sitState = playerSitState.SITUP;
        yield return new WaitForSeconds(1.3f);
        playerController.sitState = playerSitState.NONE;
        playerController.collider.enabled = true;
        playerController.rigi.useGravity = true;
        playerEyes.position += Vector3.up;
        playerEyes.GetChild(1).localPosition = new Vector3(0, 0, 0);
    }
    #endregion

    #region 互動NPC
    public void interactiveNPC()
    {
        if (interactiveObject.TryGetComponent<NPC>(out var npc))
        {
            npc.openInteractive();
        }
    }
    #endregion
}

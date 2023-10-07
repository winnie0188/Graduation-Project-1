using DG.Tweening;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

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

    #region 頭頂對話
    public bool isFinshTalk = true;
    public TMP_Text content;
    [SerializeField] Transform talkView;
    [SerializeField] float Stiff;
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
                    if (interactiveObject.TryGetComponent<Supplies>(out var supplies))
                    {
                        if (supplies.SuppliesType == SuppliesType.FRUIT)
                        {
                            supplies.collectionwild();
                        }
                        else if (supplies.SuppliesType == SuppliesType.PLANTING && supplies.PlantState == PlantState.LIVE)
                        {
                            supplies.collectionLocal();
                        }
                    }
                    else
                    {
                        if (!RecursiveCheckBlock(interactiveObject.transform))
                        {
                            RecursiveCheckBlock(interactiveParent.transform);
                        }
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


    #region 頭頂對話
    public void setContent(string content)
    {
        if (isFinshTalk)
        {
            this.content.text = "";
            isFinshTalk = false;
            if (!talkView.gameObject.activeSelf)
            {
                Image image = talkView.GetComponent<Image>();
                var a = image.color;
                a.a = 0;
                image.color = a;
                // talkView.localPosition = Vector3.zero;

                Sequence se = DOTween.Sequence();

                talkView.gameObject.SetActive(true);
                // se.Append(talkView.DOLocalMoveY(2.3137f, 0.5f).SetEase(Ease.Flash));
                se.Append(image.DOFade(1, 0.5f).SetEase(Ease.InSine));


                se.Play();

                se.OnComplete(() =>
                {
                    StartCoroutine(talk(content));
                });
            }
            else
            {
                StartCoroutine(talk(content));
            }
        }

        IEnumerator talk(string content)
        {
            for (int i = 0; i < content.Length; i++)
            {
                this.content.text += content[i];
                yield return new WaitForSeconds(0.05f);
            }

            this.content.text = content;
            yield return new WaitForSeconds(Stiff);
            isFinshTalk = true;
        }
    }
    public void closeConetnt()
    {
        talkView.gameObject.SetActive(false);
    }

    public bool getNPCtalkFinsh()
    {
        return isFinshTalk;
    }
    #endregion
}

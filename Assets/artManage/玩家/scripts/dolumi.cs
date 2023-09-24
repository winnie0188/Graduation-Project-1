using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class dolumi : MonoBehaviour
{
    public List<SpriteResolver> spriteResolvers = new List<SpriteResolver>();
    public GameObject arm;
    public GameObject tool;
    // Start is called before the first frame update
    SpriteResolver toolResolver;

    [SerializeField] SpriteRenderer leftLeg;

    string clotheName;


    void Start()
    {
        foreach (var resolver in FindObjectsOfType<SpriteResolver>())
        {
            spriteResolvers.Add(resolver);

        }

        //預設造型
        foreach (var resolver in FindObjectsOfType<SpriteResolver>())
        {
            resolver.SetCategoryAndLabel(resolver.GetCategory(), "normal");
        }
        leftLeg.sortingOrder = 10;

    }

    public void UpdateClothe()
    {
        if (BagManage.bagManage.hotKeyStore.HotKeys_Clothe.Length > 0)
        {
            var clothe = BagManage.bagManage.hotKeyStore.HotKeys_Clothe[0];
            if (clothe.HotKey_Bag != -1)
            {
                switchClothe(
                    BagManage.bagManage.bagSore[clothe.HotKey_Bag].BagItems[clothe.HotKey_item].
                    GetClothe().clotheName
                    );

                return;
            }
        }

        switchClothe("normal");
    }

    public void switchClothe(string s, int sort = 8)
    {
        if (s.Equals("normal"))
        {
            sort = 10;
        }
        chageClothe(sort, s);
    }

    public void chageClothe(int sort, string type)
    {
        foreach (var resolver in FindObjectsOfType<SpriteResolver>())
        {
            resolver.SetCategoryAndLabel(resolver.GetCategory(), type);
        }
        leftLeg.sortingOrder = sort;
    }



    void Update()
    {


        //     if (Input.GetKeyDown(KeyCode.A))
        //     {
        //         foreach (var resolver in FindObjectsOfType<SpriteResolver>())
        //         {
        //             resolver.SetCategoryAndLabel(resolver.GetCategory(), "normal");
        //         }
        //         leftLeg.sortingOrder = 9;
        //     }


        //     if (Input.GetKeyDown(KeyCode.B))
        //     {
        //         foreach (var resolver in FindObjectsOfType<SpriteResolver>())
        //         {
        //             resolver.SetCategoryAndLabel(resolver.GetCategory(), "sleep");
        //         }
        //         leftLeg.sortingOrder = 8;
        //     }

        //     if (Input.GetKeyDown(KeyCode.C))
        //     {
        //         foreach (var resolver in FindObjectsOfType<SpriteResolver>())
        //         {
        //             resolver.SetCategoryAndLabel(resolver.GetCategory(), "god");
        //         }
        //         leftLeg.sortingOrder = 8;
        //     }

        //     if (Input.GetKeyDown(KeyCode.D))
        //     {
        //         foreach (var resolver in FindObjectsOfType<SpriteResolver>())
        //         {
        //             resolver.SetCategoryAndLabel(resolver.GetCategory(), "day");
        //         }
        //         leftLeg.sortingOrder = 8;
        //     }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (arm.activeInHierarchy == true)
            {
                tool.SetActive(true);
                arm.SetActive(false);
            }
            else if (arm.activeInHierarchy == false)
            {
                tool.SetActive(false);
                arm.SetActive(true);
            }
        }
    }


}

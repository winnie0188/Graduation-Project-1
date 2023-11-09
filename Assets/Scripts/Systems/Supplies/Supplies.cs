using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supplies : MonoBehaviour
{
    [SerializeField] Vector3[] generateLocation;



    [SerializeField] SuppliesType suppliesType;

    //重生時間
    [Header("重生時間")]
    [SerializeField] int rebirthTime;
    int initRirthTime;


    [Header("===物資類===")]
    [SerializeField] float maxHp;

    [Header("===種植類===")]//成熟時間
    [SerializeField] int maxTime;
    [SerializeField] Sprite[] plantImg;
    [SerializeField] PlantState plantState;


    [Header("採集:直接拿/採集:掉落")]
    [SerializeField] Drops[] drops;

    float Hp;
    int time;



    public Vector3[] GenerateLocation { get => generateLocation; set => generateLocation = value; }
    public SuppliesType SuppliesType { get => suppliesType; set => suppliesType = value; }
    public int Time { get => time; set => time = value; }
    public PlantState PlantState { get => plantState; set => plantState = value; }

    public void init(Vector3 pos)
    {
        if (suppliesType == SuppliesType.PLANTING)
        {
            Time = 0;
            PlantState = PlantState.LIVE;
        }
        else if (suppliesType == SuppliesType.TREE || suppliesType == SuppliesType.STONE)
        {
            Hp = maxHp;
        }


        transform.position = pos;
        gameObject.SetActive(true);
    }


    public void running()
    {
        if ((playerController.playerController_.transform.position - transform.position).magnitude > 100)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }


    #region 樹石頭專用
    public void UpdateHp(float hp)
    {
        Hp -= hp;
        if (Hp <= 0)
        {
            die();
        }

        fall();
    }

    public void die()
    {
        SuppliesSystem.suppliesSystem.removeSupplies(
            SuppliesSystem.suppliesSystem.getKey(transform),
            transform
        );
        gameObject.SetActive(false);
    }



    //掉落
    public void fall()
    {
        foreach (var item in drops)
        {
            int person = Random.Range(1, 101);

            if (person <= item.person)
            {
                int count = Random.Range(item.count.current, item.count.max + 1);

                if (count > 0)
                {
                    FallSystem.fallSystem.fall(
                        item.bagItems,
                        count,
                        transform.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1))
                    );
                }
                else
                {
                    print(0);
                }
            }
        }
    }
    #endregion


    #region 果實專用
    public void collectionwild()
    {
        foreach (var item in drops)
        {
            int person = Random.Range(1, 101);

            if (person <= item.person)
            {
                int count = Random.Range(item.count.current, item.count.max + 1);

                if (count > 0)
                {
                    BagManage.bagManage.checkItem(
                        item.bagItems,
                        count,
                        false,
                        true
                    );
                }
            }
        }

        die();


        //植物重生
        SuppliesSystem.suppliesSystem.RebirthFunction(initRirthTime, rebirthTime, transform);
    }
    #endregion


    #region  種植專用

    IEnumerator plantUpdate()
    {
        int timeState = maxTime / 2;

        UpdateChildSprite(plantImg[0]);

        for (int i = 1; i <= maxTime; i++)
        {
            if (i == timeState)
            {
                UpdateChildSprite(plantImg[1]);
            }
            Time = i;
            yield return new WaitForSeconds(1);
        }

        UpdateChildSprite(plantImg[2]);
    }


    public void collectionLocal()
    {
        PlantState = PlantState.LIVE;
        UpdateChildSprite(plantImg[2]);

        foreach (var item in drops)
        {
            int person = Random.Range(1, 101);

            if (item.person <= person)
            {
                int count = Random.Range(item.count.current, item.count.max + 1);

                if (count > 0)
                {
                    BagManage.bagManage.checkItem(
                        item.bagItems,
                        count,
                        false,
                        true
                    );
                }
            }
        }
    }


    void UpdateChildSprite(Sprite sprite)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }

    #endregion

}

public enum SuppliesType
{
    //樹
    TREE,
    //石頭
    STONE,
    //水果
    FRUIT,
    //種植
    PLANTING
}


public enum PlantState
{
    LIVE,
    DIE
}
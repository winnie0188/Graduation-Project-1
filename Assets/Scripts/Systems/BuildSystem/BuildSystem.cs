using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildSystem : MonoBehaviour
{



    //chunkParent
    [SerializeField] Transform ChunkParent;

    // 每個chunk的大小
    [SerializeField] float chunkSize;

    [SerializeField] int[] rowSize;
    [SerializeField] int[] colSize;

    Dictionary<int, Transform> row = new Dictionary<int, Transform>();
    Dictionary<Vector2Int, Transform> col = new Dictionary<Vector2Int, Transform>();

    [Header("放入Grid")]
    [SerializeField] GridTransform gridTransform;




    public static BuildSystem buildSystem;


    Dictionary<Transform, int> blocks = new Dictionary<Transform, int>();


    Camera mainCamera;


    private void Awake()
    {
        buildSystem = this;

        mainCamera = Camera.main;

        initChunk();

        StartCoroutine(dynamicDisplay());
    }

    //隱藏或顯示地形
    IEnumerator dynamicDisplay()
    {
        yield return new WaitForSeconds(1);

        Transform LocalPos = GetObjectParent(playerController.playerController_.transform.position);

        for (int i = 0; i < ChunkParent.childCount; i++)
        {
            if (ChunkParent.GetChild(i) != LocalPos.parent)
            {
                ChunkParent.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                ChunkParent.GetChild(i).gameObject.SetActive(true);

                for (int j = 0; j < ChunkParent.GetChild(i).childCount; j++)
                {
                    if (ChunkParent.GetChild(i).GetChild(j) != LocalPos)
                    {
                        ChunkParent.GetChild(i).GetChild(j).gameObject.SetActive(false);
                    }
                    else
                    {
                        ChunkParent.GetChild(i).GetChild(j).gameObject.SetActive(true);
                    }
                }
            }
        }


        StartCoroutine(dynamicDisplay());
    }


    // 設置旋轉
    public void rotate(Transform gameObject)
    {
        // 獲取當前 GameObject 的旋轉角度
        float currentRotation = gameObject.rotation.eulerAngles.y;

        // 計算旋轉後的角度
        float targetRotation = currentRotation + 90f;

        // 當旋轉角度超過360度時，重置為0
        if (targetRotation >= 360f)
        {
            targetRotation -= 360f;
        }

        // 將 GameObject 的旋轉設置為新的角度
        gameObject.rotation = Quaternion.Euler(gameObject.rotation.eulerAngles.x, targetRotation, gameObject.rotation.eulerAngles.z);
    }


    #region buildData/Get/Set
    public Dictionary<Transform, int> GetBuildData()
    {
        return blocks;
    }

    public void setBuildData(Transform gameObject, int id)
    {
        blocks.Add(gameObject, id);
    }

    #endregion

    #region 初始化Chunk
    public void initChunk()
    {
        // 生成Chunk
        // 0是x正
        for (int poX = 0; poX < rowSize[0]; poX++)
        {
            SetXChunk(poX);
            // 每一row都會生成一次，因為(0,0),(1,0),(2,0)
            SetZChunk(row[poX], poX);
        }

        for (int neX = -1; neX > rowSize[1]; neX--)
        {
            SetXChunk(neX);
            // 每一row都會生成一次
            SetZChunk(row[neX], neX);
        }
    }

    //生成row
    void SetXChunk(int x)
    {
        GameObject rowChunk = new GameObject();
        rowChunk.transform.SetParent(ChunkParent);
        rowChunk.name = x.ToString();
        row[x] = rowChunk.transform;
    }

    // 生成col
    void SetZChunk(Transform Parent, int x)
    {
        // 0是x正
        for (int poZ = 0; poZ < colSize[0]; poZ++)
        {
            GameObject colChunk = new GameObject();
            colChunk.transform.SetParent(Parent);
            colChunk.name = poZ.ToString();
            col[new Vector2Int(x, poZ)] = colChunk.transform;
        }

        for (int neZ = -1; neZ > rowSize[1]; neZ--)
        {
            GameObject colChunk = new GameObject();
            colChunk.transform.SetParent(Parent);
            colChunk.name = neZ.ToString();
            col[new Vector2Int(x, neZ)] = colChunk.transform;
        }
    }
    #endregion

    #region 獲取當前chunk座標
    public Transform GetObjectParent(Vector3 pos)
    {

        // row
        int x = GetCoodinate(pos.x);
        // col
        int z = GetCoodinate(pos.z);


        if (!row.TryGetValue(x, out var valueX))
        {
            SetXChunk(x);
        }

        if (!col.TryGetValue(new Vector2Int(x, z), out var valueZ))
        {
            GameObject colChunk = new GameObject();
            colChunk.transform.SetParent(row[x]);
            colChunk.name = z.ToString();
            col[new Vector2Int(x, z)] = colChunk.transform;
        }


        return col[new Vector2Int(x, z)];
    }


    int GetCoodinate(float Pos)
    {

        // 先獲取第幾個child
        //第0 -ROW
        if (Pos <= chunkSize / 2.0f && Pos > -chunkSize / 2.0f)
        {
            return 0;
        }
        //如果是正
        else if (Pos > 0)
        {
            float distance = Pos - chunkSize / 2.0f;
            int coodinate = 0;

            while (distance >= chunkSize)
            {
                distance = distance - chunkSize;
                coodinate++;
            }

            if (distance > 0)
            {
                coodinate++;
            }

            return coodinate;
        }
        // 如果是負
        else
        {
            float distance = Pos + chunkSize / 2.0f;
            int coodinate = 0;

            while (distance <= -chunkSize)
            {
                distance = distance + chunkSize;
                coodinate--;
            }

            if (distance == 0)
            {
                coodinate--;
            }

            return coodinate;
        }
    }

    #endregion
    //inputCode
    public Vector3 GetMouseWoldPosition(int inputCode)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        int layerMask = LayerMask.GetMask("block");

        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
        {
            // isAlt是否接合
            if (inputCode == 0)
            {
                return SnapCoordinateToGrid(raycastHit.point, raycastHit.normal);
            }
            // isShift重疊
            else if (inputCode == 1)
            {
                return SnapCoordinateToGrid(raycastHit.collider.transform);
            }
            // isCtrl自由布局
            else if (inputCode == 2)
            {
                return ctrlSnapCoordinateToGrid(raycastHit.point, raycastHit.normal);
            }
            else
            {
                return raycastHit.point + raycastHit.normal / 1.11f;
            }
        }
        else
        {
            return new Vector3(0, -10000, 0);
        }
    }

    public GameObject GetMouseTochGameobject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        int layerMask = LayerMask.GetMask("block");

        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
        {
            return raycastHit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }


    #region 按鍵模式
    // 正常模式
    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPosW = gridTransform.GridLayoutw.WorldToCell(position);
        Vector3Int cellPosH = gridTransform.GridLayouth.WorldToCell(position);

        Vector3 positionW = gridTransform.Gridw.GetCellCenterWorld(cellPosW);
        Vector3 positionH = gridTransform.Gridh.GetCellCenterWorld(cellPosH);

        position = new Vector3(positionW.x, positionH.y - 0.5f, positionH.z);

        return position;
    }
    // Alt觸發
    public Vector3 SnapCoordinateToGrid(Vector3 position, Vector3 normal)
    {
        Vector3Int cellPosW = gridTransform.GridLayoutw.WorldToCell(position);
        Vector3Int cellPosH = gridTransform.GridLayouth.WorldToCell(position);

        Vector3 positionW = gridTransform.Gridw.GetCellCenterWorld(cellPosW);
        Vector3 positionH = gridTransform.Gridh.GetCellCenterWorld(cellPosH);


        float x = Mathf.Round(normal.x * 10f) / 10f;
        float y = Mathf.Round(normal.y * 10f) / 10f;
        float z = Mathf.Round(normal.z * 10f) / 10f;

        if (x > 0)
        {
            position = new Vector3(position.x + 0.5f, positionH.y - 0.5f, positionH.z);
        }
        else if (x < 0)
        {
            position = new Vector3(position.x - 0.5f, positionH.y - 0.5f, positionH.z);
        }
        else if (z > 0)
        {
            position = new Vector3(positionW.x, positionH.y - 0.5f, position.z + 0.5f);
        }
        else if (z < 0)
        {
            position = new Vector3(positionW.x, positionH.y - 0.5f, position.z - 0.5f);
        }
        else
        {
            position = new Vector3(positionW.x, position.y, positionH.z);
        }

        return position;
    }

    //shift觸發
    public Vector3 SnapCoordinateToGrid(Transform targe)
    {
        if (targe.name != "Plane")
        {
            return targe.parent.position;
        }
        else
        {
            return new Vector3(0, -10000, 0);
        }

    }
    //ctrl觸發
    public Vector3 ctrlSnapCoordinateToGrid(Vector3 position, Vector3 normal)
    {
        float x = Mathf.Round(normal.x * 10f) / 10f;
        float y = Mathf.Round(normal.y * 10f) / 10f;
        float z = Mathf.Round(normal.z * 10f) / 10f;

        if (x > 0)
        {
            position = new Vector3(position.x + 0.5f, position.y - 0.5f, position.z);
        }
        else if (x < 0)
        {
            position = new Vector3(position.x - 0.5f, position.y - 0.5f, position.z);
        }
        else if (z > 0)
        {
            position = new Vector3(position.x, position.y - 0.5f, position.z + 0.5f);
        }
        else if (z < 0)
        {
            position = new Vector3(position.x, position.y - 0.5f, position.z - 0.5f);
        }
        else
        {
            position = new Vector3(position.x, position.y, position.z);
        }

        return position;
    }
    #endregion

    //合併
    public void combine(Transform obj)
    {
        obj.GetChild(0).gameObject.SetActive(false);
        obj.GetChild(1).gameObject.SetActive(true);
    }

    //拆除
    public void dismantle(Transform obj)
    {
        obj.GetChild(0).gameObject.SetActive(true);
        obj.GetChild(1).gameObject.SetActive(false);
    }
}


[System.Serializable]
public class GridTransform
{
    public GridLayout GridLayoutw;
    public GridLayout GridLayouth;
    public Grid Gridw;
    public Grid Gridh;
}


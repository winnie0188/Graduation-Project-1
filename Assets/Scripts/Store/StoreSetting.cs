using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreSetting : MonoBehaviour
{
    public static StoreSetting storeSetting;
    // 透明綠材質
    [SerializeField] Material trasparentGreen;
    [SerializeField] BagItemStore bagItemStore;

    private void Awake()
    {
        storeSetting = this;
    }


    [ContextMenu("設置道具ID")]
    public void settingID()
    {
        bagItemStore.reset_();
    }

    [ContextMenu("設置道具總量")]
    public void setbagStoreIndex_()
    {
        bagItemStore.setbagStoreIndex_();
    }

    [ContextMenu("設置預置物件")]
    public void setPrefab()
    {
        int index = 0;

        GameObject gameObject = bagItemStore.setPrefab(index);

        while (gameObject != null)
        {
            CreateGreenObj();
            index++;
            gameObject = bagItemStore.setPrefab(index);
        }

        void CreateGreenObj()
        {
            Transform newGameObject = Instantiate(
                gameObject.transform.GetChild(0),
                gameObject.transform.GetChild(0).position,
                gameObject.transform.GetChild(0).rotation
            );

            newGameObject.SetParent(transform);

            MeshRenderer[] meshRenderers = newGameObject.GetComponentsInChildren<MeshRenderer>();
            MeshCollider[] meshColliders = newGameObject.GetComponentsInChildren<MeshCollider>();
            BoxCollider[] boxColliders = newGameObject.GetComponentsInChildren<BoxCollider>();

            int i = 0;
            while (i < meshRenderers.Length)
            {
                for (int j = 0; j < meshRenderers[i].materials.Length; j++)
                {
                    meshRenderers[i].materials[j].CopyPropertiesFromMaterial(trasparentGreen);
                }
                i++;
            }

            for (int j = 0; j < meshColliders.Length; j++)
            {
                DestroyImmediate(meshColliders[j]);
            }

            for (int j = 0; j < boxColliders.Length; j++)
            {
                DestroyImmediate(boxColliders[j]);
            }
        }
    }


    #region bagItemStore/Get
    public BagItemStore GetBagItemStore()
    {
        return bagItemStore;
    }
    #endregion
}

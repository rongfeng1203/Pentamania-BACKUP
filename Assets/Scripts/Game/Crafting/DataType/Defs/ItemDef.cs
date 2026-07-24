using UnityEngine;

[CreateAssetMenu(menuName = "Game/Defs/Item", fileName = "ItemDef")]
public class ItemDef : IngredientDef
{
    [SerializeField] private int maxStack = int.MaxValue;
    [SerializeField] private GameObject baseIngredient; // set this to a prefab to get all the stats automatically

    public int GetMaxStack()
    {
        return maxStack;
    }

    public Mesh GetMesh()
    {
        if (baseIngredient.GetComponent<MeshFilter>() != null)
            return baseIngredient.GetComponent<MeshFilter>().sharedMesh;
        else
            return null;
    }

    public Material GetMaterial()
    {
        if (baseIngredient.GetComponent<MeshRenderer>() != null)
            return baseIngredient.GetComponent<MeshRenderer>().sharedMaterial;
        else
            return null;
    }
    
    public Vector3 GetScale()
    {
        //Debug.Log(baseIngredient.transform.localScale.ToString());
        return baseIngredient.transform.localScale;
    }

    public GameObject GetPrefab()
    {
        //Debug.Log(baseIngredient.transform.localScale.ToString());
        return baseIngredient;
    }
    
    /*
    public IngredientProperty[] GetProperties()
    {
        return properties;
    }
    */
}
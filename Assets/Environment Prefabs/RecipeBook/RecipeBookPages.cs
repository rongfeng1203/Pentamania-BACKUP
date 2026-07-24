using UnityEngine;

public class RecipeBookPages : MonoBehaviour
{
    public string pageType;
    public RecipeBookManager manager;
    private MeshRenderer pageMesh;
    private SkinnedMeshRenderer flipMesh;

    void Start()
    {
        if (pageType == "Page1" || pageType == "Page2")
        {
            pageMesh = GetComponent<MeshRenderer>();
        }
        if (pageType == "Flip1" || pageType == "Flip2")
        {
            flipMesh = GetComponent<SkinnedMeshRenderer>();
        }
    }

    void Update()
    {
        if (pageType == "Page1")
        {
            pageMesh.material = manager.page1mat;
        }
        if (pageType == "Page2")
        {
            pageMesh.material = manager.page2mat;
        }
        if (pageType == "Flip1")
        {
            flipMesh.material = manager.flip1mat;
        }
        if (pageType == "Flip2")
        {
            flipMesh.material = manager.flip2mat;
        }
    }
}

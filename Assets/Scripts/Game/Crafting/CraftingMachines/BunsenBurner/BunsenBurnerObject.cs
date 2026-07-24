using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BunsenBurnerObject : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private BunsenBurnerRecipe[] recipes;
    [SerializeField] private IngredientTagDef burnedTagDef;
    [SerializeField] private float fallbackCookTime = 3f;

    [Header("VFX")]
    [SerializeField] private VisualEffect vfxStart;
    [SerializeField] private VisualEffect vfxSuccess;
    [SerializeField] private VisualEffect vfxBurned;

    [SerializeField] private List<SolidObject> solidsInside = new();

    private BunsenBurner burner;

    private void Awake()
    {
        burner = new BunsenBurner(recipes, burnedTagDef, fallbackCookTime);
        burner.OnCookFinish += HandleFinish;
    }

    private void Start()
    {
        AudioManager.instance.PlaySound("bunsen_start", gameObject);
    }

    private void Update()
    {
        burner.Tick(Time.deltaTime);
    }

    #region Receive
    public void SolidEntered(SolidObject so)
    {
        if (!solidsInside.Contains(so))
            solidsInside.Add(so);
        burner.InsertSolid(so);
        if (vfxStart) vfxStart.Play();
    }

    public void SolidExited(SolidObject so)
    {
        solidsInside.Remove(so);
        burner.RemoveSolid(so);
    }
    #endregion

    private void HandleFinish(SolidObject so)
    {
        ItemStack cur = (ItemStack)so.GetIngredient();
        bool burned = cur.tags != null &&
                      cur.tags.Exists(t => t.ingredientTagDef == burnedTagDef);
        
        
        if (burned)
        {
            if (vfxBurned) vfxBurned.Play();
            AudioManager.instance.PlaySound("bunsen_burn", gameObject);
            Debug.Log("[BunsenObj] Burned with no recipe");
        }
        else
        {
            //if (vfxSuccess) vfxSuccess.Play();

            Debug.Log("[BunsenObj] Burned with recipe");
        }
    }
}
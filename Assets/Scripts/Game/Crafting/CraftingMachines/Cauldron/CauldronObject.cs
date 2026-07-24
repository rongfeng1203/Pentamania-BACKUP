using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CauldronObject : MonoBehaviour
{
    [SerializeField] private float maxCapacity = 100f;
    [SerializeField] private int itemSlots = 16;
    [SerializeField] private int fluidTanks = 1;
    [SerializeField] private float tankCapacity = 9999f;
    [SerializeField] private Transform outputPoint;
    [SerializeField] private CauldronRecipe[] recipes;
    [SerializeField] private GameObject solidObjectPrefab;
    [SerializeField] private GameObject liquidPlane;

    [SerializeField] private float progressMax = 100f;
    [SerializeField] private float progressPerTurn = 20f;
    private float progress = 0f;
    [SerializeField] private HandleController handle;
    
    private float prevAngle = 0f;

    private Cauldron data;
    
    public Action<float> OnExplodeVisual;
    public Action<ItemStack> OnSpawnSolid;
    
    //DEBUG
    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private FluidStorage fluidStorage;
    
    //COLOR
    [SerializeField] private Color targetColor = Color.white;
    [SerializeField] private float fadeSpeed = 1.0f;
    private Material liquidMaterial;

    [Header("Visual Effects")] 
    [SerializeField] private ParticleSystem craftingFinishedNormal;
    [SerializeField] private ParticleSystem craftingFinishedInferior;
    [SerializeField] private ParticleSystem craftingFinishedAdvanced;
    //[SerializeField] private ParticleSystem explosion;

    [Header("Advanced Effects")]
    [SerializeField] private CauldronEffectExplosion explosion;
    [SerializeField] private CauldronShaker shaker;
    [SerializeField] private CauldronEffects effects;

    [Header("Sounds")] 
    [SerializeField] private List<FluidDef> potionSounds;
    [SerializeField] private GameObject voiceLocation;

    private bool isBubbling;

    private void Awake()
    {
        data = new Cauldron(recipes, itemSlots, fluidTanks, tankCapacity);
        data.OnExplode += HandleExplosion;
        targetColor = liquidPlane.GetComponent<MeshRenderer>().material.GetColor("_BaseColor");
        targetColor = data.CalculateFluidColor();
        SetFluidColor(targetColor);
        if (liquidPlane != null)
            liquidMaterial = liquidPlane.GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        AudioManager.instance.PlayVoiceLine(0, voiceLocation);
    }
    
    void Update()
    {
        if (!handle) return;

        float angle = handle.Angle;

        if (angle - prevAngle >= 0.0001f) GainProgress((angle - prevAngle) / 360 * progressPerTurn);
        
        /*
        if (prevAngle < -170f && angle > 170f)
            OnHandleFullTurn();
            */

        prevAngle = angle;
        
        //DEBUG
        itemStorage = data.GetItemStorage();
        fluidStorage = data.GetFluidStorage();

        UpdateFluidColor();

        shaker.positionShakeStrength = 0.06f * data.GetTotalAmount() / maxCapacity;
        shaker.rotationShakeStrength = 5 * data.GetTotalAmount() / maxCapacity;

        if (data.GetTotalAmount() >= 0.01)
        {
            if (isBubbling == false)
                AudioManager.instance.PlaySound("cauldron_start", gameObject);
            isBubbling = true;
        }
        /*
        else
        {
            if (isBubbling == true)
                AudioManager.instance.PlaySound("cauldron_", gameObject);
            isBubbling = false;
        }
        */
            
    }

    public float GetCurrentStrength()
    {
        return data.GetTotalAmount() / maxCapacity;
    }

    private void UpdateFluidColor()
    {
        if (liquidMaterial == null) return;

        Color currentColor = liquidMaterial.GetColor("_BaseColor");
        Color newColor = Color.Lerp(currentColor, targetColor, fadeSpeed * Time.deltaTime);
        liquidMaterial.SetColor("_BaseColor", newColor);
    }
    
    private void SetFluidColor(Color color)
    {
        if (liquidMaterial == null) return;

        Color currentColor = liquidMaterial.GetColor("_BaseColor");
        liquidMaterial.SetColor("_BaseColor", color);
    }
    
    public void UpdateTargetColor()
    {
        targetColor = data.CalculateFluidColor();
        //Debug.Log("[Cauldron] Updated the target color: " + targetColor);
    }

    public Color CalculateFluidColor()
    {
        return data.CalculateFluidColor();
    }
    
    public void UpdateCustomTargetColor(Color color)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        
        float oldV = v * 100f;
        float mappedV = oldV / 100f * 80f;
        float finalV = mappedV / 100f;

        Color newColor = Color.HSVToRGB(h, s, finalV);
        targetColor = newColor;
    }

    public Color GetCurrentLiquidColor()
    {
        return liquidMaterial.GetColor("_BaseColor");
    }

    #region Spoon
    private void OnHandleFullTurn() => GainProgress(progressPerTurn);

    private void GainProgress(float amt)
    {
        progress = Mathf.Min(progress + amt, progressMax);
        //Debug.Log("[Cauldron] Stirring progress increased");
        if (progress >= progressMax)
        {
            Debug.Log("[Cauldron] Stirring progress reached max");
            data.ShowIngredients();
            TryProcessCauldron();
        }
            
    }
    #endregion

    #region API
    public void ReceiveSolid(SolidObject sObj)
    {
        if (!sObj) return;
        //var ing = sObj.GetIngredient() as ItemStack ?? default;
        //ItemStack ing = sObj.GetIngredient() as ItemStack;
        float before = data.GetTotalAmount();
        
        ItemStack ing = (ItemStack)sObj.GetIngredient();

        var stack = ing;

        if (stack.IsEmpty) return;
        
        Debug.Log($"[Cauldron] Inserting a solid object with: {stack.Def.GetId()}");
        
        data.InsertSolid(stack);
        Destroy(sObj.gameObject);
        
        float after = data.GetTotalAmount();
        if (after > 0f) progress *= before / after;
        
        AfterInsert();
    }

    public float ReceiveLiquid(FluidStack fluid)
    {
        if (fluid.IsEmpty) 
            return 0;
        
        Debug.Log($"[Cauldron] Inserting fluid of: {fluid.Def.GetId()}");
        
        float before = data.GetTotalAmount();
        float remain = data.InsertLiquid(fluid);
        float after = data.GetTotalAmount();
        if (after > 0f)
            progress *= before / after;

        AfterInsert();
        return remain;
    }

    
    public void TryScoopLiquid(IFluidContainer container)
    {
        //if (!container) return;

        if (!data.TryGetOnlyFluid(out var cauldronFluid))
        {
            Debug.Log("[CauldronObj] Cannot scoop: none or multiple liquids");
            return;
        }
        if (!container.CanAccept(cauldronFluid))
        {
            Debug.Log("[Cauldron] Container can't accept this fluid");
            return;
        }

        float capacityLeft = container.Capacity - container.CurrentFluid.volume;
        if (capacityLeft <= 0f)
        {
            Debug.Log("[Cauldron] Container is full");
            return;
        }
        
        //float take = Mathf.Min(capacityLeft, cauldronVol);
        FluidStack take = data.TakeFluid(cauldronFluid);
        float took = take.volume;
        take.volume = container.Fill(take);
        data.InsertLiquid(take);

        UpdateTargetColor();
        //liquidPlane.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", data.CalculateFluidColor());
        
        Debug.Log($"[Cauldron] Scooped {took}, container now has {container.CurrentFluid.volume}");
    }
    #endregion

    #region PRIVATE
    private void AfterInsert()
    {
        UpdateTargetColor();
        
        float total = data.GetTotalAmount();
        if (total > maxCapacity)
        {
            HandleExplosion(total - maxCapacity);
            return;
        }

        //data.ShowIngredients();
        
        /*
        bool any = false;
        while (data.TryProcessOnce(out var solids, out var liquids))
        {
            any = true;
            if (solids != null)
                foreach (var s in solids) SpawnSolid(s);
            if (liquids != null)
            {
                liquidPlane.GetComponent<MeshRenderer>().material = liquids[0].Def.GetMaterial();
            }
        }
        if (any) Debug.Log("[CauldronObj] Recipes processed.");
        */
    }
    
    private void TryProcessCauldron()
    {
        /*
        int guard = 0;
        bool anyRecipe = false;

        while (data.TryProcessOnce(out var solids, out var liquids))
        {
            if (++guard > 200) { Debug.LogError("Loop runaway"); break; }
            anyRecipe = true;

            if (solids != null)
                foreach (var s in solids) SpawnSolid(s);
            
            /*
            if (liquids != null)
                foreach (var l in liquids) data.InsertLiquid(l);
                */
/*
            if (liquids != null && liquids.Count > 0)
                liquidPlane.GetComponent<MeshRenderer>().material = liquids[0].Def.GetMaterial();
        }
    */
        bool anyRecipe = data.TryProcessOnce(out var solids, out var liquids, out bool violatePower);
        if (anyRecipe)
        {
            if (solids != null)
                foreach (var s in solids) SpawnSolid(s);

            if (liquids != null && liquids.Count > 0)
                if (liquids[0].Def.GetMaterial() != null)
                {
                    Color.RGBToHSV(liquids[0].Def.GetMaterial().color, out float h, out float s, out float v);
        
                    float oldV = v * 100f;
                    float mappedV = oldV / 100f * 85f;
                    float finalV = mappedV / 100f;

                    Color newColor = Color.HSVToRGB(h, s, finalV);
                    UpdateCustomTargetColor(newColor);
                }
                else
                {
                    UpdateTargetColor();
                }
                //liquidPlane.GetComponent<MeshRenderer>().material = liquids[0].Def.GetMaterial();
                foreach (var potion in potionSounds)
                {
                    if (potion == liquids[0].Def)
                    {
                        AudioManager.instance.PlayVoiceLine(3, voiceLocation, potionSounds.IndexOf(potion));
                        AudioManager.instance.PlaySound("cauldron_success", gameObject);
                        break;
                    }
                }

            Debug.Log("[Cauldron] Recipe processed once");
        }
        else
        {
            AudioManager.instance.PlaySound("cauldron_failure", gameObject);
            if (violatePower)
                AudioManager.instance.PlayVoiceLine(1, voiceLocation);
            else
                AudioManager.instance.PlayVoiceLine(2, voiceLocation);

        }
        
        //if (anyRecipe) Debug.Log("[Cauldron] Recipes processed");

        // TODO Byproduct Logic
        if (!anyRecipe && !data.IsEmpty() && data.ByproductDef)
        {
            data.InsertLiquid(new FluidStack(data.ByproductDef, data.ByproductVol));
            UpdateCustomTargetColor(data.ByproductDef.GetMaterial().color);
            liquidPlane.GetComponent<MeshRenderer>().material = data.ByproductDef.GetMaterial();
            Debug.Log("[Cauldron] No recipe found, produced by-product");
        }

        progress = 0f;
    }

    private void SpawnSolid(ItemStack stack)
    {
        if (stack.IsEmpty || !stack.Def) return;
        int count = stack.amount;
        for (int i = 0; i < count; i++)
        {
            if (OnSpawnSolid != null)
            {
                OnSpawnSolid.Invoke(new ItemStack(stack.Def, 1));
                continue;
            }

            GameObject go = Instantiate(solidObjectPrefab);
            var so = go.GetComponent<SolidObject>();
            var rb = go.GetComponent<Rigidbody>();
            so.Init(stack);

            go.transform.position = outputPoint ? outputPoint.position : transform.position + Vector3.up * 2f;
            rb.AddForce(new Vector3(UnityEngine.Random.Range(-100,100), 10000, UnityEngine.Random.Range(-100,100)), ForceMode.Impulse);
        }

        Debug.Log($"[Cauldron] Spawned item {stack.Def.GetId()} x{count}");
    }

    private void HandleExplosion(float power)
    {
        Debug.Log($"[Cauldron] EXPLOSION! power={power}");
        OnExplodeVisual?.Invoke(power);
        data.HandleExplosion(power);
        //explosion.Play();
        explosion.Explode(targetColor);
        effects.Explode(power);
        UpdateTargetColor();
        SetFluidColor(data.CalculateFluidColor());
    }
    #endregion
}
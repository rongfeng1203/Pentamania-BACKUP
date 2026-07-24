using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FluidContainer : SolidObject, IFluidContainer
{
    [SerializeField] private FluidStack fluidIngredient;
    [SerializeField] private float capacity = 10f;
    [SerializeField] private float pourRateLps = 1f;
    [SerializeField] private float angleThreshold = 45f;
    [SerializeField] private ParticleSystem pourEffect;
    [SerializeField] private GameObject fluidDisplay;
    [SerializeField] private IngredientTagDef dust;
    [SerializeField] private Material fluidMat;
    [SerializeField] private Material dustMat;

    [SerializeField] private bool isPouring;
    
    private AudioSource audioSource;

    public AudioClip loopClip;


    //private bool isPouring;
    private CauldronLiquidReceiver receiverCache;

    public float Capacity => capacity;

    public FluidStack CurrentFluid => fluidIngredient;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }


    public FluidContainer(ItemStack ingredient,FluidStack fluidIngredient)
    {
        base.ingredient = ingredient;
        this.fluidIngredient = fluidIngredient;
    }

    private void UpdateFluidLevel()
    {
        fluidDisplay.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Fill", fluidIngredient.volume / capacity);
    }

    private new void Start()
    {
        base.Start();
        Init(fluidIngredient);
    }
    public void Init(FluidStack ingredient)
    {
        base.Init(base.ingredient);
        if (ingredient.Def == null) return;
        this.fluidIngredient = ingredient;
        // fluidDisplay.GetComponent<MeshRenderer>().sharedMaterial = ingredient.Def.GetMaterial();
        SetMaterial(ingredient);
        UpdateFluidLevel();
    }

    private void SetMaterial(FluidStack ingredient)
    {
        if (HasTag(ingredient.tags, dust))
        {
            fluidDisplay.GetComponent<MeshRenderer>().sharedMaterial = new Material(dustMat);

            Material source = ingredient.Def.GetMaterial();
            Material target = fluidDisplay.GetComponent<MeshRenderer>().sharedMaterial;

            string[] propertyNames = {
                "_BaseMap", "_BaseColor",
                "_MetallicMap", "_Smoothness",
                "_NormalMap", "_HeightMap",
                "_OcclusionMap", "_FillAmount"
            };

            foreach (var prop in propertyNames)
            {
                if (source.HasProperty(prop) && target.HasProperty(prop))
                {
                    if (prop == "_BaseColor")
                        target.SetColor(prop, source.GetColor(prop));
                    else if (prop == "_Smoothness" || prop == "_FillAmount")
                        target.SetFloat(prop, source.GetFloat(prop));
                    else if (source.GetTexture(prop) != null)
                        target.SetTexture(prop, source.GetTexture(prop));
                }
            }

        }
        else
        {
            // fluidDisplay.GetComponent<MeshRenderer>().sharedMaterial = new Material(fluidMat);

            // Material source = ingredient.Def.GetMaterial(ingredient.isInverted);
            // Material target = fluidDisplay.GetComponent<MeshRenderer>().sharedMaterial;

            // string[] propertyNames = {
            //     "_BaseMap", "_BaseColor",
            //     "_MetallicMap", "_Smoothness",
            //     "_NormalMap", "_HeightMap",
            //     "_OcclusionMap", "_FillAmount"
            // };

            // foreach (var prop in propertyNames)
            // {
            //     if (source.HasProperty(prop) && target.HasProperty(prop))
            //     {
            //         if (prop == "_BaseColor")
            //             target.SetColor(prop, source.GetColor(prop));
            //         else if (prop == "_Smoothness" || prop == "_FillAmount")
            //             target.SetFloat(prop, source.GetFloat(prop));
            //         else if (source.GetTexture(prop) != null)
            //             target.SetTexture(prop, source.GetTexture(prop));
            //     }
            // }
            
            fluidDisplay.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", CalculateFluidColor(ingredient));
        }
        UpdateFluidLevel();
    }

    private Color CalculateFluidColor(FluidStack ingredient)
    {
        Color totalColor = Color.black;
        float totalWeight = 0f;

        if (ingredient.IsEmpty || ingredient.tags == null) return totalColor;

        foreach (var tag in ingredient.tags)
        {
            if (tag.ingredientTagDef == null)
                continue;
            float weight = tag.value * ingredient.volume * tag.ingredientTagDef.GetColorWeight();
            totalColor += tag.GetColor() * weight;
            totalWeight += weight;
        }
        
        Color finalColor = totalWeight > 0 ? totalColor / totalWeight : Color.grey;
        
        Color.RGBToHSV(finalColor, out float h, out float s, out float v);
        
        float oldV = v * 100f;
        float mappedV = oldV / 100f * 80f;
        float finalV = mappedV / 100f;

        Color newColor = Color.HSVToRGB(h, s, finalV);
        Debug.Log(newColor.ToString());
        return newColor;
    }

    public static bool HasTag(List<IngredientTag> tags, IngredientTagDef targetDef)
    {
        if (tags == null || targetDef == null)
            return false;

        foreach (var tag in tags)
        {
            if (tag.ingredientTagDef == targetDef)
                return true;
        }

        return false;
    }

    public override IngredientStack GetIngredient()
    {
        return base.ingredient;
    }
    
    public IngredientStack GetFluidIngredient()
    {
        return fluidIngredient;
    }
    
    public FluidStack GetRealFluidIngredient()
    {
        return fluidIngredient;
    }

    public void SetFluidIngredient(IngredientStack ingredient)
    {
        if (ingredient is FluidStack fluidIngredient)
        {
            if (fluidIngredient.Def == null) return;
            // handle logic for the change in ingredient
            this.fluidIngredient = fluidIngredient;
            // TODO:
            // change liquid color to fluidIngredient.Def.GetMaterial();
            // fluidDisplay.GetComponent<MeshRenderer>().material = fluidIngredient.Def.GetMaterial();
            SetMaterial(fluidIngredient);
            UpdateFluidLevel();
        }
        else
        {
            // throw some error
        }
    }
    
    public void StartPour()
    {
        if (isPouring) return;
        isPouring = true;
        var main = pourEffect.main;
        main.startColor = fluidDisplay.GetComponent<Renderer>().sharedMaterial.GetColor("_BaseColor");
        if (pourEffect) pourEffect.Play();
        Debug.Log("[Container] Started pouring");
        // AudioManager.instance.PlaySound("water_pour_start", gameObject);
        
        if (loopClip == null) return;
        audioSource.clip = loopClip;
        audioSource.Play();
    }

    public void StopPour()
    {
        if (!isPouring) return;
        isPouring = false;
        if (pourEffect) pourEffect.Stop();
        Debug.Log("[Container] Stopped pouring");
        // AudioManager.instance.PlaySound("water_pour_stop", gameObject);
        
        audioSource.Stop();
        audioSource.clip = null;
    }

    /*
    public float Fill(FluidStack stack)
    {
        
        if (CanAccept(stack.Def))
        {
            ingredient.Merge(ref stack);
            return ingredient.volume;
        }
        else
        {
            throw new ArgumentException("The fluid stack must pass the CanMerge function to Merge with this fluid container.");
        }
    }
    */
    
    public float Fill(FluidStack stack)
    {
        if (stack.IsEmpty)
        {
            return 0;
        }
            
        if (!CanAccept(stack)) 
            return stack.volume;

        float room = capacity - fluidIngredient.volume;
        float toFill = Mathf.Min(room, stack.volume);

        if (fluidIngredient.IsEmpty)
            SetFluidIngredient(stack.CopyWithVolume(toFill));
        else
            fluidIngredient.volume += toFill;
        
        UpdateFluidLevel();

        return stack.volume - toFill;
    }

    /*
    public FluidStack Drain(int amount)
    {
        ingredient.volume -= amount;
        return ingredient;
    }
    */
    
    public FluidStack Drain(float amount)
    {
        if (fluidIngredient.IsEmpty || amount <= 0) 
            return new FluidStack(null, 0);

        float toDrain = Mathf.Min(fluidIngredient.volume, amount);
        
        FluidStack drained = fluidIngredient.CopyWithVolume(toDrain);
        
        fluidIngredient.volume -= toDrain;

        if (Mathf.Approximately(fluidIngredient.volume, 0f))
            fluidIngredient = new FluidStack(null, 0);

        UpdateFluidLevel();

        return drained;
    }
    
    public FluidStack TryPour()
    {
        if (fluidIngredient.IsEmpty ||
            Vector3.Angle(transform.up, Vector3.up) < angleThreshold)
        {
            StopPour();
            return FluidStack.Empty;
        }

        float requestVol = pourRateLps * Time.deltaTime;
        
        FluidStack poured = Drain(requestVol);
        
        if (!poured.IsEmpty) StartPour();
        if (fluidIngredient.IsEmpty) StopPour();

        return poured;
    }
    
    /*
    public bool CanAccept(FluidDef def)
    {
        return fluidIngredient.IsEmpty || fluidIngredient.Def.Equals(def);
    }
    */
    
    public bool CanAccept(FluidStack stack)
    {
        return fluidIngredient.IsEmpty || fluidIngredient.CanMerge(stack);
    }
}
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(CauldronObject))]
public class CauldronEffects : MonoBehaviour
{
    [SerializeField] private VisualEffect surfaceBubbleEffect;
    [SerializeField] private ParticleSystem fogEffect;
    [SerializeField] private ParticleSystem splashEffect;
    private CauldronObject cauldron;
    
    private void Start()
    {
        cauldron = GetComponent<CauldronObject>();
    }
    
    private void Update()
    {
        surfaceBubbleEffect.SetVector4("Base Color", cauldron.GetCurrentLiquidColor());
        surfaceBubbleEffect.SetFloat("Strength", cauldron.GetCurrentStrength());
        
        //fogEffect.main.startColor = cauldron.GetCurrentLiquidColor();
        Color fogColor = cauldron.GetCurrentLiquidColor();
        fogColor.a = cauldron.GetCurrentStrength() * 0.5f;
        var fog = fogEffect.main;
        fog.startColor = fogColor;
        
        
        
        Color splashColor = cauldron.GetCurrentLiquidColor();
        
        Color.RGBToHSV(splashColor, out float h, out float s, out float v);
        float oldV = v * 100f;
        float mappedV = oldV / 60f * 100f;
        float finalV = mappedV / 100f;
        Color newSplashColor = Color.HSVToRGB(h, s, finalV);
        newSplashColor.a = cauldron.GetCurrentStrength() * 0.08f;
        
        var splash = splashEffect.main;
        splash.startColor = newSplashColor;
        splash.startSpeed = new ParticleSystem.MinMaxCurve(cauldron.GetCurrentStrength() * 7f, splash.startSpeed.constantMax);
    }

    public void Explode(float power)
    {
        Debug.Log("111");
        surfaceBubbleEffect.Reinit();
    }
}
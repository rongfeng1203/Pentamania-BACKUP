using UnityEngine;
using UnityEngine.VFX;

public class CauldronEffectExplosion : MonoBehaviour
{
    [SerializeField] private VisualEffect explosionEffect;

    private bool hasExploded = false;

    public void Explode(Color inputColor)
    {
        /*
        if (hasExploded) return;
        hasExploded = true;
        */

        Gradient glow = new Gradient();
        glow.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0.0f),
                new GradientColorKey(inputColor, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(1.0f, 1.0f)
            }
        );

        Gradient secondary = new Gradient();
        secondary.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.gray, 0.0f),
                new GradientColorKey(inputColor * 0.8f, 0.4f),
                new GradientColorKey(inputColor, 0.8f),
                new GradientColorKey(Color.black, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.6f, 0.4f),
                new GradientAlphaKey(0.3f, 0.8f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );

        explosionEffect.SetGradient("GlowGradient", glow);
        explosionEffect.SetGradient("SecondaryGradient", secondary);

        //explosionEffect.Reinit();
        //explosionEffect.Play();
        explosionEffect.SendEvent("explode");

        Debug.Log("[CauldronExplosion] Boom!");
    }
}
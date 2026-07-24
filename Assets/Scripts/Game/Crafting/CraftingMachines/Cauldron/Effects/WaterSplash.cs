using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WaterSplash : MonoBehaviour
{
    public Transform ySource;
    public GameObject vfxPrefab;
    public CauldronObject cauldron;

    private void OnTriggerEnter(Collider collision)
    {
        GameObject other = collision.gameObject;

        if (other.GetComponent<XRGrabInteractable>() != null)
        {
            Vector3 hitPosition = other.transform.position;
            Vector3 spawnPosition = new Vector3(hitPosition.x, ySource.position.y, hitPosition.z);

            GameObject newObject = Instantiate(vfxPrefab, spawnPosition, Quaternion.identity);
            Color color = cauldron.GetCurrentLiquidColor();

            ParticleSystem[] allParticles = newObject.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in allParticles)
            {
                var main = ps.main;
                main.startColor = color;
            }
        }
    }
}
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class SoulFollowing : MonoBehaviour
{
    public Transform target;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public bool followPosition = true;
    public bool followRotation = true;

    private Vector3 initScale;

    private XRGrabInteractable grab;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrabbed);
        initScale = transform.localScale;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        if (followPosition)
            transform.position = target.position + target.TransformDirection(positionOffset);

        if (followRotation)
            transform.rotation = target.rotation * Quaternion.Euler(rotationOffset);
        
        transform.localScale = initScale;
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        if (grab != null)
            grab.selectEntered.RemoveListener(OnGrabbed);
    }
}
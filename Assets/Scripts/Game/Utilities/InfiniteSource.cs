using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[DisallowMultipleComponent]
public class InfiniteSourceList : MonoBehaviour
{
    public List<XRGrabInteractable> initialItems = new();

    public bool freezeOnStand = true;
    public bool forceDynamicWhenGrabbed = true;
    public RigidbodyConstraints frozenConstraints = RigidbodyConstraints.FreezeAll;
    public RigidbodyConstraints releasedConstraints = RigidbodyConstraints.None;
    public bool useGravityWhenGrabbed = true;

    private class Slot
    {
        public Vector3 pos;
        public Quaternion rot;
        public GameObject template;
        public XRGrabInteractable current;
    }

    private readonly Dictionary<XRGrabInteractable, Slot> grabToSlot = new();
    private readonly List<Slot> _slots = new();
    private Transform templateBucket;

    private void Awake()
    {
        templateBucket = new GameObject("[InfiniteSource_Templates]").transform;
        templateBucket.SetParent(transform, false);

        foreach (var item in initialItems)
        {
            if (!item) continue;

            var slot = new Slot
            {
                pos = item.transform.position,
                rot = item.transform.rotation,
                template = CreateCleanTemplate(item.gameObject)
            };

            PrepareAsCurrent(item, slot);
            _slots.Add(slot);
        }
    }

    private GameObject CreateCleanTemplate(GameObject src)
    {
        var t = Instantiate(src, templateBucket);
        t.name = src.name + "_Template";
        t.SetActive(false);

        if (t.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (t.TryGetComponent<XRGrabInteractable>(out var grab))
        {
            grab.enabled = true;
        }

        return t;
    }
    
    private void PrepareAsCurrent(XRGrabInteractable grab, Slot slot)
    {
        grab.transform.SetPositionAndRotation(slot.pos, slot.rot);

        Subscribe(grab);
        slot.current = grab;
        grabToSlot[grab] = slot;

        if (freezeOnStand) FreezeToStand(grab);
        else MakeDynamic(grab);
    }

    private void Subscribe(XRGrabInteractable grab)
    {
        grab.selectEntered.RemoveListener(OnSelectEntered);
        grab.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        var grabbed = args.interactableObject as XRGrabInteractable;
        if (!grabbed) return;
        if (!grabToSlot.TryGetValue(grabbed, out var slot)) return;

        if (forceDynamicWhenGrabbed)
            MakeDynamic(grabbed);

        var replacement = SpawnReplacement(slot);

        grabToSlot.Remove(grabbed);
        slot.current = replacement;
        grabToSlot[replacement] = slot;

        grabbed.selectEntered.RemoveListener(OnSelectEntered);
    }

    private XRGrabInteractable SpawnReplacement(Slot slot)
    {
        var go = Instantiate(slot.template, slot.pos, slot.rot, transform);
        go.name = slot.template.name.Replace("_Template", "") + "_Instance";
        go.SetActive(true);

        var grab = go.GetComponent<XRGrabInteractable>();
        if (!grab) grab = go.AddComponent<XRGrabInteractable>();

        if (!go.TryGetComponent<Rigidbody>(out var rb))
            rb = go.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (freezeOnStand)
            FreezeToStand(grab);

        Subscribe(grab);
        return grab;
    }

    private void FreezeToStand(XRGrabInteractable grab)
    {
        if (grab.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            // rb.useGravity = false;
            rb.constraints = frozenConstraints;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void MakeDynamic(XRGrabInteractable grab)
    {
        if (grab.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.constraints = releasedConstraints;
            rb.isKinematic = false;
            rb.useGravity = useGravityWhenGrabbed;
        }
    }
}
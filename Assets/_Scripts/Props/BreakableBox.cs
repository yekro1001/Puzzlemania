using UnityEngine;

public class BreakableBox : Interactable
{
    // inspector parameters
    [SerializeField]
    protected DoorButton hiddenButton;
    [SerializeField]
    protected float explosionForce = 10;
    [SerializeField]
    protected float explosionRadius = 1;

    // public properties
    public bool HasExploded { get => _hasExploded; }

    // private vars
    protected bool _hasExploded;

    protected override void Start()
    {
        base.Start();
        hiddenButton.Redisable();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Joint joint))
            {
                joint.breakForce = explosionForce * 0.75f;
            }
        }
    }

    protected override void Interact()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                rb.isKinematic = false;
            }
        }
        hiddenButton.Reenable();
        _hasExploded = true;
        _canBeInteractedWith = false;
        _outline.enabled = false;
        hintText.alpha = 0;
    }
}

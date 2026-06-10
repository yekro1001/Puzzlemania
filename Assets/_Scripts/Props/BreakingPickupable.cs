using UnityEngine;

public class BreakingPickupable : Pickupable
{
    [SerializeField]
    protected BreakableBox breakableBox;

    protected override void Interact()
    {
        base.Interact();
        if (!_isPickedUp)
        {
            breakableBox.Redisable();
        }
        else if (!breakableBox.HasExploded)
        {
            breakableBox.Reenable();
        }
    }
}

using UnityEngine;

public class DoorButton : Interactable
{
    // inspector parameters
    [SerializeField]
    protected Animator doorController;

    // readonly values
    protected static readonly int openHash = Animator.StringToHash("open");

    protected override void Interact()
    {
        doorController.SetTrigger(openHash);
        _canBeInteractedWith = false;
        _outline.enabled = false;
        hintText.alpha = 0;
    }
}

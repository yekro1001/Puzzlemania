using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    // instector parameters
    [SerializeField]
    protected TMP_Text hintText;
    [SerializeField]
    protected bool disableOnInit;
    
    // private vars
    protected Outline _outline;
    protected Collider _collider;
    protected InputActionMap _playerActionMap;
    protected InputAction _interactionAction;
    protected bool _canBeInteractedWith;

    protected virtual void OnEnable()
    {
        _playerActionMap = InputSystem.actions.FindActionMap("Player");
        _interactionAction = _playerActionMap.FindAction("Interact");
        _interactionAction.performed += InteractConditional;
    }

    protected virtual void OnDisable()
    {
        _interactionAction.performed -= InteractConditional;
    }

    protected virtual void Start()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
        hintText.alpha = 0;
        _canBeInteractedWith = !disableOnInit;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _canBeInteractedWith)
        {
            _outline.enabled = true;
            hintText.alpha = 1;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _outline.enabled = false;
            hintText.alpha = 0;
        }
    }

    protected virtual void InteractConditional(InputAction.CallbackContext context)
    {
        if (_outline.enabled)
        {
            Interact();
        }
    }

    protected abstract void Interact();

    public virtual void Reenable()
    {
        _canBeInteractedWith = true;
    }

    public virtual void Redisable()
    {
        _canBeInteractedWith = false;
    }
}

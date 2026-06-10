using UnityEngine;

public class Pickupable : Interactable
{
    // instector parameters
    [SerializeField]
    protected float elevatedDistance = 1;
    [SerializeField]
    protected float putDownDistance = 1;

    // private vars
    protected PlayerController _player;
    protected float _initialHeight;
    protected bool _isPickedUp;

    protected override void Start()
    {
        base.Start();
        hintText.text = "[E] Pick up";
        _initialHeight = transform.position.y;
    }

    protected override void Interact()
    {
        if (!_isPickedUp)
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            hintText.text = "[E] Put down";
        }
        else
        {
            hintText.text = "[E] Pick up";
            transform.position = new(_player.transform.position.x, _initialHeight, _player.transform.position.z);
            transform.Translate(putDownDistance * _player.transform.forward);
        }
        _isPickedUp = !_isPickedUp;
    }

    protected void Update()
    {
        if (_isPickedUp)
        {
            transform.position = _player.transform.position + elevatedDistance * Vector3.up;
        }
    }
}

using UnityEngine;

public class Pickupable : Interactable
{
    // instector parameters
    [SerializeField]
    private float elevatedDistance = 1;
    [SerializeField]
    private float putDownDistance = 1;

    // private vars
    private PlayerController _player;
    private float _initialHeight;
    private bool _isPickedUp;

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

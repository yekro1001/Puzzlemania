using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField]
    private Animator doorController;
    [SerializeField]
    private float sinkDistance = 0.075f;
    
    // readonly values
    private static readonly int openHash = Animator.StringToHash("open");
    private static readonly int closeHash = Animator.StringToHash("close");

    // private vars
    private int _objectsOnPlate;

    private void OnTriggerEnter(Collider other)
    {
        _objectsOnPlate++;
        if (_objectsOnPlate == 1)
        {
            doorController.SetTrigger(openHash);
            transform.Translate(sinkDistance * Vector3.down);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _objectsOnPlate--;
        if (_objectsOnPlate == 0)
        {
            doorController.SetTrigger(closeHash);
            transform.Translate(sinkDistance * Vector3.up);
        }
    }
}

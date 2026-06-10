using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    [SerializeField]
    private bool inverted;

    private void Update()
    {
        if (inverted)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
        else
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}

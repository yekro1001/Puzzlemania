using UnityEngine;

public class Remote : Interactable
{
    // inspector parameters
    [SerializeField]
    protected Renderer screen;
    [SerializeField]
    protected float emissionIntensity = 5;

    // readonly values
    protected static readonly int openHash = Animator.StringToHash("open");

    protected override void Interact()
    {
        screen.material.color *= emissionIntensity;
        foreach(EnemyController enemy in FindObjectsByType<EnemyController>())
        {
            enemy.DistractionLocation = screen.transform.position;
            enemy.EnemyState = EnemyState.Distracted;
        }
        _canBeInteractedWith = false;
        _outline.enabled = false;
        hintText.alpha = 0;
    }
}

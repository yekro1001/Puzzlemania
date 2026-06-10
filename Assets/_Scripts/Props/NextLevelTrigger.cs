using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _gameManager.CompleteLevel();
        }
    }
}

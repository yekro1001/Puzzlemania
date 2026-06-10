using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    // inspector parameters
    [SerializeField]
    private float runSpeedMultiplier = 1.5f;
    [SerializeField]
    private float wanderMaxDelay = 5;
    [SerializeField]
    private float wanderDistance = 5;
    [SerializeField]
    private float detectionDistance = 20;
    [SerializeField]
    private float fieldOfView = 60;

    // readonly values
    private static readonly int attackHash = Animator.StringToHash("attack");
    private static readonly int runHash = Animator.StringToHash("run");
    private static readonly int movingHash = Animator.StringToHash("moving");

    // public properties
    public EnemyState EnemyState
    {
        get => _enemyState;
        set => HandleStateTransition(_enemyState, value);
    }
    public Vector3 DistractionLocation { get; set; }

    // private vars
    private NavMeshAgent _agent;
    private Animator _animator;
    private EnemyState _enemyState;
    private PlayerController _player;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        StartCoroutine(nameof(WanderCoroutine));
    }

    private void Update()
    {
        if (!_player)
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        // handle behavior based on current state
        switch (EnemyState)
        {
            case EnemyState.Distracted:
                return;
            case EnemyState.Chase:
                _agent.SetDestination(_player.transform.position);
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    EnemyState = EnemyState.Attack;
                }
                break;
            case EnemyState.Wander:
            case EnemyState.Attack:
                if (!_player.GetComponent<NavMeshAgent>().enabled
                    && Vector3.Angle(_player.transform.position - transform.position, transform.forward) <= fieldOfView * 0.5f
                    && Physics.Raycast(transform.position, _player.transform.position - transform.position, out RaycastHit hit, detectionDistance)
                    && hit.transform == _player.transform)
                {
                    EnemyState = EnemyState.Chase;
                }
                break;
        }
    }

    private void HandleStateTransition(EnemyState oldState, EnemyState newState)
    {
        if (oldState == newState)
        {
            return;
        }

        // remove old state modifiers
        switch (oldState)
        {
            case EnemyState.Wander:
                StopCoroutine(nameof(WanderCoroutine));
                break;
            case EnemyState.Chase:
                _agent.speed /= runSpeedMultiplier;
                break;
        }

        // handle transition to new state
        switch (newState)
        {
            case EnemyState.Wander:
                StartCoroutine(nameof(WanderCoroutine));
                break;
            case EnemyState.Chase:
                _agent.speed *= runSpeedMultiplier;
                _animator.SetTrigger(runHash);
                break;
            case EnemyState.Attack:
                _player.GetAttacked();
                _animator.SetTrigger(attackHash);
                EnemyState = EnemyState.Wander;
                break;
            case EnemyState.Distracted:
                _animator.SetBool(movingHash, true);
                _agent.destination = DistractionLocation;
                break;

        }
        _enemyState = newState;
    }

    private IEnumerator WanderCoroutine()
    {
        while (true)
        {
            // periodically move to random point
            for (int i = 0; i < 9999; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderDistance;
                randomDirection += transform.position;
                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderDistance, NavMesh.AllAreas))
                {
                    _agent.SetDestination(hit.position);
                    break;
                }
            }
            _animator.SetBool(movingHash, true);
            yield return new WaitUntil(() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance);
            _agent.ResetPath();
            _animator.SetBool(movingHash, false);
            yield return new WaitForSeconds(Random.Range(0, wanderMaxDelay));
        }
    }
}

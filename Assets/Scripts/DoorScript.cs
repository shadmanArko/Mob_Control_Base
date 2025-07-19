using UnityEngine;
using DG.Tweening;

public class DoorScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _cloneAmount = 1;
    [SerializeField] private float _moveTime = 1f;
    [SerializeField] private Vector2 _moveRange = new Vector2(-1f, 1f);
    [SerializeField] private bool _isMoving = false;

    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    private void Start()
    {
        if (_isMoving)
        {
            StartMovement();
        }
    }

    private void StartMovement()
    {
        MoveRight();
    }

    private void MoveLeft()
    {
        transform.DOMoveX(_moveRange.x, _moveTime).OnComplete(MoveRight);
    }

    private void MoveRight()
    {
        transform.DOMoveX(_moveRange.y, _moveTime).OnComplete(MoveLeft);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController == null || playerController.cloneSource == gameObject) return;

        SpawnPlayerClones(playerController);
    }

    private void SpawnPlayerClones(PlayerController originalPlayer)
    {
        originalPlayer.cloneSource = gameObject;

        for (int i = 0; i < _cloneAmount - 1; i++)
        {
            Vector3 spawnPosition = CalculateRandomSpawnPosition(originalPlayer.transform.position);
            Instantiate(originalPlayer.gameObject, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 CalculateRandomSpawnPosition(Vector3 originalPosition)
    {
        float randomX = Random.Range(-4f, 4f);
        float randomZ = Random.Range(-2f, 2f);
        
        return new Vector3(
            originalPosition.x + randomX,
            originalPosition.y,
            originalPosition.z + randomZ
        );
    }
}
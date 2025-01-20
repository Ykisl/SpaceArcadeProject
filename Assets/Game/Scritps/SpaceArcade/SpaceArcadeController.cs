using UnityEngine;

public class SpaceArcadeController : MonoBehaviour
{
    [SerializeField] private Camera _gameCamera;
    [Space]
    [SerializeField] private Transform _gameRootCenter;
    [SerializeField] private float _gameAreaSize;
    [SerializeField] private Transform _playerShipRoot;

    public Camera GameCamera => _gameCamera;
    public Vector3 GameCenter => GetGameRootPosition();
    public Transform PlayerShipRoot => _playerShipRoot;
    public float GameAreaSize => _gameAreaSize;

    private Vector3 GetGameRootPosition()
    {
        var position = Vector3.zero;
        if(_gameRootCenter != null)
        {
            position = _gameRootCenter.position;
        }

        return position;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        var rootPosition = GetGameRootPosition();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(rootPosition, new Vector2(_gameAreaSize, 1));
    }

#endif

}

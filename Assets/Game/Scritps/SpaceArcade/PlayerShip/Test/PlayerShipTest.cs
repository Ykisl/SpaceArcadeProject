using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShipTest : MonoBehaviour
{
    [SerializeField] private PlayerShipController _playerShip;
    [Space]
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _fireAction;
    [Space]
    [SerializeField] private Transform _shipCenter;
    [SerializeField] private float _shipMoveAreaSize;

    private void Start()
    {
        var moveArea = new PlayerShipController.MoveArea()
        {
            Center = _shipCenter.position,
            Size = _shipMoveAreaSize
        };

        _playerShip.SetMoveArea(moveArea);

        var inputMap = new PlayerShipController.InputMap()
        {
            MoveInputAction = _moveAction.ToInputAction(),
            FireInputAction = _fireAction.ToInputAction()
        };

        inputMap.MoveInputAction.Enable();
        inputMap.FireInputAction.Enable();

        _playerShip.SetInputMap(inputMap);
    }
}

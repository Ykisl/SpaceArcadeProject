using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShipController : MonoBehaviour
{
    public class InputMap
    {
        public InputAction MoveInputAction;
        public InputAction FireInputAction;
    }

    public class MoveArea
    {
        public Vector3 Center;
        public float Size;
    }

    [SerializeField] private Transform _viewTransform;
    [Space]
    [SerializeField] private float _rotateAngle = 15f;
    [SerializeField] private float _rotationSpeed = 13f;
    [Space]
    [SerializeField] private List<Transform> _fireSources;

    private Transform _transform;
    private Vector3 _targetAngle = Vector3.zero;

    private InputMap _inputMap;
    private MoveArea _moveArea = new MoveArea();
    private float _moveSpeed = 0;

    public IList<Transform> FireSources => _fireSources;

    public event Action OnFire;

    public Transform GetTransform()
    {
        _transform ??= transform;
        return _transform;
    }

    private void OnEnable()
    {
        if (_inputMap != null)
        {
            SubscribeInputMap(_inputMap);
        }
    }

    private void OnDisable()
    {
        if (_inputMap != null)
        {
            UnsubscribeInputMap(_inputMap);
        }
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;

        UpdateInput(deltaTime);
        UpdateShipAngle(deltaTime);
    }

    #region Movement

    public void SetMoveArea(MoveArea moveArea)
    {
        _moveArea = moveArea;
        _moveArea ??= new MoveArea();
        
        ResetMovement();
    }

    public void SetMovementSpeed(float movementSpeed)
    {
        _moveSpeed = movementSpeed;
    }

    public void ResetMovement()
    {
        _targetAngle = Vector3.zero;

        var shipTransform = GetTransform();
        shipTransform.position = _moveArea.Center;
    }

    private void UpdateMovement(float deltaTime, Vector2 moveVector)
    {
        var shipTransform = GetTransform();

        var shipPosition = shipTransform.position;
        shipPosition.x += moveVector.x * _moveSpeed * deltaTime;

        var moveAreaHalfSize = _moveArea.Size / 2;
        shipPosition.x = Mathf.Clamp(shipPosition.x, _moveArea.Center.x - moveAreaHalfSize, _moveArea.Center.x + moveAreaHalfSize);

        shipTransform.position = shipPosition;
        _targetAngle.z = -moveVector.x * _rotateAngle;
    }

    private void UpdateShipAngle(float deltaTime)
    {
        _viewTransform.rotation = Quaternion.Euler(_targetAngle);
    }

    #endregion

    #region Actions

    private void FireBullet()
    {
        OnFire?.Invoke();
    }

    #endregion

    #region InputMap

    public void SetInputMap(PlayerShipController.InputMap inputMap)
    {
        if(_inputMap != null)
        {
            UnsubscribeInputMap(_inputMap);
        }

        _inputMap = inputMap;
        SubscribeInputMap(_inputMap);
    }

    private void SubscribeInputMap(InputMap inputMap)
    {
        if(inputMap == null)
        {
            return;
        }

        if(inputMap.FireInputAction != null)
        {
            inputMap.FireInputAction.performed += HandleInputFire;
        }
    }

    public void UnsubscribeInputMap(InputMap inputMap)
    {
        if (inputMap == null)
        {
            return;
        }

        if (inputMap.FireInputAction != null)
        {
            inputMap.FireInputAction.performed -= HandleInputFire;
        }
    }

    private void UpdateInput(float deltaTime)
    {
        if (_inputMap == null)
        {
            return;
        }

        if(_inputMap.MoveInputAction != null)
        {
            var moveVector = _inputMap.MoveInputAction.ReadValue<Vector2>();
            if(moveVector.magnitude > 0f)
            {
                UpdateMovement(deltaTime, moveVector);
            }
            else
            {
                _targetAngle = Vector3.zero;
            }
        }
    }

    private void HandleInputFire(InputAction.CallbackContext inputContext)
    {
        FireBullet();
    }

    #endregion

}

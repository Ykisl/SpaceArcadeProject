using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShipTargetPrototype : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveInputReference;
    [SerializeField] private Transform _targetRoot;
    [SerializeField] private Vector2 _targetFieldSize;

    private InputAction _moveInput;

    public event Action OnTargetMove;

    public Vector2 LocalTargetPosition
    {
        get
        {
            return new Vector2(transform.position.x / MathF.Abs(_targetFieldSize.x), transform.position.y / MathF.Abs(_targetFieldSize.y));
        }
    }

    private void Start()
    {
        _moveInput = _moveInputReference.ToInputAction();
        _moveInput.Enable();
    }

    private void OnDestroy()
    {
        _moveInput.Disable();
    }

    private void Update()
    {
        var moveInput = _moveInput.ReadValue<Vector2>();
        if(moveInput.magnitude > 0)
        {
            MovePlayerTarget(moveInput);
        }
    }

    private void MovePlayerTarget(Vector2 inputVector)
    {
        var newPos = transform.position;
        var halfSize = _targetFieldSize / 2;
        inputVector.y = 0;
        newPos += ((Vector3)(inputVector)) * 15f * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, _targetRoot.position.x - halfSize.x, _targetRoot.position.x + halfSize.x);
        newPos.y = Mathf.Clamp(newPos.y, _targetRoot.position.y - halfSize.y, _targetRoot.position.y + halfSize.y);
        transform.position = newPos;

        OnTargetMove?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(_targetRoot.position, _targetFieldSize);
    }
}

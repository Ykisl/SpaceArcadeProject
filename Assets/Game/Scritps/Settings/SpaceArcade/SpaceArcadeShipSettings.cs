using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SpaceArcadeShipSettings", menuName = "[SPA] Data/Settings/SpaceArcadeShipSettings")]
public class SpaceArcadeShipSettings : ScriptableObject
{
    [Header("Movement")]
    public float ShipMovementSpeed;
    [Header("Input")]
    public InputActionReference MoveInputAction;
    public InputActionReference FireInputAction;
}

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShipPrototype : MonoBehaviour
{
    [SerializeField] private PlayerShipTargetPrototype _target;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private GameObject _projectilePrefab;

    private List<Transform> _activePj = new List<Transform>();

    private void Start()
    {
        MoveToTarget();
        _target.OnTargetMove += HandleTargetMove;
    }

    private void OnDestroy()
    {
        _target.OnTargetMove -= HandleTargetMove;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }

        foreach(Transform t in _activePj)
        {
            t.position += t.forward * 50f * Time.deltaTime;
        }
    }

    private void Fire()
    {
        var createdGo = Instantiate(_projectilePrefab, _shootPoint.position, _shootPoint.rotation);
        createdGo.transform.LookAt(_target.transform, Vector3.up);
        _activePj.Add(createdGo.transform);
    }

    private void MoveToTarget()
    {
        var shipPos = transform.position;
        shipPos.x = _target.transform.position.x;
        //shipPos.y = _target.transform.position.y;
        transform.position = shipPos;

        //transform.LookAt(_target.transform, Vector3.up);
    }

    private void HandleTargetMove()
    {
        MoveToTarget();
    }
}

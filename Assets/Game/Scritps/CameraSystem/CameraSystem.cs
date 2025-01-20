using UnityEngine;
using Zenject;

public class CameraSystem : ICameraSystem
{
    private Camera _mainCamera;
    private Camera _activeCamera;

    private bool _isCameraEnabled = true;

    public Camera ActiveCamera => _activeCamera;

    public bool IsCameraEnabled => _isCameraEnabled;

    public CameraSystem(Camera mainCamera)
    {
        _mainCamera = mainCamera;
    }

    public void Initialize()
    {
        ResetCamera();
    }

    public void SetCamera(Camera newCamera)
    {
        DeactivateCamera(_mainCamera);

        _mainCamera = newCamera;
        if (_isCameraEnabled && _mainCamera != null)
        {
            ActivateCamera(_mainCamera);
        }
    }

    public void ResetCamera()
    {
        SetCamera(_mainCamera);
    }

    public void EnableCamera()
    {
        _isCameraEnabled = true;
        _activeCamera?.gameObject?.SetActive(true);
    }

    public void DisableCamera()
    {
        _isCameraEnabled = false;
        _activeCamera?.gameObject?.SetActive(false);
    }

    private void ActivateCamera(Camera camera)
    {
        if(camera == null)
        {
            return;
        }

        camera.gameObject.SetActive(true);
        camera.enabled = true;
    }

    private void DeactivateCamera(Camera camera)
    {
        if (camera == null)
        {
            return;
        }

        camera.gameObject.SetActive(false);
        camera.enabled = false;
    }
}

using UnityEngine;

public interface ICameraSystem
{
    Camera ActiveCamera { get; }
    bool IsCameraEnabled { get; }

    void Initialize();
    void SetCamera(Camera newCamera);
    void ResetCamera();

    void EnableCamera();
    void DisableCamera();

}

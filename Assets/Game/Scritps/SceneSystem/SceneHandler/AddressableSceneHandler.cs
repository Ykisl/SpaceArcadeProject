using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

public class AddressableSceneHandler : BaseSceneHandler
{
    private string _sceneName;

    private SceneInstance _sceneInstance;
    private bool _isLoading;
    private bool _isLoaded;
    private bool _isActive;

    public override bool IsLoading => _isLoading;
    public override bool IsLoaded => _isLoaded;
    public override bool IsActive => _isActive;

    public AddressableSceneHandler(string sceneName)
    {
        _sceneName = sceneName;
    }

    public override ICollection<GameObject> GetRootGameObjects()
    {
        if (!IsLoaded || !_sceneInstance.Scene.IsValid())
        {
            return null;
        }

        var scene = _sceneInstance.Scene;
        return scene.GetRootGameObjects();
    }

    public override async UniTask LoadScene(bool isAutoActiveScene = true)
    {
        if (IsLoaded)
        {
            return;
        }

        if (IsLoading)
        {
            await WaitForLoading();
            return;
        }

        if (string.IsNullOrEmpty(_sceneName))
        {
            return;
        }

        _isLoading = true;
        _isLoaded = false;
        _sceneInstance = await Addressables.LoadSceneAsync(_sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive, isAutoActiveScene);
        _isActive = isAutoActiveScene;

        _isLoaded = _sceneInstance.Scene.IsValid();
        _isLoading = false;
    }

    public override async UniTask UnloadScene()
    {
        if (!_sceneInstance.Scene.IsValid())
        {
            return;
        }

        if (IsLoading)
        {
            await WaitForLoading();
        }

        if (!IsLoaded)
        {
            return;
        }

        _isLoaded = false;
        _isLoading = false;
        _isActive = false;
        await Addressables.UnloadSceneAsync(_sceneInstance);
    }

    public override async UniTask ActivateScene()
    {
        if (!_sceneInstance.Scene.IsValid())
        {
            return;
        }

        if (IsLoading)
        {
            await WaitForLoading();
        }

        if (!IsLoaded)
        {
            return;
        }

        if (_isActive)
        {
            return;
        }

        await _sceneInstance.ActivateAsync();
        _isActive = true;
    }
}

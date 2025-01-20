using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class SceneSystem : ISceneSystem
{
    private List<ISceneHandler> _scenes;

    public async UniTask Initialize()
    {
        _scenes ??= new List<ISceneHandler>();
        if(_scenes.Count > 0)
        {
            await UnloadAll();
        }
    }

    public async UniTask<ISceneHandler> LoadScene(string sceneName, bool isAutoActiveScene = true)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return null;
        }

        var sceneHandler = new AddressableSceneHandler(sceneName);
        _scenes.Add(sceneHandler);
        await sceneHandler.LoadScene(isAutoActiveScene);

        return sceneHandler;
    }

    public async UniTask UnloadScene(ISceneHandler sceneHandler)
    {
        if(sceneHandler == null || _scenes == null)
        {
            return;
        }

        if (!_scenes.Contains(sceneHandler))
        {
            return;
        }

        await sceneHandler.UnloadScene();
        _scenes.Remove(sceneHandler);
    }

    public async UniTask UnloadAll()
    {
        if (_scenes == null || _scenes.Count <= 0)
        {
            return;
        }

        foreach(var scene in _scenes)
        {
            await scene.UnloadScene();
        }

        _scenes.Clear();
    }
}

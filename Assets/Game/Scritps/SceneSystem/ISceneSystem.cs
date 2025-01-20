using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneSystem
{
    UniTask Initialize();

    UniTask<ISceneHandler> LoadScene(string sceneName, bool isAutoActiveScene = true);
    UniTask UnloadScene(ISceneHandler sceneHandler);
    UniTask UnloadAll();
}

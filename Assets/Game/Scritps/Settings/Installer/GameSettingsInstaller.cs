using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "[SPA] Data/Settings/Installer")]
public class GameSettingsInstaller : ScriptableObjectInstaller
{
    [SerializeField] private List<ScriptableObject> _settings;

    public override void InstallBindings()
    {
        foreach(var setting in _settings)
        {
            Container.Bind(setting.GetType()).FromInstance(setting).AsSingle();
        }
    }
}

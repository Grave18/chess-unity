using Chess3D.Runtime.Utilities;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;
using VContainer.Unity;

namespace Chess3D.Runtime.Bootstrap
{
    public sealed class BootstrapScope : LifetimeScope
    {
        [SerializeField] private AudioMixer audioMixer;

        protected override void Awake()
        {
            DontDestroyOnLoad(this);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LoadingService>(Lifetime.Scoped);
            builder.Register<SceneManager>(Lifetime.Singleton);
            builder.Register<SettingsContainer>(Lifetime.Singleton).As<ISettingsContainer>();
            builder.Register<ConfigContainer>(Lifetime.Singleton);
            builder.Register<ApplicationConfigurationLoadUnit>(Lifetime.Scoped);
            builder.RegisterComponent(audioMixer);

            builder.RegisterEntryPoint<BootstrapFlow>();
        }
    }
}
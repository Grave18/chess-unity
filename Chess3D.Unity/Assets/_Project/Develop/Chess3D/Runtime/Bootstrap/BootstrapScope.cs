using Chess3D.Runtime.Utilities;
using VContainer;
using VContainer.Unity;

namespace Chess3D.Runtime.Bootstrap
{
    public class BootstrapScope : LifetimeScope
    {
        protected override void Awake()
        {
            DontDestroyOnLoad(this);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LoadingService>(Lifetime.Scoped);
            builder.Register<SceneManager>(Lifetime.Singleton);
            // builder.Register<ConfigContainer>(Lifetime.Singleton);

            builder.RegisterEntryPoint<BootstrapFlow>();
        }
    }
}
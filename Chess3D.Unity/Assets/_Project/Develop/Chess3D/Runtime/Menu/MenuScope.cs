using Chess3D.Runtime.Core;
using VContainer;
using VContainer.Unity;

namespace Chess3D.Runtime.Menu
{
    public class MenuScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {

            builder.RegisterBuildCallback(ServiceLocator.Initialize);

            builder.RegisterEntryPoint<MenuFlow>();
        }
    }
}
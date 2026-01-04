using Chess3D.Runtime.Core;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Online
{
    [Preserve]
    public class OnlineSceneManager
    {
        private readonly CoreScope _coreScope;

        public OnlineSceneManager(CoreScope coreScope)
        {
            _coreScope = coreScope;
        }

        public void LoadCorePrefab()
        {
           _coreScope.Build();
        }
    }
}
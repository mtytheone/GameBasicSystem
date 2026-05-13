using HatzeLaboratory.GameBasicSystem.Runtime.Core;

namespace HatzeLaboratory.GameBasicSystem.Runtime.System
{
    public abstract class GameSystemBase : SingletonBehaviour<GameSystemBase>
    {
        public bool IsInitialized { get; protected set; }
    }
}
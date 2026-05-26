using HatzeLaboratory.GameBasicSystem.Runtime.Core;

namespace HatzeLaboratory.GameBasicSystem.Runtime.System
{
    /// <summary>
    /// ゲームシステムの抽象シングルトン基底クラス。
    /// シーンをまたいで存在し続けるゲーム全体のシステムを実装する場合はこのクラスを継承してください。
    /// </summary>
    public abstract class GameSystemBase : SingletonBehaviour<GameSystemBase>
    {
        /// <summary>
        /// このシステムの初期化が完了しているかどうかを取得します。
        /// ゲームシステムの初期化が完了する際、<c>true</c> にセットしてください。
        /// </summary>
        public bool IsInitialized { get; protected set; }
    }
}
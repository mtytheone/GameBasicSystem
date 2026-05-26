namespace HatzeLaboratory.GameBasicSystem.Runtime.Pool.Interface
{
    /// <summary>
    /// オブジェクトプールで管理されるアイテムのインターフェース。
    /// <see cref="ObjectPool"/> で使用するオブジェクトはこのインターフェースを実装してください。
    /// </summary>
    public interface IObjectPoolItem
    {
        /// <summary>
        /// アイテムを表示状態にします。
        /// プールから取り出す際に呼ばれます。
        /// </summary>
        public void Show();

        /// <summary>
        /// アイテムを非表示状態にします。
        /// プールに返却する際に呼ばれます。
        /// </summary>
        public void Hide();
    }
}

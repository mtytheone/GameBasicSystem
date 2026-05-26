using System;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Runtime.UI.Interface
{
    /// <summary>
    /// モーダルUIのインターフェース。
    /// カスタムモーダルを作成する場合はこのインターフェースを実装してください。
    /// </summary>
    public interface IModal
    {
        /// <summary>
        /// このモーダルの GameObject
        /// </summary>
        public GameObject GameObject { get; }

        /// <summary>
        /// モーダルを表示
        /// </summary>
        public void Show();

        /// <summary>
        /// モーダルを非表示
        /// </summary>
        public void Hide();

        /// <summary>
        /// このモーダルの型
        /// </summary>
        /// <returns>モーダルの <see cref="System.Type"/></returns>
        public Type GetModalType();
    }
}
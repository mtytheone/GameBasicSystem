using UnityEditor.Build.Reporting;

namespace HatzeLaboratory.GameBasicSystem.Editor.BuildTool.Interface
{
    /// <summary>
    /// プレイヤービルドのカスタムタスクインターフェース。
    /// ビルド前後に処理を追加する場合はこのインターフェースを実装し、Project Settings > GameBasicSystem のタスクリストに登録してください。
    /// </summary>
    public interface IPlayerBuildTask
    {
        /// <summary>
        /// ビルド開始前に実行される処理
        /// </summary>
        /// <param name="buildType">実行するビルドの種類</param>
        public void RunPreprocess(PlayerBuildTool.BuildType buildType);

        /// <summary>
        /// ビルド完了後に実行される処理
        /// </summary>
        /// <param name="buildType">実行したビルドの種類</param>
        /// <param name="buildReport">ビルド結果のレポート</param>
        public void RunPostprocess(PlayerBuildTool.BuildType buildType, BuildReport buildReport);
    }
}

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Runtime.System
{
    /// <summary>
    /// プラットフォーム判定のユーティリティクラス。
    /// <see cref="RuntimePlatform"/> や <see cref="BuildTarget"/> から統一されたプラットフォーム名を取得できます。
    /// </summary>
    public static class PlatformDefine
    {
        /// <summary>
        /// プラットフォームの種類
        /// </summary>
        public enum PlatformType
        {
            /// <summary>
            /// macOS
            /// </summary>
            Mac,

            /// <summary>
            /// Windows
            /// </summary>
            Windows,

            /// <summary>
            /// iOS
            /// </summary>
            iOS,

            /// <summary>
            /// Android
            /// </summary>
            Android,

            /// <summary>
            /// Linux
            /// </summary>
            Linux,

            /// <summary>
            /// WebGL
            /// </summary>
            WebGL,

            /// <summary>
            /// Universal Windows Platform
            /// </summary>
            UWP,

            /// <summary>
            /// PlayStation4
            /// </summary>
            PS4,

            /// <summary>
            /// XboxOne
            /// </summary>
            XboxOne,

            /// <summary>
            /// Nintendo Switch
            /// </summary>
            Switch,

            /// <summary>
            /// PlayStation5
            /// </summary>
            PS5,

            /// <summary>
            /// Apple Vision Pro
            /// </summary>
            VisionOS,

            /// <summary>
            /// 上記以外の不明なプラットフォーム
            /// </summary>
            Unknown
        }

        /// <summary>
        /// <see cref="RuntimePlatform"/> から <see cref="PlatformType"/> の名前文字列を返します。
        /// </summary>
        /// <param name="platform">判定する RuntimePlatform</param>
        /// <returns><see cref="PlatformType"/> の名前文字列</returns>
        public static string GetPlatformName(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXServer:
                case RuntimePlatform.QNXArm32:
                case RuntimePlatform.QNXArm64:
                case RuntimePlatform.QNXX64:
                case RuntimePlatform.QNXX86:
                    return PlatformType.Mac.ToString();

                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsServer:
                    return PlatformType.Windows.ToString();

                case RuntimePlatform.IPhonePlayer:
                    return PlatformType.iOS.ToString();

                case RuntimePlatform.Android:
                    return PlatformType.Android.ToString();

                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.LinuxServer:
                    return PlatformType.Linux.ToString();

                case RuntimePlatform.WebGLPlayer:
                    return PlatformType.WebGL.ToString();

                case RuntimePlatform.WSAPlayerARM:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerX86:
                    return PlatformType.UWP.ToString();

                case RuntimePlatform.PS4:
                    return PlatformType.PS4.ToString();

                case RuntimePlatform.XboxOne:
                    return PlatformType.XboxOne.ToString();

                case RuntimePlatform.Switch:
                    return PlatformType.Switch.ToString();
                
                case RuntimePlatform.PS5:
                    return PlatformType.PS5.ToString();

                case RuntimePlatform.VisionOS:
                    return PlatformType.VisionOS.ToString();

                default:
                    return PlatformType.Unknown.ToString();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// <see cref="BuildTarget"/> から <see cref="PlatformType"/> の名前文字列を返します。
        /// </summary>
        /// <param name="platform">判定する BuildTarget</param>
        /// <returns><see cref="PlatformType"/> の名前文字列</returns>
        public static string GetPlatformName(BuildTarget platform)
        {
            switch (platform)
            {
                case BuildTarget.StandaloneOSX:
                    return PlatformType.Mac.ToString();

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return PlatformType.Windows.ToString();

                case BuildTarget.iOS:
                    return PlatformType.iOS.ToString();

                case BuildTarget.Android:
                    return PlatformType.Android.ToString();

                case BuildTarget.WebGL:
                    return PlatformType.WebGL.ToString();

                case BuildTarget.WSAPlayer:
                    return PlatformType.UWP.ToString();

                case BuildTarget.StandaloneLinux64:
                    return PlatformType.Linux.ToString();

                case BuildTarget.PS4:
                    return PlatformType.PS4.ToString();

                case BuildTarget.XboxOne:
                    return PlatformType .XboxOne.ToString();

                case BuildTarget.Switch:
                    return PlatformType.Switch.ToString();

                case BuildTarget.PS5:
                    return PlatformType.PS5.ToString();

                case BuildTarget.VisionOS:
                    return PlatformType.VisionOS.ToString();

                default:
                    return PlatformType.Unknown.ToString();
            }
        }

        /// <summary>
        /// <see cref="BuildTarget"/> から <see cref="PlatformType"/> の名前文字列の短縮文字列を返します。
        /// </summary>
        /// <param name="platform">判定する BuildTarget</param>
        /// <returns><see cref="PlatformType"/> の名前文字列の短縮文字列(3文字)</returns>
        public static string GetShortPlatformName(BuildTarget platform)
        {
            switch (platform)
            {
                case BuildTarget.StandaloneOSX:
                    return "Mac";

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Win";

                case BuildTarget.iOS:
                    return "iOS";

                case BuildTarget.Android:
                    return "And";

                case BuildTarget.WebGL:
                    return "WGL";

                case BuildTarget.WSAPlayer:
                    return "UWP";

                case BuildTarget.StandaloneLinux64:
                    return "Lnx";

                case BuildTarget.PS4:
                    return "PS4";

                case BuildTarget.XboxOne:
                    return "Xb1";

                case BuildTarget.Switch:
                    return "Swi";

                case BuildTarget.PS5:
                    return "PS5";

                case BuildTarget.VisionOS:
                    return "Vis";

                default:
                    return "Unk";
            }
        }
#endif
    }
}

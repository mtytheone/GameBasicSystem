using HatzeLaboratory.GameBasicSystem.Editor.Addressable;
using HatzeLaboratory.GameBasicSystem.Editor.BuildTool;
using HatzeLaboratory.GameBasicSystem.Editor.BuildTool.Interface;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Reporting;

namespace HatzeLaboratory.GameBasicSystem.Editor.BuildTool.Task
{
    public class AddressableBuildTask : IPlayerBuildTask
    {
        private const string DEVELOPMENT_KEYWORD = "Development";


        private readonly List<AddressableAssetGroup> _developmentGroupBackupList = new();
        private readonly List<DevelopmentEntryInfo> _developmentEntryBackupList = new();


        void IPlayerBuildTask.RunPreprocess(PlayerBuildTool.BuildType buildType)
        {
            if (buildType == PlayerBuildTool.BuildType.Release)
            {
                RemoveDevelopmentAssets();
            }

            AddressableBuildTool.BuildAddressableAssets();
            AddressableBuildTool.AddAddressableAssetsInStreamingDirectory();
        }

        void IPlayerBuildTask.RunPostprocess(PlayerBuildTool.BuildType buildType, BuildReport buildReport)
        {
            AddressableBuildTool.RemoveAddressableAssetsInStreamingDirectory();
            if (buildType == PlayerBuildTool.BuildType.Release)
            {
                RevertAddressableAssets();
            }
        }

        private void RemoveDevelopmentAssets()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            foreach (AddressableAssetGroup group in settings.groups)
            {
                RemoveDevelopmentAddressableGroup(group);
                foreach (AddressableAssetEntry entry in group.entries.ToList())
                {
                    RemoveDevelopmentAddressableEntry(entry, group);
                }
            }
        }

        private void RemoveDevelopmentAddressableGroup(AddressableAssetGroup group)
        {
            BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
            if (!schema)
            {
                return;
            }

            if (group.Name.Contains(DEVELOPMENT_KEYWORD))
            {
                _developmentGroupBackupList.Add(group);
                schema.IncludeInBuild = false;
            }
        }

        private void RemoveDevelopmentAddressableEntry(AddressableAssetEntry entry, AddressableAssetGroup group)
        {
            if (!entry.address.Contains(DEVELOPMENT_KEYWORD))
            {
                return;
            }

            _developmentEntryBackupList.Add(new DevelopmentEntryInfo
            {
                Entry = entry,
                ParentGroup = group,
                Address = entry.address,
            });

            group.RemoveAssetEntry(entry);
        }

        private void RevertAddressableAssets()
        {
            foreach (AddressableAssetGroup developmentGroup in _developmentGroupBackupList)
            {
                RevertAddressableGroup(developmentGroup);
            }

            foreach (DevelopmentEntryInfo developmentEntryInfo in _developmentEntryBackupList)
            {
                RevertAddressableEntry(developmentEntryInfo);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void RevertAddressableGroup(AddressableAssetGroup group)
        {
            BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
            if (schema)
            {
                schema.IncludeInBuild = true;
            }
        }

        private void RevertAddressableEntry(DevelopmentEntryInfo developmentEntryInfo)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(developmentEntryInfo.Entry.guid, developmentEntryInfo.ParentGroup);
            entry.SetAddress(developmentEntryInfo.Entry.address);
        }


        private struct DevelopmentEntryInfo
        {
            public AddressableAssetEntry Entry;
            public AddressableAssetGroup ParentGroup;
            public string Address;
        }
    }
}

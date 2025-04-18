using Colossal.IO.AssetDatabase;
using Colossal.PSI.Environment;
using Game.Buildings;
using Game.Modding;
using Game.Objects;
using Game.SceneFlow;
using Game.Settings;
using Game;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using Unity.Entities;
using UnityEngine.Device;

namespace ReplaceThatPrefab
{
    [FileLocation(nameof(ReplaceThatPrefab))]
    [SettingsUITabOrder(MainTab, AboutTab)]
    [SettingsUIGroupOrder(MainGroup, InfoGroup)]
    public class Setting(IMod mod) : ModSetting(mod)
    {
        public const string MainTab = "Main";
        public const string MainGroup = "Main";

        public const string AboutTab = "About";
        public const string InfoGroup = "Info";

        private static readonly PrefabReplaceSystem prs = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabReplaceSystem>();

        [SettingsUISection(MainTab, MainGroup)]
        public bool OpenConfig
        { set { Task.Run(() => Process.Start($"{EnvPath.kUserDataPath}/ModsData/{Mod.Name.Replace(" ", "")}.txt")); } }

        [SettingsUISection(MainTab, MainGroup)]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(IsNotInGame))]
        public bool StartBuildingReplacement
        {
            set
            {
                prs.Enabled = true;
                prs.StartReplacingBldg(ComponentType.ReadWrite<Building>());
            }
        }

        [SettingsUISection(MainTab, MainGroup)]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(IsNotInGame))]
        public bool StartStaticObjectReplacement
        {
            set
            {
                prs.Enabled = true;
                prs.StartReplacingBldg(ComponentType.ReadWrite<Static>());
            }
        }

        public override void SetDefaults()
        {
        }
        public bool IsNotInGame()
        {
            return (GameManager.instance.gameMode & GameMode.Game) == 0;
        }

        [SettingsUISection(AboutTab, InfoGroup)]
        public string NameText => Mod.Name;

        [SettingsUISection(AboutTab, InfoGroup)]
        public string VersionText => Mod.Version;

        [SettingsUISection(AboutTab, InfoGroup)]
        public string AuthorText => "StarQ";

        [SettingsUIButtonGroup("Social")]
        [SettingsUIButton]
        [SettingsUISection(AboutTab, InfoGroup)]
        public bool BMaCLink
        {
            set
            {
                try
                {
                    Application.OpenURL($"https://buymeacoffee.com/starq");
                }
                catch (Exception e)
                {
                    Mod.log.Info(e);
                }
            }
        }

    }
}

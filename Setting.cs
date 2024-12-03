using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using Unity.Entities;

namespace ReplaceThatPrefab
{
    [FileLocation(nameof(ReplaceThatPrefab))]
    //[SettingsUIGroupOrder(StartReplacementButtonGroup)]
    public class Setting(IMod mod) : ModSetting(mod)
    {
        //public const string ButtonSection = "";
        //public const string StartReplacementButtonGroup = "";
        private static readonly PrefabReplaceSystem prs = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabReplaceSystem>();

        //[SettingsUISection(ButtonSection, StartReplacementButtonGroup)]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(IsNotInGame))]
        public bool StartReplacementButton
        {
            set
            {
                prs.Enabled = true;
                prs.StartReplacing();
            }
        }
        
        public override void SetDefaults()
        {
        }
        public bool IsNotInGame()
        {
            return (GameManager.instance.gameMode & GameMode.Game) == 0;
        }

    }

    public class LocaleEN(Setting setting) : IDictionarySource
    {
        private readonly Setting m_Setting = setting;

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), Mod.m_Name },
                //{ m_Setting.GetOptionTabLocaleID(Setting.ButtonSection),Setting.ButtonSection},

                //{ m_Setting.GetOptionGroupLocaleID(Setting.StartReplacementButtonGroup), Setting.StartReplacementButtonGroup },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StartReplacementButton)), "Start Replacement" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StartReplacementButton)), "Start Replacement" },
            };
        }

        public void Unload()
        {

        }
    }
}

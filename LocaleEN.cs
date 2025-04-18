using Colossal;
using System.Collections.Generic;

namespace ReplaceThatPrefab
{
    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), Mod.Name },

                { m_Setting.GetOptionTabLocaleID(Setting.MainTab),Setting.MainTab},
                { m_Setting.GetOptionGroupLocaleID(Setting.MainGroup), Setting.MainGroup },

                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), Setting.AboutTab },
                { m_Setting.GetOptionGroupLocaleID(Setting.InfoGroup), Setting.InfoGroup },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenConfig)), "Open Configuration File" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenConfig)), "Open Configuration File" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StartBuildingReplacement)), "Start Building Prefab Replacement" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StartBuildingReplacement)), "Start Building Prefab Replacement" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StartStaticObjectReplacement)), "Start Static Object Prefab Replacement" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StartStaticObjectReplacement)), "Start Static Object Prefab Replacement" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameText)), "Mod Name" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameText)), "" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Mod Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)), "" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AuthorText)), "Author" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AuthorText)), "" },
            };
        }

        public void Unload()
        {

        }
    }
}

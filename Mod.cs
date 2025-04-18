﻿using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using System.Reflection;

namespace ReplaceThatPrefab
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(ReplaceThatPrefab)}").SetShowsErrorsInUI(false);
        public static Setting m_Setting;
        public static string Name = "Replace That Prefab";
        public static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        public void OnLoad(UpdateSystem updateSystem)
        {
            //log.Info(nameof(OnLoad));

            //if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            //log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));


            AssetDatabase.global.LoadSettings(nameof(ReplaceThatPrefab), m_Setting, new Setting(this));
            //World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabReplaceSystem>();
            updateSystem.UpdateAt<PrefabReplaceSystem>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose()
        {
            //log.Info(nameof(OnDispose));
            log.SetShowsErrorsInUI(true);
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}

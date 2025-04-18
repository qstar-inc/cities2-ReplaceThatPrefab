using Colossal.Entities;
using Colossal.PSI.Environment;
using Game.Common;
using Game.Prefabs;
using Game;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Unity.Collections;
using Unity.Entities;
using Game.Buildings;
using Game.Objects;

namespace ReplaceThatPrefab
{
    public partial class PrefabReplaceSystem : GameSystemBase
    {
        private PrefabSystem m_PrefabSystem;
        //private EntityQuery m_PlacedQuery;
        private readonly string separator = "-->";
        private readonly string commentor = "#";
        private readonly string filePath = EnvPath.kUserDataPath + "/ModsData/ReplaceThatPrefab.txt";

        protected override void OnCreate()
        {
            base.OnCreate();
            CreateTxt();
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
        }

        protected override void OnUpdate()
        {
        }

        private void CreateTxt()
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Mod.log.Info($"Directory {directoryPath} does not exist. Creating it.");
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, $"# Write the name of the current prefab name and then the prefab name you want it to change to.\n# Example:\n# Game.Prefabs.BuildingPrefab:School01{separator}Game.Prefabs.StaticObjectPrefab:Prop01\n# Lines including a \"{commentor}\" like this one will be ignored.\n# After saving this file, go into the save, then head over to \"Options\" > \"{Mod.Name}\" > \"Start XXXX Replacement\" where XXXX is the type of the object it is before replacement.");
            }
        }

        public void StartReplacing(int ct)
        {
            Enabled = true;
            Mod.log.Info("Starting replacer");
            Dictionary<string, string> nameDictionary = new();

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line) && line.Contains(separator) && !line.Contains(commentor))
                    {
                        var parts = line.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();
                            nameDictionary[key] = value;
                        }
                        else
                        {
                            Mod.log.Info($"Invalid line format: {line}");
                        }
                    }
                    else if(!line.Contains(commentor))
                    {
                        Mod.log.Info($"Skipping invalid line: {line}");
                    }
                }

                if (nameDictionary.Count == 0)
                {
                    Mod.log.Info("File is empty or contains no valid pairs.");
                }
                else
                {
                    Mod.log.Info($"Found {nameDictionary.Count} valid pairs to replace.");
                }
            }
            catch (Exception ex)
            {
                Mod.log.Info($"Error loading names from file: {ex.Message}");
            }

            if (nameDictionary.Count > 0)
            {
                try
                {
                    EntityQuery m_PlacedQuery;

                    switch (ct)
                    {
                        case 1:
                            m_PlacedQuery = SystemAPI.QueryBuilder().WithAllRW<Building>().Build();
                            break;
                        case 2:
                            m_PlacedQuery = SystemAPI.QueryBuilder().WithAllRW<Static>().Build();
                            break;
                        default:
                            return;
                    }

                    NativeArray<Entity> placedEntities = m_PlacedQuery.ToEntityArray(Allocator.Temp);
                    Mod.log.Info($"{placedEntities.Count()} items found for analyzing.");

                    CheckEntities(placedEntities, nameDictionary);
                }
                catch (Exception e)
                {
                    Mod.log.Info($"[ERROR]: {e}");
                }
            }
            //Enabled = false;
        }

        public void CheckEntities(NativeArray<Entity> placedEntities, Dictionary<string, string> nameDictionary)
        {
            int i = 0;
            int replaced = 0;
            int failed = 0;
            foreach (Entity placedEntity in placedEntities)
            {
                i++;
                bool fail = true;

                string log = $"Entity {i} of {placedEntities.Count()}: {placedEntity.Index}";

                EntityManager.TryGetComponent(placedEntity, out PrefabRef prefabRef);
                m_PrefabSystem.TryGetPrefab(prefabRef, out PrefabBase prefabBase);
                //Mod.log.Info();
                if (prefabBase != null)
                {
                    string prefabtype = (prefabBase.prefab.ToString() ?? "Prefabless").Replace(prefabBase.name, "").Replace(" (", "").Replace(")", "");
                    string prefabFullName = $"{prefabtype}:{prefabBase.name}";
                    log += $" | prefabFullName is {prefabFullName}";

                    if (nameDictionary.ContainsKey(prefabFullName))
                    {
                        string value = nameDictionary[prefabFullName];
                        log += $" || found in txt, replacing with {value}";
                        var parts = value.Split(':');
                        if (parts.Length == 2)
                        {
                            string replacingPrefabType = parts[0].Trim().Split('.').Last();

                            string replacingPrefabName = parts[1].Trim();
                            PrefabID newPrefabID = new(replacingPrefabType, replacingPrefabName);
                            m_PrefabSystem.TryGetPrefab(newPrefabID, out var toReplace);
                            //try
                            //{
                            //    log += $" ||-|| {toReplace.name}";
                            //}
                            //catch (Exception ex) { log += $" |XXXX| ERROR: {ex}"; } //Why null
                            if (toReplace != null)
                            {
                                m_PrefabSystem.TryGetEntity(toReplace, out Entity prefabEntity);
                                if (prefabEntity != null)
                                {
                                    EntityManager.SetComponentData(placedEntity, new PrefabRef(prefabEntity));
                                    EntityManager.AddComponent<Updated>(placedEntity);
                                    try
                                    {
                                        //var componentType = Type.GetType("Anarchy.Components.PreventOverride, AnarchyMod");
                                        //if (componentType != null)
                                        //{
                                        EntityManager.AddComponent<Anarchy.Components.PreventOverride>(placedEntity);
                                        //}
                                    }
                                    catch (Exception ex)
                                    {
                                        log += $" XXX {ex}";
                                    }
                                    replaced++;
                                    fail = false;
                                    log += $" ||| done";
                                }
                            }
                            else
                            {
                                log += $" |X| failed to find replacement prefab";
                            }
                            Mod.log.Info(log);
                        }
                    }
                }
                else
                {
                    log += $" | No prefabBase: {prefabRef.m_Prefab}";
                }

                if (fail == true)
                {
                    failed++;
                }
                //Mod.log.Info(log);
            }
            Mod.log.Info($"Finished processing {replaced} items...");
            if (failed > 0)
            {
                Mod.log.Info($"Failed {failed} items...");
            }
        }
    }
}
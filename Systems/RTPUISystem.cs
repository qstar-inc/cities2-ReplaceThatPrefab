using Colossal.Entities;
using Game.Areas;
using Game.Input;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Simulation;
using Game.Tools;
using Game.UI;
using Game.UI.Thumbnails;
using ReplaceThatPrefab.Domain.Enums;
using ReplaceThatPrefab.Domain.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace ReplaceThatPrefab.Systems
{
    public partial class RTPUISystem : ExtendedUISystemBase
    {
        private Entity workingEntity;
#nullable disable
        private ToolSystem toolSystem;
        private PrefabSystem prefabSystem;
        private RTPToolSystem rtpToolSystem;
        private DefaultToolSystem defaultToolSystem;
        private ValueBindingHelper<RTPToolMode> RTPMode;
        private ValueBindingHelper<Entity> SelectedEntity;
        private ValueBindingHelper<ObjectsInEntityUIBinder[]> ObjectsInEntity;
        private ProxyAction _toolKeyBinding;
#nullable enable
        public event Action? ConfigurationsUpdated;
        public RTPToolMode Mode { get => RTPMode; set => RTPMode.Value = value; }
        public Entity WorkingEntity => workingEntity;

        protected override void OnCreate()
        {
            base.OnCreate();

            _toolKeyBinding = Mod.m_Setting!.GetAction(nameof(Setting.ToolToggle));
            _toolKeyBinding.shouldBeEnabled = true;

            Mod.log.Info("RTPUISystem OnCreate");

            toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            rtpToolSystem = World.GetOrCreateSystemManaged<RTPToolSystem>();
            defaultToolSystem = World.GetOrCreateSystemManaged<DefaultToolSystem>();

            toolSystem.EventToolChanged += OnToolChanged;

            ObjectsInEntity = CreateBinding("GetObjectsInEntity", new ObjectsInEntityUIBinder[0]);
            RTPMode = CreateBinding("RTPToolMode", RTPToolMode.None);
            SelectedEntity = CreateBinding("SelectedEntity", Entity.Null);

            CreateTrigger("ToggleTool", () => ToggleTool());
            CreateTrigger("ObjectSelected", () => ObjectSelected());
            CreateTrigger("ClearTool", ClearTool);
        }

        protected override void OnUpdate()
        {
            if (_toolKeyBinding.WasPerformedThisFrame())
            {
                ToggleTool();
            }
            base.OnUpdate();
        }

        private void OnToolChanged(ToolBaseSystem system)
        {
            if (system is not RTPToolSystem)
            {
                RTPMode.Value = RTPToolMode.None;
            }
        }

        public void ToggleTool(bool? enable = null)
        {
            Mod.log.Info("RTPUISystem ToggleTool");
            if (enable == false || (enable is null && toolSystem.activeTool is RTPToolSystem))
            {
                ClearTool();
            }
            else
            {
                RTPMode.Value = RTPToolMode.Picker;

                toolSystem.selected = Entity.Null;
                toolSystem.activeTool = rtpToolSystem;

                //roadBuilderConfigurationsUISystem.UpdateConfigurationList();
            }
        }

        public void ClearTool()
        {
            Mod.log.Info("RTPUISystem ClearTool");
            RTPMode.Value = RTPToolMode.None;

            toolSystem.selected = Entity.Null;
            toolSystem.activeTool = defaultToolSystem;
        }

        public void ObjectSelected()
        {
            Entity entity = workingEntity;
            SelectedEntity.Value = entity;
            //Mod.log.Info(entity.ToString());

            List<ObjectsInEntityUIBinder> objects = new();
            ObjectsInEntity.Value = objects.ToArray();

            if (EntityManager.TryGetBuffer<Game.Objects.SubObject>(entity, false, out var subObjects))
            {
                for (var i = 0; i < subObjects.Length; i++)
                {
                    var subObject = subObjects[i];
                    Entity ent = subObject.m_SubObject;

                    if (EntityManager.TryGetComponent<PrefabRef>(ent, out var prefabRef) && prefabSystem.TryGetPrefab(prefabRef, out PrefabBase prefab))
                    {
                        string thumb = ImageSystem.GetIcon(prefab);
                        if (thumb == null || thumb == "")
                        {
                            thumb = ImageSystem.GetThumbnail(prefab);
                            if (thumb == null || thumb == "")
                            {
                                thumb = "Media/Placeholder.svg";
                            }
                        }

                        ObjectsInEntityUIBinder objectsInEntityUIBinder = new()
                        {
                            ID = ent.Index.ToString(),
                            Name = prefab.name,
                            Thumbnail = thumb,
                        };
                        //Mod.log.Info(prefab.name);
                        objects.Add(objectsInEntityUIBinder);
                    }

                    
                }
            }
            ObjectsInEntity.Value = objects.ToArray();
            //Mod.log.Info(objects.Length);
            ConfigurationsUpdated?.Invoke();
        }

        public void SetWorkingEntity(Entity entity, RTPToolMode mode)
        {
            workingEntity = entity;
            RTPMode.Value = mode;
            //Mod.log.Info($"workingEntity set: {entity.Index}");
        }
    }
}

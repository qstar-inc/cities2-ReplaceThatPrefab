using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Entities;
using Colossal.Entities;
using Colossal.Serialization.Entities;

using Game;
using Game.Common;
using Game.Input;
using Game.Net;
using Unity.Jobs;
using Game.SceneFlow;
using Game.UI.InGame;
using Unity.Collections;
using Game.UI.Editor;
using ReplaceThatPrefab.Systems;
using ReplaceThatPrefab.Domain.Enums;

using Game.Simulation;
using Colossal.Json;

namespace ReplaceThatPrefab.Systems
{
    public partial class RTPToolSystem : ToolBaseSystem
    {
#nullable disable
        private PrefabSystem prefabSystem;
        private RTPUISystem rtpUiSystem;
        private ToolSystem toolSystem;
        private Entity lastMarkedEntity;
        public List<Entity> objectList = new();
        private ProxyAction placeAction;
        private new ProxyAction applyAction;
        private new ProxyAction cancelAction;
#nullable enable

        private EntityQuery validQuery;
        public override string toolID => "RTPTool";

        public override PrefabBase GetPrefab()
        {
            return null;
        }

        public override bool TrySetPrefab(PrefabBase prefab)
        {
            return false;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Mod.log.Info("RTPToolSystem OnCreate");
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            rtpUiSystem = World.GetOrCreateSystemManaged<RTPUISystem>();
            toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            Enabled = false;


            applyAction = Mod.m_Setting!.GetAction(nameof(ReplaceThatPrefab) + "Apply");
            cancelAction = Mod.m_Setting!.GetAction(nameof(ReplaceThatPrefab) + "Cancel");

            var builtInApplyAction = InputManager.instance.FindAction(InputManager.kToolMap, "Apply");
            var mimicApplyBinding = applyAction.bindings.FirstOrDefault(b => b.device == InputManager.DeviceType.Mouse);
            var builtInApplyBinding = builtInApplyAction.bindings.FirstOrDefault(b => b.device == InputManager.DeviceType.Mouse);

            mimicApplyBinding.path = builtInApplyBinding.path;
            mimicApplyBinding.modifiers = builtInApplyBinding.modifiers;

            var builtInCancelAction = InputManager.instance.FindAction(InputManager.kToolMap, "Cancel");
            var mimicCancelBinding = cancelAction.bindings.FirstOrDefault(b => b.device == InputManager.DeviceType.Mouse);
            var builtInCancelBinding = builtInCancelAction.bindings.FirstOrDefault(b => b.device == InputManager.DeviceType.Mouse);

            mimicCancelBinding.path = builtInCancelBinding.path;
            mimicCancelBinding.modifiers = builtInCancelBinding.modifiers;

            InputManager.instance.SetBinding(mimicApplyBinding, out _);
            InputManager.instance.SetBinding(mimicCancelBinding, out _);

            validQuery = SystemAPI.QueryBuilder().WithAny<Highlighted>().WithAll<Game.Objects.SubObject>().Build();
        }

        public void ToggleTool(bool enable)
        {
            Mod.log.Info($"ToggleTool enabled: {enable}");
            if (enable && m_ToolSystem.activeTool != this)
            {
                m_ToolSystem.selected = Entity.Null;
                m_ToolSystem.activeTool = this;
            }
            else if (!enable && m_ToolSystem.activeTool == this)
            {
                m_ToolSystem.selected = Entity.Null;
                m_ToolSystem.activeTool = m_DefaultToolSystem;
            }

            Mod.log.Info($"ToggleTool ended");
        }

        protected override void OnStartRunning()
        {
            Mod.log.Info($"OnStartRunning");
            base.OnStartRunning();
            Mod.log.Info("Starting RTPToolSystem");

            //_modRaycastSystem.Enabled = true;
            //_validationSystem.Enabled = false;
        }

        protected override void OnGamePreload(Purpose purpose, GameMode mode)
        {
            base.OnGamePreload(purpose, mode);

            //if (mode == GameMode.Editor)
            //{
            //    if (editorToolUISystem?.tools?.Any(t => t?.id == toolID) ?? true)
            //    {
            //        return;
            //    }

            //    var tools = editorToolUISystem.tools;
            //    Array.Resize(ref tools, tools.Length + 1);
            //    tools[tools.Length - 1] = new RoadBuilderEditorTool(World, this, roadBuilderUISystem);
            //    editorToolUISystem.tools = tools;
            //}
        }

        public override void InitializeRaycast()
        {
            base.InitializeRaycast();
            
            m_ToolRaycastSystem.typeMask = TypeMask.All;
            m_ToolRaycastSystem.raycastFlags = RaycastFlags.BuildingLots | RaycastFlags.SubBuildings | RaycastFlags.SubElements ;
            m_ToolRaycastSystem.areaTypeMask = Game.Areas.AreaTypeMask.Lots;
            m_ToolRaycastSystem.netLayerMask = Layer.None;
            //m_ToolRaycastSystem.collisionMask = CollisionMask.OnGround | CollisionMask.;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            //placeAction.shouldBeEnabled = rtpUiSystem.Mode is RTPToolMode.Selected;
            applyAction.shouldBeEnabled = rtpUiSystem.Mode is RTPToolMode.Picker;
            cancelAction.shouldBeEnabled = true;

            if (cancelAction.WasPerformedThisFrame())
            {
                if (rtpUiSystem.Mode is RTPToolMode.Picker)
                {
                    applyAction.shouldBeEnabled = false;
                    rtpUiSystem.ClearTool();
                }
                else
                {
                    rtpUiSystem.Mode = RTPToolMode.Picker;
                }
                return base.OnUpdate(inputDeps);
            }

            switch (rtpUiSystem.Mode)
            {
                case RTPToolMode.Picker:
                    {
                        objectList.Clear();
                        var raycastHit = HandlePicker(out var entity);

                        HandleHighlight(validQuery, raycastHit ? x => x == entity : null);

                        if (raycastHit)
                        {
                            TryHighlightEntity(entity);
                        }
                        break;
                    }
                case RTPToolMode.Selected:
                    {
                        rtpUiSystem.ObjectSelected();
                        Enabled = false;
                        break;
                    }
            }
            return base.OnUpdate(inputDeps);

        }

        private void TryHighlightEntity(Entity entity)
        {
            if (entity != Entity.Null && !EntityManager.HasComponent<Highlighted>(entity))
            {
                lastMarkedEntity = entity;
                EntityManager.AddComponent<Highlighted>(entity);
                EntityManager.AddComponent<BatchesUpdated>(entity);
            }
            //Mod.log.Info(objectList.Count);
            //for (int j = 0; j < objectList.Count; j++)
            //{
            //    Entity obj = objectList[j];

            //    //if (!EntityManager.TryGetComponent<PrefabRef>(obj, out var prefabRef))
            //    //{
            //    //    string name = prefabSystem.GetPrefabName(prefabRef);
            //    //    Mod.log.Info(name);
            //    //}
            //}
        }

        private bool HandlePicker(out Entity entity)
        {
            if (!GetRaycastResult(out entity, out var hit))
            {
                return false;
            }

            if (!EntityManager.TryGetComponent<PrefabRef>(entity, out var prefabRef))
            {
                return false;
            }

            if (!prefabSystem.TryGetPrefab(prefabRef, out PrefabBase prefab))
            {
                return false;
            }

            if (!EntityManager.TryGetBuffer<Game.Objects.SubObject>(entity, false, out var subObjects))
            {
                return false;
            }

            for (var i = 0; i < subObjects.Length; i++)
            {
                var subObject = subObjects[i];
                Entity ent = subObject.m_SubObject;                
                objectList.Add(ent);
            }
            

            if (applyAction.WasPerformedThisFrame())
            {
                rtpUiSystem.SetWorkingEntity(entity, RTPToolMode.Selected);
            }

            return true;
        }

        private void HandleHighlight(EntityQuery query, Func<Entity, bool>? shouldBeHighlighted)
        {
            var entities = query.ToEntityArray(Allocator.Temp);
            //var editing = roadBuilderUISystem.Mode >= RoadBuilderToolMode.Editing;

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];

                if (shouldBeHighlighted != null && shouldBeHighlighted(entity))
                {
                    EntityManager.AddComponent<Highlighted>(entity);

                    //if (editing)
                    //{
                    //    EntityManager.AddComponent<RoadBuilderUpdateFlagComponent>(entity);
                    //}
                }
                else
                {
                    EntityManager.RemoveComponent<Highlighted>(entity);
                    //EntityManager.RemoveComponent<RoadBuilderUpdateFlagComponent>(entity);
                }

                EntityManager.AddComponent<BatchesUpdated>(entity);
            }
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();

            //applyAction.shouldBeEnabled = false;
            //cancelAction.shouldBeEnabled = false;

            var entities = validQuery.ToEntityArray(Allocator.Temp);

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];

                //EntityManager.RemoveComponent<RoadBuilderUpdateFlagComponent>(entity);
                EntityManager.RemoveComponent<Highlighted>(entity);
                EntityManager.AddComponent<BatchesUpdated>(entity);
            }
        }
    }
}

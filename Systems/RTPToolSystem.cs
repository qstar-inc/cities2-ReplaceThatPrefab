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

namespace ReplaceThatPrefab.Systems
{
    public partial class RTPToolSystem : ToolBaseSystem
    {
        private PrefabSystem prefabSystem;
        private EntityQuery highlightedQuery;
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
            Mod.log.Info("RTPToolSystem OnCreate");
            base.OnCreate();

            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            Enabled = false;
            highlightedQuery = SystemAPI.QueryBuilder().WithAll<Highlighted, Static>().Build();
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
            Mod.log.Info($"Starting {nameof(RTPToolSystem)}");

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
            Mod.log.Info($"InitializeRaycast");
            base.InitializeRaycast();

            m_ToolRaycastSystem.netLayerMask = Layer.All;
            m_ToolRaycastSystem.typeMask = TypeMask.Net;
            m_ToolRaycastSystem.collisionMask = CollisionMask.OnGround | CollisionMask.Overground;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Mod.log.Info($"OnUpdate");
            var raycastHit = HandlePicker(out var entity);

            HandleHighlight(highlightedQuery, raycastHit ? x => x == entity : null);

            if (raycastHit)
            {
                TryHighlightEntity(entity);
            }
            return base.OnUpdate(inputDeps);
        }
        private void TryHighlightEntity(Entity entity)
        {
            Mod.log.Info($"TryHighlightEntity");
            if (entity != Entity.Null && !EntityManager.HasComponent<Highlighted>(entity))
            {
                EntityManager.AddComponent<Highlighted>(entity);
                EntityManager.AddComponent<BatchesUpdated>(entity);
            }
        }

        private bool HandlePicker(out Entity entity)
        {
            Mod.log.Info($"HandlePicker");
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

            Mod.log.Info(prefab.name);

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

            var entities = highlightedQuery.ToEntityArray(Allocator.Temp);

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

using Anarchy.Extensions;
using Game.Input;
using Game.SceneFlow;
using Game.Simulation;
using Game.Tools;
using ReplaceThatPrefab.Domain.Enums;
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
        private ToolSystem toolSystem;
        private RTPToolSystem rtpToolSystem;
        private DefaultToolSystem defaultToolSystem;
        private ValueBindingHelper<RTPToolMode> RTPMode;

        public RTPToolMode Mode { get => RTPMode; set => RTPMode.Value = value; }

        protected override void OnCreate()
        {
            base.OnCreate();


            Mod.log.Info("RTPUISystem OnCreate");

            toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            rtpToolSystem = World.GetOrCreateSystemManaged<RTPToolSystem>();
            defaultToolSystem = World.GetOrCreateSystemManaged<DefaultToolSystem>();

            toolSystem.EventToolChanged += OnToolChanged;

            RTPMode = CreateBinding("RTPToolMode", RTPToolMode.None);
            CreateTrigger("ToggleTool", () => ToggleTool());
        }

        protected override void OnUpdate()
        {
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
                //RoadBuilderMode.Value = RoadBuilderToolMode.Picker;

                toolSystem.selected = Entity.Null;
                toolSystem.activeTool = rtpToolSystem;

                //roadBuilderConfigurationsUISystem.UpdateConfigurationList();
            }
        }

        public void ClearTool()
        {
            Mod.log.Info("RTPUISystem ClearTool");
            //RoadBuilderMode.Value = RoadBuilderToolMode.None;

            toolSystem.selected = Entity.Null;
            toolSystem.activeTool = defaultToolSystem;
        }
    }
}

import { bindValue, trigger } from "cs2/api";
// import { LaneSectionType } from "domain/LaneSectionType";
// import { NetSectionGroup } from "domain/NetSectionGroup";
// import { NetSectionItem } from "domain/NetSectionItem";
// import { OptionSection } from "domain/Options";
import { RTPToolModeEnum } from "domain/RTPToolMode";
// import { RoadConfiguration } from "domain/RoadConfiguration";
// import { RoadLane } from "domain/RoadLane";
import mod from "mod.json";

// export const allNetSections$ = bindValue<NetSectionGroup[]>(mod.id, "NetSections");
// export const allRoadConfigurations$ = bindValue<RoadConfiguration[]>(mod.id, "GetRoadConfigurations");
export const rtpToolMode$ = bindValue(mod.id, "RTPToolMode", RTPToolModeEnum.None);
// export const roadLanes$ = bindValue<RoadLane[]>(mod.id, "GetRoadLanes", []);
// export const roadOptions$ = bindValue<OptionSection[]>(mod.id, "GetRoadOptions");
// export const getRoadName$ = bindValue<string>(mod.id, "GetRoadName");
// export const getRoadSize$ = bindValue<string>(mod.id, "GetRoadSize");
// export const getRoadTypeName$ = bindValue<string>(mod.id, "GetRoadTypeName");
// export const getRoadId$ = bindValue<string>(mod.id, "GetRoadId");
// export const isPaused$ = bindValue<boolean>(mod.id, "IsPaused");
// export const fpsMeterLevel$ = bindValue<number>("app", "fpsMode");
// export const roadListView$ = bindValue<boolean>(mod.id, "RoadListView");
// export const IsCustomRoadSelected$ = bindValue<boolean>(mod.id, "IsCustomRoadSelected", false);
// export const DiscoverLoading$ = bindValue<boolean>(mod.id, "Discover.Loading", true);
// export const DiscoverErrorLoading$ = bindValue<boolean>(mod.id, "Discover.ErrorLoading", false);
// export const DiscoverUploading$ = bindValue<boolean>(mod.id, "Discover.Uploading", false);
// export const DiscoverCurrentPage$ = bindValue<number>(mod.id, "Discover.CurrentPage", 1);
// export const DiscoverMaxPages$ = bindValue<number>(mod.id, "Discover.MaxPages", 1);
// export const DiscoverItems$ = bindValue<RoadConfiguration[]>(mod.id, "Discover.Items");
// export const RestrictPlayset$ = bindValue<boolean>(mod.id, "Management.RestrictPlayset");
// export const managedRoad$ = bindValue<RoadConfiguration | undefined>(mod.id, "Management.ManagedRoad");
// export const managedRoadOptions$ = bindValue<OptionSection[]>(mod.id, "Management.GetRoadOptions");

export const toggleTool = trigger.bind(null, mod.id, "ToggleTool");
export const clearTool = trigger.bind(null, mod.id, "ClearTool");
// export const manageRoads = trigger.bind(null, mod.id, "ManageRoads");
// export const createNewPrefab = trigger.bind(null, mod.id, "CreateNewPrefab"); // create a new prefab from the selected one
// export const pickPrefab = trigger.bind(null, mod.id, "PickPrefab"); // create a new prefab from the selected one
// export const editPrefab = trigger.bind(null, mod.id, "EditPrefab"); // edit the selected prefab
// export const cancelActionPopup = trigger.bind(null, mod.id, "CancelActionPopup");
// export const duplicateLane = (index: number) => trigger(mod.id, "DuplicateLane", index);
// export const setRoadName = (name: string) => trigger(mod.id, "SetRoadName", name);
// export const setRoadListView = (active: boolean) => trigger(mod.id, "SetRoadListView", active);
// export const activateRoad = (id: string) => trigger(mod.id, "ActivateRoad", id);
// export const editRoad = (id: string) => trigger(mod.id, "EditRoad", id);
// export const findRoad = (id: string) => trigger(mod.id, "FindRoad", id);
// export const deleteRoad = (id: string) => trigger(mod.id, "DeleteRoad", id);
// export const setIsUIDragging = (isDragging: boolean) => trigger(mod.id, "SetDragging", isDragging);
// export const setSearchBinder = (q: string) => trigger(mod.id, "Lanes.SetSearchQuery", q);
// export const setRoadsSearchBinder = (q: string) => trigger(mod.id, "Roads.SetSearchQuery", q);
// export const setRoadLanes = (lanes: RoadLane[]) => {
//   trigger(
//     mod.id,
//     "SetRoadLanes",
//     lanes.filter((x) => !x.IsEdgePlaceholder)
//   );
// };
// export const laneOptionClicked = (optionIndex: number, netSectionId: number, optionId: number, value: number) =>
//   trigger(mod.id, "OptionClicked", optionIndex, netSectionId, optionId, value);
// export const roadOptionClicked = (netSectionId: number, optionId: number, value: number) =>
//   trigger(mod.id, "RoadOptionClicked", netSectionId, optionId, value);
// export const managedRoadOptionClicked = (netSectionId: number, optionId: number, value: number) =>
//   trigger(mod.id, "Management.RoadOptionClicked", netSectionId, optionId, value);
// export const setDiscoverPage = (p: number) => trigger(mod.id, "Discover.SetPage", p);
// export const setManagementSearchBinder = (q: string) => trigger(mod.id, "Management.SetSearchQuery", q);
// export const setManagementSetCategory = (s: number) => trigger(mod.id, "Management.SetCategory", s);
// export const setManagementRoad = (r: string) => trigger(mod.id, "Management.SetRoad", r);
// export const setManagedRoadName = (name: string) => trigger(mod.id, "Management.SetRoadName", name);
// export const setDiscoverSearchBinder = (q: string) => trigger(mod.id, "Discover.SetSearchQuery", q);
// export const setDiscoverSorting = (s: number) => trigger(mod.id, "Discover.SetSorting", s);
// export const downloadConfig = (id: string) => trigger(mod.id, "Discover.Download", id);
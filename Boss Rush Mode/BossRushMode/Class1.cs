using BepInEx;
using BepInEx.Configuration;
using GameDataEditor;
using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using TileTypes;
using System.Reflection.Emit;
using System.Reflection;
using I2.Loc;
using Random = System.Random;
using System.Collections;

namespace ExpertPlusMod
{
    [BepInPlugin(GUID, "Boss Rush Mod", version)]
    [BepInProcess("ChronoArk.exe")]
    public class ExpertPlusPlugin : BaseUnityPlugin
    {
        public const string GUID = "windy.bossrush";
        public const string version = "1.0.0";

        private static readonly Harmony harmony = new Harmony(GUID);

        private static ConfigEntry<bool> EasyCrimson;
        void Awake()
        {
            EasyCrimson = Config.Bind("Generation config", "Easy Crimson Wilderness", false, "Easy Crimson Wilderness\nReceive ??? for free in Bloody Park 1.\nReceive 2 less Soulstones, remove starting gold, and remove guaranteed shop key in Misty Garden 2 and Bloody Park 1.");
            harmony.PatchAll();
        }
        void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchAll(GUID);
        }

        // Modify gdata.json
        [HarmonyPatch(typeof(GDEDataManager), nameof(GDEDataManager.InitFromText))]
        class ModifyGData
        {
            static void Prefix(ref string dataString)
            {
                Dictionary<string, object> masterJson = (Json.Deserialize(dataString) as Dictionary<string, object>);
                foreach (var e in masterJson)
                {
                    // Remove all regular encounters
                    if (((Dictionary<string, object>)e.Value).ContainsKey("_gdeSchema"))
                    {
                        if (e.Key == "Stage1_1")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["EnemyNum"] = 0;
                            (masterJson[e.Key] as Dictionary<string, object>)["Event_EnemyNum"] = 0;
                        }
                        if (e.Key == "Stage1_2")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["EnemyNum"] = 0;
                            (masterJson[e.Key] as Dictionary<string, object>)["Event_EnemyNum"] = 0;
                        }
                        if (e.Key == "Stage2_1")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["EnemyNum"] = 0;
                            (masterJson[e.Key] as Dictionary<string, object>)["Event_EnemyNum"] = 0;
                        }
                        if (e.Key == "Stage2_2")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["EnemyNum"] = 0;
                            (masterJson[e.Key] as Dictionary<string, object>)["Event_EnemyNum"] = 0;
                        }
                        if (e.Key == "Stage3")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["EnemyNum"] = 0;
                            (masterJson[e.Key] as Dictionary<string, object>)["Event_EnemyNum"] = 0;
                        }
                        if (e.Key == "Stage4")
                        {
                            (masterJson[e.Key] as Dictionary<string, object>)["EnemyNum"] = 0;
                            (masterJson[e.Key] as Dictionary<string, object>)["Event_EnemyNum"] = 0;
                        }
                    }
                }
                dataString = Json.Serialize(masterJson);
            }
        }

        // Add rewards at start of stage
        [HarmonyPatch(typeof(FieldSystem))]
        class FieldSystem_Patch
        {
            [HarmonyPatch(nameof(FieldSystem.StageStart))]
            [HarmonyPostfix]
            static void StageStartPostfix()
            {

                List<ItemBase> rewards = new List<ItemBase>();

                if (StageSystem.instance.Map.StageData.Key == GDEItemKeys.Stage_Stage1_1)
                {
                    rewards.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Soul, 4));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    //InventoryManager.Reward(rewards);
                    foreach (ItemBase item in rewards)
                    {
                        PartyInventory.InvenM.AddNewItem(item);
                    }
                }
                else if (StageSystem.instance.Map.StageData.Key == GDEItemKeys.Stage_Stage1_2)
                {
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    if (EasyCrimson.Value)
                    {
                        rewards.RemoveAll(x => x.itemkey == GDEItemKeys.Item_Misc_Gold);
                        rewards.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Soul, 6));
                    }
                    else
                    {
                        rewards.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Soul, 8));
                    }
                    InventoryManager.Reward(rewards);
                }
                else if (StageSystem.instance.Map.StageData.Key == GDEItemKeys.Stage_Stage2_1)
                {
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    if (EasyCrimson.Value)
                    {
                        rewards.RemoveAll(x => x.itemkey == GDEItemKeys.Item_Misc_Gold);
                        rewards.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Soul, 7));
                        rewards.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_RWEnterItem));
                    }
                    else
                    {
                        rewards.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Soul, 9));
                    }
                    InventoryManager.Reward(rewards);
                }
                else if (StageSystem.instance.Map.StageData.Key == GDEItemKeys.Stage_Stage2_2)
                {
                    rewards.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Soul, 12));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    InventoryManager.Reward(rewards);
                }
                else if (StageSystem.instance.Map.StageData.Key == GDEItemKeys.Stage_Stage3)
                {
                    rewards.Add(ItemBase.GetItem(GDEItemKeys.Item_Misc_Soul, 12));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    rewards.AddRange(InventoryManager.RewardKey("Battle", false));
                    InventoryManager.Reward(rewards);
                }

                //Debug.Log(StageSystem.instance.Map.StageData.Key);
            }
        }

        // Easy Crimson - Remove keys
        [HarmonyPatch(typeof(FieldStore))]
        class FieldStore_Patch
        {
            [HarmonyPatch(nameof(FieldStore.Init))]
            [HarmonyPostfix]
            static void Postfix(FieldStore __instance)
            {
                if (EasyCrimson.Value)
                {
                    if (StageSystem.instance.Map.StageData.Key == GDEItemKeys.Stage_Stage1_2 || StageSystem.instance.Map.StageData.Key == GDEItemKeys.Stage_Stage2_1)
                    {
                        __instance.StoreItems.RemoveAll(x => x.itemkey == GDEItemKeys.Item_Misc_Item_Key);
                    }
                }
            }
        }

        // Easy Crimson - Disable Vending Machines
        [HarmonyPatch(typeof(HiddenDoor), "Start")]
        class Vending_Patch
        {
            static void Postfix(HiddenDoor __instance)
            {
                if (EasyCrimson.Value)
                {
                    __instance.MainEvob.Useless();
                }
            }
        }

        // Sanctuary - despawn all regular battles
        [HarmonyPatch(typeof(HexGenerator))]
        class HexGen_Patch
        {
            [HarmonyPatch(nameof(HexGenerator.FinalMap))]
            [HarmonyPrefix]
            static bool Prefix(ref HexMap __result)
            {
                    TileTypes.Event @event = new TileTypes.Event();
                    Start start = new Start();
                    Boss boss = new Boss();
                    MiniBoss miniBoss = new MiniBoss();
                    Block block = new Block();
                    BlockEvent blockEvent = new BlockEvent();
                    Objective objective = new Objective();
                    Monster monster = new Monster();
                    ItemEvent itemEvent = new ItemEvent();
                    TeleportTile teleportTile = new TeleportTile();
                    LightTile lightTile = new LightTile();
                    Store store = new Store();
                    HiddenWall hiddenWall = new HiddenWall();
                    HexMap hexMap = new HexMap();
                    hexMap.StageData = new GDEStageData(GDEItemKeys.Stage_Stage4);
                    int num = 30;
                    int num2 = 20;
                    hexMap.MapObject = new MapTile[num, num2];
                    for (int i = 0; i < num; i++)
                    {
                        for (int j = 0; j < num2; j++)
                        {
                            hexMap.MapObject[i, j] = new MapTile();
                        }
                    }
                    for (int k = 0; k < num; k++)
                    {
                        for (int l = 0; l < num2; l++)
                        {
                            hexMap.MapObject[k, l].Init(new Vector2((float)k, (float)l), hexMap);
                            if (k <= 1 || k >= num - 2 || l <= 1 || l >= num2 - 2)
                            {
                                hexMap.MapObject[k, l].Info.Type = new Border();
                            }
                            else
                            {
                                hexMap.MapObject[k, l].Info.Type = new Block();
                            }
                        }
                    }
                    List<Vector2Int> list = new List<Vector2Int>();
                    start.Add(ref hexMap, new Vector2Int(3, 10));
                    foreach (Vector2 vector in MapTile.MapRange(hexMap.MapObject[7, 10].Cube, 1, hexMap.Size))
                    {
                        hexMap.MapObject[(int)vector.x, (int)vector.y].Info.Type = new DebugRoad();
                    }
                    List<Vector2> list2 = new List<Vector2>();
                    hexMap.MapObject[5, 10].Info.Type = new DebugRoad();
                    hexMap.MapObject[7, 10].NeighborTile(Direction.North).Info.Type = new DebugRoad();
                    hexMap.MapObject[7, 10].NeighborTile(Direction.North).NeighborTile(Direction.North).Info.Type = new DebugRoad();
                    //monster.Add(ref hexMap, hexMap.MapObject[7, 10].NeighborTile(Direction.North).NeighborTile(Direction.North).NeighborTile(Direction.North).Pos);
                    hexMap.MapObject[7, 10].NeighborTile(Direction.North).NeighborTile(Direction.North).NeighborTile(Direction.North).NeighborTile(Direction.NorthEast).Info.Type = new DebugRoad();
                    hexMap.MapObject[7, 10].NeighborTile(Direction.North).NeighborTile(Direction.North).NeighborTile(Direction.North).NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).Info.Type = new DebugRoad();
                    hexMap.MapObject[7, 10].NeighborTile(Direction.North).NeighborTile(Direction.North).NeighborTile(Direction.North).NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).Info.Type = new DebugRoad();
                    list2.Add(hexMap.MapObject[7, 10].NeighborTile(Direction.North).NeighborTile(Direction.North).NeighborTile(Direction.North).NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).Pos);
                    hexMap.MapObject[7, 10].NeighborTile(Direction.South).Info.Type = new DebugRoad();
                    hexMap.MapObject[7, 10].NeighborTile(Direction.South).NeighborTile(Direction.South).Info.Type = new DebugRoad();
                    //monster.Add(ref hexMap, hexMap.MapObject[7, 10].NeighborTile(Direction.South).NeighborTile(Direction.South).NeighborTile(Direction.South).Pos);
                    hexMap.MapObject[7, 10].NeighborTile(Direction.South).NeighborTile(Direction.South).NeighborTile(Direction.South).NeighborTile(Direction.SouthEast).Info.Type = new DebugRoad();
                    hexMap.MapObject[7, 10].NeighborTile(Direction.South).NeighborTile(Direction.South).NeighborTile(Direction.South).NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).Info.Type = new DebugRoad();
                    hexMap.MapObject[7, 10].NeighborTile(Direction.South).NeighborTile(Direction.South).NeighborTile(Direction.South).NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).Info.Type = new DebugRoad();
                    list2.Add(hexMap.MapObject[7, 10].NeighborTile(Direction.South).NeighborTile(Direction.South).NeighborTile(Direction.South).NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).Pos);
                    hexMap.MapObject[7, 10].NeighborTile(Direction.NorthEast).Info.Type = new DebugRoad();
                    //monster.Add(ref hexMap, hexMap.MapObject[7, 10].NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).Pos);
                    hexMap.MapObject[7, 10].NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).Info.Type = new DebugRoad();
                    hexMap.MapObject[7, 10].NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).NeighborTile(Direction.SouthEast).Info.Type = new DebugRoad();
                    hexMap.MapObject[7, 10].NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).NeighborTile(Direction.SouthEast).NeighborTile(Direction.NorthEast).Info.Type = new DebugRoad();
                    list2.Add(hexMap.MapObject[7, 10].NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).NeighborTile(Direction.NorthEast).NeighborTile(Direction.SouthEast).NeighborTile(Direction.NorthEast).Pos);
                    hexMap.MapObject[7, 10].NeighborTile(Direction.SouthEast).Info.Type = new DebugRoad();
                    //monster.Add(ref hexMap, hexMap.MapObject[7, 10].NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).Pos);
                    hexMap.MapObject[7, 10].NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).Info.Type = new DebugRoad();
                    hexMap.MapObject[7, 10].NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).NeighborTile(Direction.NorthEast).Info.Type = new DebugRoad();
                    hexMap.MapObject[7, 10].NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).NeighborTile(Direction.NorthEast).NeighborTile(Direction.SouthEast).Info.Type = new DebugRoad();
                    list2.Add(hexMap.MapObject[7, 10].NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).NeighborTile(Direction.SouthEast).NeighborTile(Direction.NorthEast).NeighborTile(Direction.SouthEast).Pos);
                    list2 = Misc.Shuffle<Vector2>(list2);
                    TileTypes.Stage6Chest stage6Chest = new TileTypes.Stage6Chest();
                    Stage6ChestKey stage6ChestKey = new Stage6ChestKey();
                    for (int m = 0; m < list2.Count; m++)
                    {
                        hexMap.MapObject[(int)(list2[m].x - 2f), (int)list2[m].y].Info.Type = new DebugRoad();
                        if (m == 0 || m == 1)
                        {
                            //stage6Chest.Add(ref hexMap, list2[m]);
                            stage6Chest.EventWall(ref hexMap, list2[m]);
                        }
                        else if (m == 2)
                        {
                            Vector2 pos = list2[m];
                            for (int n = 0; n < 8; n++)
                            {
                                pos.x += 1f;
                                hexMap.MapObject[(int)pos.x, (int)pos.y].Info.Type = new DebugRoad();
                                if (n > 3)
                                {
                                    hexMap.MapObject[(int)pos.x, (int)pos.y - 1].Info.Type = new DebugRoad();
                                }
                                if (n > 5)
                                {
                                    hexMap.MapObject[(int)pos.x, (int)pos.y - 2].Info.Type = new DebugRoad();
                                }
                            }
                            boss.Add(ref hexMap, pos);
                        }
                    }
                    stage6ChestKey.Add(ref hexMap, new Vector2Int(2, 12));
                    stage6ChestKey.Add(ref hexMap, new Vector2Int(3, 12));
                    stage6ChestKey.Add(ref hexMap, new Vector2Int(4, 12));
                    store.Add(ref hexMap, new Vector2Int(6, 12));
                    foreach (MapTile mapTile in hexMap.EventTileList)
                    {
                        foreach (MapTile mapTile2 in hexMap.EventTileList)
                        {
                            if (mapTile != mapTile2 && !HexGenerator.Path(mapTile2.Pos, mapTile.Pos, hexMap))
                            {
                                HexGenerator.PathCreate(mapTile2.Pos, mapTile.Pos, ref hexMap);
                            }
                        }
                    }
                    __result = hexMap;
                    Debug.Log("asdfasdf");
                    return false;
            }
        }

        //[HarmonyPatch(typeof(BattleSystem), "BattleStart")]
        //class BattleSystem_Patch
        //{
        //    [HarmonyPostfix]
        //    static void StageStartPostfix()
        //    {

        //        Debug.Log(PlayData.BattleReward);
        //    }
        //}

    }
}



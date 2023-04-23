using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSE.Models
{
    public class GameSaveUtil
    {
        public static List<GameSave> GetGameSaves()
        {
            var saves = new List<GameSave>();

            // Retrieve files in save data directory
            var files = Directory.GetFiles(GlobalState.Instance.SaveSearchDir);

            int i = 1;

            // Enumerate save files
            foreach (var fileStr in files.Where(p => p.Contains(".save") && !p.Contains("Settings"))
                                            .OrderBy(p => p))
            {
                try
                {
                    // Parse to raw object
                    var str = File.ReadAllText(fileStr);
                    var json = JsonConvert.DeserializeObject<JArray>(File.ReadAllText(fileStr))?.First().ToObject<UnparsedGameSave>();

                    Log.Information($"Successfully deserialized {fileStr}!");

                    // Parse into something that we can modify and display
                    var gameSave = new GameSave();
                    gameSave.SaveName = $"Save {i}";
                    gameSave.GameCoins = int.Parse(json?.game_Coins ?? "-1");
                    gameSave.GameTimeSeconds = double.Parse(json?.game_Time ?? "-1.0") / 1000;
                    gameSave.HardmodePlayers = (HardmodeState)int.Parse(json?.game_HardMode ?? "-1");
                    gameSave.GameTutorialState = int.Parse(json?.game_Tutorial ?? "3");

                    var gameStages = new List<Stage>();

                    var stagesArePacifist = json?.stage_Clears_Pacifist?.Split("|");
                    var stagesWithCloudsCollected = json?.coins_Cloud?.Split("|");
                    var stagesWithCometShards = json?.coins_CometShard?.Split("|");
                    var stagesWithCometCollected = json?.coins_Comet?.Split("|");
                    var stageTimes = json?.stage_Times?.Split("|");
                    var stageClears = json?.stage_Clears?.Split("|");
                    var stageMoons = json?.coins_Moon?.Split("|");

                    Log.Information("Successfully split stage items!");

                    // Parse collectibles. They are deliminated by stage with |
                    // We are assuming they all have a length of 100
                    for (int x = 0; x < 100; x++)
                    {
                        var stage = new Stage();

                        stage.IsPacifist = (stagesArePacifist?[x] ?? "0") == "0" ? false : true;
                        stage.StageTimeSeconds = double.Parse(stageTimes?[x] ?? "0.0") / 1000;
                        stage.CloudState = (StageCollectableState)int.Parse(stagesWithCloudsCollected?[x] ?? "0");
                        stage.CometState = (StageCollectableState)int.Parse(stagesWithCometCollected?[x] ?? "0");

                        // Check stage cleared state, convert to enum
                        // Easier to manually parse it
                        if (stageClears != null)
                        {
                            stage.IsStageCleared = (StageClearedState)int.Parse(stageClears[x]);
                        }

                        // Comet shards are deliminated with | for per stage
                        // and then deliminated with - for shard in stage
                        if (stagesWithCometShards != null)
                        {
                            string[] shardNum = stagesWithCometShards[x].Split("-");
                            stage.ShardsCollected = shardNum.Select(p => int.Parse(p)).ToArray();
                        }

                        // Moons are deliminated with | for stage AND moon
                        // There can be 5 moons per stage, but can be unused, still mark as 0
                        if (stageMoons != null)
                        {
                            stage.MoonsCollected = stageMoons.Skip(5 * x).Take(5).Select(p => int.Parse(p)).ToArray();
                        }

                        stage.StageIndex = x;
                        gameStages.Add(stage);
                    }

                    gameSave.UnparsedGameSave = json;
                    gameSave.Stages = gameStages;

                    saves.Add(gameSave);

                    Log.Information($"Completed parsing save data for save {i}");

                    i++;
                } catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to parse save data for save {i}");
                }
            }
            
            return saves;
        }

        public static string ExportSaveFile(GameSave save)
        {
            var rawGameSaveExport = new UnparsedGameSave();

            try
            {
                rawGameSaveExport.game_Coins = save.GameCoins.ToString();
                rawGameSaveExport.game_HardMode = ((int)save.HardmodePlayers).ToString();
                rawGameSaveExport.game_Time = (save.GameTimeSeconds * 1000.0).ToString();
                rawGameSaveExport.game_Tutorial = save.GameTutorialState.ToString();

                rawGameSaveExport.stage_Players = save?.UnparsedGameSave?.stage_Players ?? "1|0";

                // Loop through and add stage items
                for (int i = 0; i < save?.Stages.Count; i++)
                {
                    var stage = save.Stages[i];

                    rawGameSaveExport.coins_Comet += ((int)stage.CometState).ToString();
                    rawGameSaveExport.coins_Cloud += ((int)stage.CloudState).ToString();
                    rawGameSaveExport.stage_Clears += ((int)stage.IsStageCleared).ToString();
                    rawGameSaveExport.stage_Times += (stage.StageTimeSeconds * 1000.0).ToString();
                    rawGameSaveExport.stage_Clears_Pacifist += (stage.IsPacifist ? 1 : 0).ToString();

                    // These two saves require a special format
                    rawGameSaveExport.coins_CometShard += string.Join("-", stage.ShardsCollected.Select(s => s.ToString()));
                    rawGameSaveExport.coins_Moon += string.Join("|", stage.MoonsCollected.Select(s => s.ToString()));

                    if (i + 1 != save.Stages.Count)
                    {
                        rawGameSaveExport.coins_Comet += "|";
                        rawGameSaveExport.coins_Cloud += "|";
                        rawGameSaveExport.coins_CometShard += "|";
                        rawGameSaveExport.coins_Moon += "|";
                        rawGameSaveExport.stage_Clears_Pacifist += "|";
                        rawGameSaveExport.stage_Clears += "|";
                        rawGameSaveExport.stage_Times += "|";
                    }
                }
            } catch (Exception ex)
            {
                Log.Error(ex, "Failed to parse save for export!");
            }

            
            return JsonConvert.SerializeObject(new List<UnparsedGameSave>() { rawGameSaveExport });
        }

        public static string[] StageNameLookupTable = 
        {
            "GUIDING GLADE",
            "HOPPET HEIGHTS",
            "BRIDGE-WHEEL WATERWAY",
            "SALAMANCER's SANCTUM",
            "BRAMBLES IN THE BREEZE",
            "OCTULENT'S ONSLAUGHT",
            "PRICKLY PEAL",
            "CAWBIE CLIFFS",
            "8",
            "PIPS AND PITS",
            "BEEVY BARRACKS",
            "OVER THE RAYBOW",
            "GRABBA'S GROTTO",
            "BEEVY BATTLEGROUND",
            "THE PIP SHIP",
            "15",
            "SNOWY SPRINT",
            "A TRACK IN TIME",
            "ABYSSAL ALGOR",
            "SLICKO SLIDE",
            "END OF THE RAYBOW",
            "21",
            "DIZZYING DESCENT",
            "SPICY ICE SPEEDWAY",
            "24",
            "LAVA PIKE POLDER",
            "MAGMAW WELL",
            "HONEYBUZZ BOILER",
            "VEXATIOUS VENTS",
            "SMOKY SQUALL",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "DROPDASH CHAPARRAL",
        };
    }
}

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
                    gameSave.HardmodePlayers = (HardmodeState)int.Parse(json?.game_Hardmode ?? "-1");
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

        public static void ExportSaveFile(GameSave save)
        {
            var rawGameSaveExport = new UnparsedGameSave();

            rawGameSaveExport.game_Coins = save.GameCoins.ToString();
            rawGameSaveExport.game_Hardmode = save.HardmodePlayers.ToString();
            rawGameSaveExport.game_Time = (save.GameTimeSeconds * 1000.0).ToString();
            rawGameSaveExport.game_Tutorial = save.UnparsedGameSave?.game_Tutorial;
            
            for(int i = 0; i < save.Stages.Count; i++)
            {
                var stage = save.Stages[i];

                rawGameSaveExport.coins_Comet += stage.CometState.ToString();
                rawGameSaveExport.coins_Cloud += stage.CloudState.ToString();

                // Not sure if the position matters for shard collection, it probably does, let's just fill with default
                rawGameSaveExport.coins_CometShard = save.UnparsedGameSave?.coins_CometShard;

                if (i + 1 != save.Stages.Count)
                {
                    rawGameSaveExport.coins_Comet += "|";
                    rawGameSaveExport.coins_Cloud += "|";
                }
            }
        }

        public static string[] StageNameLookupTable = 
        {
            "GUIDING GLADE",
        };
    }
}

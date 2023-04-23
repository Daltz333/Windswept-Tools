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

            try
            {
                // Retrieve files in save data directory
                var files = Directory.GetFiles(GlobalState.Instance.SaveSearchDir);

                int i = 1;

                // Enumerate save files
                foreach (var fileStr in files.Where(p => p.Contains(".save") && !p.Contains("Settings"))
                                                .OrderBy(p => p))
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
                    gameSave.HardmodePlayers = (HardmodeState) int.Parse(json?.game_Hardmode ?? "-1");

                    var gameStages = new List<Stage>();

                    // Iterate over comets states and create initial stages
                    if (json?.coins_Comet != null)
                    {
                        int x = 0;
                        foreach (var comet in json.coins_Comet.Split("|"))
                        {
                            var stage = new Stage();
                            stage.StageIndex = x;
                            stage.CometState = (StageCollectableState) int.Parse(comet);

                            gameStages.Add(stage);

                            x++;
                        }
                    }

                    if (json?.coins_Cloud != null)
                    {
                        int x = 0;
                        foreach (var cloud in json.coins_Cloud.Split("|"))
                        {
                            gameStages[x].CloudState = (StageCollectableState )int.Parse(cloud);

                            x++;
                        }
                    }

                    if (json?.coins_Moon != null)
                    {
                        int x = 0;
                        int y = 0;
                        foreach (var moon in json.coins_Moon.Split("|"))
                        {
                            // Grouped into "5"
                            if (y == 5)
                            {
                                y = 0;
                                x += 1;
                            }

                            // If a moon is marked as "collected" (2) then add to num moons
                            gameStages[x].NumMoons += int.Parse(moon) == 2 ? 1 : 0;
                        }
                    }

                    if (json?.stage_Times != null)
                    {
                        int x = 0;
                        foreach (var time in json.stage_Times.Split("|"))
                        {
                            gameStages[x].StageTimeSeconds = double.Parse(time) / 1000;

                            x++;
                        }
                    }

                    if (json?.coins_CometShard != null)
                    {
                        int x = 0;
                        foreach(var shardCollec in json.coins_CometShard.Split("|"))
                        {
                            int numShardsCollected = shardCollec.Split("-").Where(p => p == "2").Count();

                            gameStages[x].ShardsCollected = numShardsCollected;
                            x++;
                        }
                    }

                    gameSave.UnparsedGameSave = json;
                    gameSave.Stages = gameStages;

                    saves.Add(gameSave);
                    i++;
                }
            } catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve game saves");
            }

            return saves;
        }

        public void ExportSaveFile(GameSave save)
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

        public static Dictionary<int, string> StageNameLookupTable = new()
        {
            { 0, "GUIDING GLADE" },
        };
    }
}

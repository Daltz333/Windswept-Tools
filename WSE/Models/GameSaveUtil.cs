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
                var files = Directory.GetFiles(GlobalState.Instance.SaveSearchDir);

                int i = 1;

                // Enumerate save files
                foreach (var fileStr in files.Where(p => p.Contains(".save") && !p.Contains("Settings"))
                                                .OrderBy(p => p))
                {
                    var str = File.ReadAllText(fileStr);
                    var json = JsonConvert.DeserializeObject<JArray>(File.ReadAllText(fileStr))?.First().ToObject<UnparsedGameSave>();

                    Log.Information($"Successfully deserialized {fileStr}!");

                    var gameSave = new GameSave();
                    gameSave.SaveName = $"Save {i}";
                    gameSave.GameCoins = int.Parse(json?.game_Coins ?? "-1");
                    gameSave.GameTimeSeconds = double.Parse(json?.game_Time ?? "-1.0") / 1000;
                    gameSave.HardmodePlayers = int.Parse(json?.game_Hardmode ?? "-1");

                    var gameStages = new List<Stage>();

                    // iterate over comets states and create initial stages
                    if (json?.coins_Comets != null)
                    {
                        foreach (var comet in json.coins_Comets.Split("|"))
                        {
                            var stage = new Stage();
                            stage.CometState = (StageCollectableState) int.Parse(comet);
                        }
                    }

                    // TODO finish

                    saves.Add(gameSave);
                    i++;
                }
            } catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve game saves");
            }

            return saves;
        }
    }
}

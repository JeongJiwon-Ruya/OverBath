using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

public struct StageInfo
{
  public int World;
  public int Stage;
  public int Goal_3;
  public int Goal_2;
  public int Goal_1;
  public bool Bathtub;
  public string BathtubType;
  public bool ShowerBooth;
  public string ShowerBoothType;
  public bool Sauna;
  public string SaunaType;
  public bool Massage;
  public string MassageType;
  public bool Bucket;
  public int BucketPercentage;
  public bool Filth;
  public string FilthPeriod;
  public string Temperature;

}

public static class StageInfoReader
{
  public static List<StageInfo> StageInfo;

  public static StageInfo currentStageInfo = new()
  {
      World = 1,
      Stage = 1,
      Goal_3 = 700,
      Goal_2 = 600,
      Goal_1 = 500,
      Bathtub = false,
      BathtubType = "Water",
      ShowerBooth = false,
      ShowerBoothType = "Bodywash",
      Sauna = true,
      SaunaType = "Ocher",
      Massage = false,
      MassageType = "Towel",
      Bucket = false,
      BucketPercentage = 20,
      Filth = false,
      FilthPeriod = "",
      Temperature = "35/38"
  };
  public static (int world, int stage) selectedStageInfo
  {
    set
    {
      //테스트 상황
      // currentStageInfo ??= new StageInfo
      // {
      //     World = 1,
      //     Stage = 1,
      //     Goal_3 = 700,
      //     Goal_2 = 600,
      //     Goal_1 = 500,
      //     Bathtub = true,
      //     BathtubType = "Water",
      //     ShowerBooth = true,
      //     ShowerBoothType = "Bodywash",
      //     Sauna = false,
      //     SaunaType = "",
      //     Massage = false,
      //     MassageType = "Towel",
      //     Bucket = false,
      //     BucketPercentage = 20,
      //     Filth = false,
      //     FilthPeriod = "",
      //     Temperature = "35/38"
      // };
      currentStageInfo = StageInfo.First(x => x.World == value.world && x.Stage == value.stage);
    }
  }


  public static List<StageInfo> GetStageInfo()
  {
    if (StageInfo == null)
    {
      var b = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "StageInfo.json")); 
      var c = JsonConvert.DeserializeObject<List<StageInfo>>(b);
      StageInfo = c;
    }
    return StageInfo;
  }
}

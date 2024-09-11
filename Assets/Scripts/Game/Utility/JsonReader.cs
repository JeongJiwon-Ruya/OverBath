using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public struct StageStruct
{
  private int World;
  private int Stage;
  private int Goal_3;
  private int Goal_2;
  private int Goal_1;
  private bool Bathtub;
  private string BathtubType;
  private bool ShowerBooth;
  private string ShowerBoothType;
  private bool Sauna;
  private string SaunaType;
  private bool Massage;
  private bool Bucket;
  private int BucketPercentage;
  private bool Filth;
  private int FilthPeriod;

}

public static class JsonReader
{
  static void Initialize()
  {
    var a = JsonConvert.DeserializeObject<List<StageStruct>>(
        Path.Combine(Application.streamingAssetsPath, "Stage.json"));
  }
}

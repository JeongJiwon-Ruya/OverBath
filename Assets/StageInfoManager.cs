using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageInfoManager : MonoBehaviour
{
  public RectTransform contentBox;
  public GameObject stageBox;

  private void Start()
  {
    var a = StageInfoReader.GetStageInfo();

    foreach (var stageInfo in a)
    {
      var newBox = Instantiate(stageBox, contentBox);
      newBox.GetComponentInChildren<TextMeshProUGUI>().text =
          $"{stageInfo.Goal_1} / {stageInfo.Goal_2} / {stageInfo.Goal_3}\n{stageInfo.World}-{stageInfo.Stage}";
    }
  }
}
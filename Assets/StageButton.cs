using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageButton : MonoBehaviour
{
  public void OnClick()
  {
    StageInfoReader.selectedStageInfo = (1, 1);
    SceneManager.LoadScene("LoadingScene");
  }
}

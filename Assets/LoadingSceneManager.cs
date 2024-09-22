using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
  public Image progress;
    async void Start()
    {
      /*
       * stage 정보 확인
       */
      var a = SceneManager.LoadSceneAsync("GameScene");
      a.allowSceneActivation = false;
      while (a is { progress: < 0.9f })
      {
        progress.fillAmount = a.progress;
        await UniTask.WaitForSeconds(Time.deltaTime);
      }
      a.allowSceneActivation = true;
    }
}

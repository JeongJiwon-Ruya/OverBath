using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class FilthGenerator : MonoBehaviour
{
  private bool isFilthGenerate;
  private (int min, int max) filthPeriod;
  
  [SerializeField] private GameObject filth;
  [SerializeField] private GameObject bottom;
  private Vector2 min;
  private Vector2 max;

  private void Awake()
  {
    isFilthGenerate = StageInfoReader.currentStageInfo.Filth;
    if (isFilthGenerate)
    {
      var periodString = StageInfoReader.currentStageInfo.FilthPeriod.Split('/');
      filthPeriod = (int.Parse(periodString[0]), int.Parse(periodString[1]));
    }
  }

  private async void Start()
  {
    await UniTask.WaitUntil(() => GameManger.gameState == GameState.Start);
    if (!isFilthGenerate) return;
    
    var a = bottom.transform.localScale;
    max = new Vector2(a.x * 5f, a.z * 5f);
    min = max * -1;

    Debug.Log(max);
    Debug.Log(min);
    SpawnFilth();
  }

  private async UniTask SpawnFilth()
  {
    while (true)
    {
      await UniTask.WaitForSeconds(Random.Range(filthPeriod.min, filthPeriod.max));
      var spawned = false;
      do
      {
        var position = GetRandomPosition();
        if (position != Vector3.zero)
        {
          Instantiate(filth, position, Quaternion.identity);
          spawned = true;
        }
      } while (!spawned);
    }
    
  }

  private Vector3 GetRandomPosition()
  {
    var randomX = Random.Range(min.x, max.x);
    var randomZ = Random.Range(min.y, max.y);
    
    if (NavMesh.SamplePosition(new Vector3(randomX, 0, randomZ), out var hit, 3f, NavMesh.AllAreas))
    {
      Debug.Log(hit.position);
      return hit.position;
    }
    return Vector3.zero;
  }
}
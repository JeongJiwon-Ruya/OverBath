using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class FilthGenerator : MonoBehaviour
{
  [SerializeField] private GameObject filth;
  [SerializeField] private GameObject bottom;
  private Vector2 min;
  private Vector2 max;
  
  private async void Start()
  {
    await UniTask.WaitUntil(() => GameManger.gameState == GameState.Start);
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
      await UniTask.WaitForSeconds(Random.Range(5, 15));
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
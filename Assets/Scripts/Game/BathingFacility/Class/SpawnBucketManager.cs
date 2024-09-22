using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

public static class SpawnBucketManager
{
  private static bool isBucketSpawn;
  private static int bucketSpawnPercentage;
  
  private static ObjectPool<GameObject> bucketsPool;

  public static async void Initialize()
  {
    isBucketSpawn = StageInfoReader.currentStageInfo.Bucket;
    if (isBucketSpawn)
    {
      bucketSpawnPercentage = StageInfoReader.currentStageInfo.BucketPercentage;
      await LoadGameObjectPrefabs();
    }
    
  }

  private static async UniTask LoadGameObjectPrefabs()
  {
    await Addressables.LoadAssetsAsync<GameObject>("Assets/Prefabs/Game/Bucket.prefab", o =>
    {
      bucketsPool = new ObjectPool<GameObject>(
          () => Object.Instantiate(o),
          gameObject => gameObject.SetActive(true),
          gameObject => gameObject.SetActive(false),
          gameObject => { },
          defaultCapacity: 5,
          maxSize: 20
      );
    });
  }

  // 객체를 풀에서 가져오기
  public static void SpawnObject(Vector3 position)
  {
    if (!isBucketSpawn) return;
    if (Random.Range(0, 100) >= bucketSpawnPercentage) return;
    
    var obj = bucketsPool.Get();
    obj.transform.position = position + new Vector3(Random.Range(-1f,1f), 0, Random.Range(-1f,1f));
    return;
  }

  // 객체를 풀에 반환하기
  public static void DespawnObject(GameObject obj)
  {
    bucketsPool.Release(obj);
  }
}
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

public static class GameObjectPool
{
  private static ObjectPool<GameObject> bucketsPool;

  public static async void Initialize()
  {
    await LoadGameObjectPrefabs();
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
  public static GameObject SpawnObject(Vector3 position)
  {
    var obj = bucketsPool.Get();
    obj.transform.position = position;
    return obj;
  }

  // 객체를 풀에 반환하기
  public static void DespawnObject(GameObject obj)
  {
    bucketsPool.Release(obj);
  }
}
using UnityEngine;

public class Bucket : MonoBehaviour, ICleaningObject
{
  public CleaningObjectType cleaningObjectType
  {
    get => CleaningObjectType.Bucket;
    set => throw new System.NotImplementedException();
  }

  public GameObject GetGameObject()
  {
    return gameObject;
  }
}
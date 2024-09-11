using UnityEngine;

public class CleaningBox : MonoBehaviour
{
  public bool TakeCleaningObject(GameObject cleaningObject)
  {
    if (cleaningObject.TryGetComponent<Bucket>(out var bucket))
    {
      return true;
    }

    return false;
  }
  
}
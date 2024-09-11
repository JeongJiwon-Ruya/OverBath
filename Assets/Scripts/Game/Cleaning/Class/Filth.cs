using UnityEngine;

public class Filth : MonoBehaviour, ICleaningObject
{
  public CleaningObjectType cleaningObjectType
  {
    get => CleaningObjectType.Filth;
    set => throw new System.NotImplementedException();
  }
  
  public GameObject GetGameObject()
  {
    throw new System.NotImplementedException();
  }
}
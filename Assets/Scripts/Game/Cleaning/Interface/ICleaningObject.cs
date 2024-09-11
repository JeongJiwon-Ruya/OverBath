using UnityEngine;

public enum CleaningObjectType 
{
  Bucket,
  Broomstick,
  Filth
}

public interface ICleaningObject
{
  public CleaningObjectType cleaningObjectType { get; set; }

  public GameObject GetGameObject();
}
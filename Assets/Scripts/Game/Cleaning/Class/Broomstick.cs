using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Broomstick : MonoBehaviour, ICleaningObject
{
  public CleaningObjectType cleaningObjectType
  {
    get => CleaningObjectType.Broomstick;
    set => throw new System.NotImplementedException();
  }

  public GameObject GetGameObject()
  {
    return gameObject;
  }
}

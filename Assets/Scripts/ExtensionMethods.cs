using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
  public static bool IsNear(this Vector3 basePosition, Vector3 targetPosition, float epsilon)
  {
    return Vector2.Distance(new Vector2(basePosition.x, basePosition.z),
        new Vector2(targetPosition.x, targetPosition.z)) < epsilon;
  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSubArea : MonoBehaviour
{
  [SerializeField] private IPlayerArea parentArea;

  private void Awake()
  {
    parentArea = GetComponentInParent<IPlayerArea>();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.TryGetComponent<Player>(out var player))
    { 
      Debug.Log("player in");
      parentArea.IsPlayerIn = true;
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject.TryGetComponent<Player>(out var player))
    {
      Debug.Log("player out");
      parentArea.IsPlayerIn = false;
    }
  }
}

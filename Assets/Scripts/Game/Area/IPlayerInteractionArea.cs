using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInteractionArea
{
  public Player CurrentPlayer { get; set; }
  public bool IsPlayerIn { get; set; }
}

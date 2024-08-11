using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureControlModule : MonoBehaviour
{
  [SerializeField]private TemperatureControlSymbol symbol;
  [SerializeField]private ITemperatureControl facility;

  private void Start()
  {
    facility = GetComponentInParent<ITemperatureControl>();
  }

  public void ChangeFacilitiesTemperature()
  {
    facility.ChangeTemperature(symbol);
  }
}
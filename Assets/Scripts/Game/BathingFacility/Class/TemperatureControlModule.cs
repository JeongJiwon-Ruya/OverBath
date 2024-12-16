using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    transform.DORotate(new Vector3(0, 50, 0), 0.5f, RotateMode.LocalAxisAdd);
  }
}
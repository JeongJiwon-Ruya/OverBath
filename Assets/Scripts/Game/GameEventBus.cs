using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameEventType {BathStateChange, ShowerBoothTempStateChange}

#region TransportDataClass
public class TransportData
{
  protected TransportData() { }
  public TransportData(FacilityType facilityType, TemperatureControlSymbol symbol, Vector3 facilityPosition, int temperature)
  {
    this.facilityType = facilityType;
    this.symbol = symbol;
    this.facilityPosition = facilityPosition;
    this.temperature = temperature;
  }
  public FacilityType facilityType { get; protected set; }
  public TemperatureControlSymbol symbol { get; protected set; }
  public Vector3 facilityPosition { get; protected set; }
  public int temperature { get; protected set; }
}
public class BathStateChangeTransportData : TransportData
{
  public BathStateChangeTransportData(FacilityType facilityType, TemperatureControlSymbol symbol, Vector3 facilityPosition, int temperature, BathItemType bathItemType, BathItemType pastBathItemType)
  {
    this.facilityType = facilityType;
    this.symbol = symbol;
    this.facilityPosition = facilityPosition;
    this.temperature = temperature;
    this.bathItemType = bathItemType;
    this.pastBathItemType = pastBathItemType;
  }
  public BathItemType bathItemType { get; private set; }
  public BathItemType pastBathItemType { get; private set; }
}
#endregion

public static class GameEventBus {
  private static readonly IDictionary<GameEventType, UnityEvent<TransportData>> Events = new Dictionary<GameEventType, UnityEvent<TransportData>>();

  public static void Subscribe(GameEventType type, UnityAction<TransportData> listener) {
    if (Events.TryGetValue(type, out var thisEvent)) {
      thisEvent.AddListener(listener);
    }
    else {
      thisEvent = new UnityEvent<TransportData>();
      thisEvent.AddListener(listener);
      Events.Add(type, thisEvent);
    }
  }

  public static void UnSubscribe(GameEventType type, UnityAction<TransportData> listener) {
    if (Events.TryGetValue(type, out var thisAction)) {
      thisAction.RemoveListener(listener);
    }
  }

  public static void Publish(GameEventType type, TransportData data) {
    if (Events.TryGetValue(type, out var thisAction)) {
      thisAction.Invoke(data);
    }
  }
}
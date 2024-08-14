using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameEventType {TemperatureChange, BathItemChange}
public class TransportData
{
 
}

public class TemperatureChangeTransportData : TransportData
{
  public TemperatureChangeTransportData(TemperatureControlSymbol symbol, Vector3 facilityPosition, int temperature)
  {
    this.symbol = symbol;
    this.facilityPosition = facilityPosition;
    this.temperature = temperature;
  }
  public TemperatureControlSymbol symbol { get; private set; }
  public Vector3 facilityPosition { get; private set; }
  public int temperature { get; private set; }
}

public class BathItemChangeTransportData : TransportData
{
}

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
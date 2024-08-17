using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameEventType
{
  BathStateChange, 
  ShowerBoothTempStateChange,
  SendAreaPosition,
  Request_Bathtub,
  Request_ShowerBooth,
  Request_Area
}

#region TransportDataStruct

public struct RequestTransportData
{
  public bool request;
  public FacilityType facilityType;

  public RequestTransportData(FacilityType facilityType, bool request = true)
  {
    this.request = request;
    this.facilityType = facilityType;
  }
}

public struct AreaInfoTransportData
{
  public FacilityType facilityType { get; private set; }
  public Vector3 position;
  public AreaInfoTransportData(FacilityType facilityType, Vector3 position)
  {
    this.position = position;
    this.facilityType = facilityType;
  }
}

public struct ShowerBoothStateChangeTransportData
{
  public ShowerBoothStateChangeTransportData(FacilityType facilityType, TemperatureControlSymbol symbol, Vector3 facilityPosition, int temperature)
  {
    this.facilityType = facilityType;
    this.symbol = symbol;
    this.facilityPosition = facilityPosition;
    this.temperature = temperature;
  }
  public FacilityType facilityType { get; private set; }
  public TemperatureControlSymbol symbol { get; private set; }
  public Vector3 facilityPosition { get; private set; }
  public int temperature { get; private set; }
}
public struct BathStateChangeTransportData
{
  public BathStateChangeTransportData(FacilityType facilityType, TemperatureControlSymbol symbol, Vector3 facilityPosition, int temperature, BathItemType? bathItemType, BathItemType? pastBathItemType)
  {
    this.facilityType = facilityType;
    this.symbol = symbol;
    this.facilityPosition = facilityPosition;
    this.temperature = temperature;
    this.bathItemType = bathItemType ?? BathItemType.None; 
    this.pastBathItemType = pastBathItemType ?? BathItemType.None;
  }
  public FacilityType facilityType { get; private set; }
  public TemperatureControlSymbol symbol { get; private set; }
  public Vector3 facilityPosition { get; private set; }
  public int temperature { get; private set; }
  public BathItemType bathItemType { get; private set; }
  public BathItemType pastBathItemType { get; private set; }
}
#endregion

public static class GameEventBus {
  private static readonly IDictionary<GameEventType, UnityEvent<RequestTransportData>> RequestTransportDataEvents = new Dictionary<GameEventType, UnityEvent<RequestTransportData>>();
  private static readonly IDictionary<GameEventType, UnityEvent<AreaInfoTransportData>> AreaInfoTransportDataEvents = new Dictionary<GameEventType, UnityEvent<AreaInfoTransportData>>();
  private static readonly IDictionary<GameEventType, UnityEvent<ShowerBoothStateChangeTransportData>> ShowerBoothTransportDataEvents = new Dictionary<GameEventType, UnityEvent<ShowerBoothStateChangeTransportData>>();
  private static readonly IDictionary<GameEventType, UnityEvent<BathStateChangeTransportData>> BathStateChangeTransportDataEvents = new Dictionary<GameEventType, UnityEvent<BathStateChangeTransportData>>();

  #region Subscribe
  public static void Subscribe(GameEventType type, UnityAction<ShowerBoothStateChangeTransportData> listener) {
    if (ShowerBoothTransportDataEvents.TryGetValue(type, out var thisEvent)) {
      thisEvent.AddListener(listener);
    }
    else {
      thisEvent = new UnityEvent<ShowerBoothStateChangeTransportData>();
      thisEvent.AddListener(listener);
      ShowerBoothTransportDataEvents.Add(type, thisEvent);
    }
  }
  
  public static void Subscribe(GameEventType type, UnityAction<BathStateChangeTransportData> listener) {
    if (BathStateChangeTransportDataEvents.TryGetValue(type, out var thisEvent)) {
      thisEvent.AddListener(listener);
    }
    else {
      thisEvent = new UnityEvent<BathStateChangeTransportData>();
      thisEvent.AddListener(listener);
      BathStateChangeTransportDataEvents.Add(type, thisEvent);
    }
  }
  public static void Subscribe(GameEventType type, UnityAction<RequestTransportData> listener) {
    if (RequestTransportDataEvents.TryGetValue(type, out var thisEvent)) {
      thisEvent.AddListener(listener);
    }
    else {
      thisEvent = new UnityEvent<RequestTransportData>();
      thisEvent.AddListener(listener);
      RequestTransportDataEvents.Add(type, thisEvent);
    }
  }
  public static void Subscribe(GameEventType type, UnityAction<AreaInfoTransportData> listener) {
    if (AreaInfoTransportDataEvents.TryGetValue(type, out var thisEvent)) {
      thisEvent.AddListener(listener);
    }
    else {
      thisEvent = new UnityEvent<AreaInfoTransportData>();
      thisEvent.AddListener(listener);
      AreaInfoTransportDataEvents.Add(type, thisEvent);
    }
  }
  #endregion
  
  #region UnSubscribe
  public static void UnSubscribe(GameEventType type, UnityAction<ShowerBoothStateChangeTransportData> listener) {
    if (ShowerBoothTransportDataEvents.TryGetValue(type, out var thisAction)) {
      thisAction.RemoveListener(listener);
    }
  }
  public static void UnSubscribe(GameEventType type, UnityAction<BathStateChangeTransportData> listener) {
    if (BathStateChangeTransportDataEvents.TryGetValue(type, out var thisAction)) {
      thisAction.RemoveListener(listener);
    }
  }
  public static void UnSubscribe(GameEventType type, UnityAction<RequestTransportData> listener) {
    if (RequestTransportDataEvents.TryGetValue(type, out var thisAction)) {
      thisAction.RemoveListener(listener);
    }
  }
  public static void UnSubscribe(GameEventType type, UnityAction<AreaInfoTransportData> listener) {
    if (AreaInfoTransportDataEvents.TryGetValue(type, out var thisAction)) {
      thisAction.RemoveListener(listener);
    }
  }
  #endregion
  
  #region Publish
  public static void Publish(GameEventType type, ShowerBoothStateChangeTransportData data) {
    if (ShowerBoothTransportDataEvents.TryGetValue(type, out var thisAction)) {
      thisAction.Invoke(data);
    }
  }
  public static void Publish(GameEventType type, BathStateChangeTransportData data) {
    if (BathStateChangeTransportDataEvents.TryGetValue(type, out var thisAction)) {
      thisAction.Invoke(data);
    }
  }
  public static void Publish(GameEventType type, RequestTransportData data) {
    if (RequestTransportDataEvents.TryGetValue(type, out var thisAction)) {
      thisAction.Invoke(data);
    }
  }
  public static void Publish(GameEventType type, AreaInfoTransportData data) {
    if (AreaInfoTransportDataEvents.TryGetValue(type, out var thisAction)) {
      thisAction.Invoke(data);
    }
  }
  #endregion

}
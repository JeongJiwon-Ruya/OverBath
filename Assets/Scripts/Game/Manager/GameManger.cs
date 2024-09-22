using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public enum GameState {Ready, Start, Pause, GameOver}

public class GameManger : MonoBehaviour
{
  private readonly int timeLimit = 90;
  
  public static GameState gameState = GameState.Ready;
  
  [SerializeField] private TextMeshProUGUI standByText;
  [SerializeField]private GameObject gameOverPanel;
  
  [SerializeField]private CustomerManager customerManager;
  [SerializeField]private RectTransform customerInfoScrollView;
  [SerializeField]private GameObject customerInfoUIPrefab;
  
  private Dictionary<Customer, GameObject> customerInfoUIDictionary;

  [SerializeField] private TextMeshProUGUI timerText;
  [SerializeField]private TextMeshProUGUI cashText;

  private int cash;
  public int Cash
  {
    get => cash;
    set
    {
      cash = value;
      cashText.text = cash.ToString();
    }
  }

  private void Awake()
  {
    Time.timeScale = 1f;
    gameState = GameState.Ready;
    SpawnBucketManager.Initialize();
    customerInfoUIDictionary = new Dictionary<Customer, GameObject>();
  }

  private void Start()
  {
    StartCoroutine(MainTimer());
  }

  private IEnumerator MainTimer()
  {
    standByText.gameObject.SetActive(true);
    standByText.text = "Ready...";
    yield return new WaitForSeconds(3f);
    standByText.text = "Start!";
    yield return new WaitForSeconds(1f);
    gameState = GameState.Start;
    standByText.gameObject.SetActive(false);
    int currentTime = timeLimit;
    while (currentTime > 0)
    {
      currentTime--;
      timerText.text = currentTime.ToString();
      yield return new WaitForSeconds(1f);
    }

    Debug.Log("Game Over!");
    Time.timeScale = 0f;
    gameOverPanel.SetActive(true);
  }

  public void MakeCustomerInfoUI(Customer customer)
  {
    var sb = new StringBuilder();
    foreach (var fcb in customer.facilityFlow)
    {
      switch (fcb.facilityType)
      {
        case FacilityType.Bathtub:
          sb.Append($"{fcb.facilityType} : {fcb.itemTypeList.First()}/{fcb.temperature}");
          break;
        case FacilityType.ShowerBooth:
          var sbb = new StringBuilder();
          foreach (var VARIABLE in fcb.itemTypeList)
          {
            sbb.Append(VARIABLE + ",");
          }
          sb.Append($"{fcb.facilityType} : {sbb}/{fcb.temperature}");
          break;
        default:
          sb.Append($"{fcb.facilityType}");
          break;
      }
      sb.Append("\n");
    }
    
    var newInfoUI = Instantiate(customerInfoUIPrefab, customerInfoScrollView);
    newInfoUI.GetComponentInChildren<TextMeshProUGUI>().text = sb.ToString();
    customerInfoUIDictionary.Add(customer, newInfoUI);
  }

  public void UpdateCustomerInfoUI(Customer customer)
  {
    if (customerInfoUIDictionary.TryGetValue(customer, out var value))
    {
      var sb = new StringBuilder();
      foreach (var fcb in customer.facilityFlow)
      {
        switch (fcb.facilityType)
        {
          case FacilityType.Bathtub:
            sb.Append($"{fcb.facilityType} : {fcb.itemTypeList.First()}/{fcb.temperature}");
            break;
          case FacilityType.ShowerBooth:
            var sbb = new StringBuilder();
            foreach (var VARIABLE in fcb.itemTypeList)
            {
              sbb.Append(VARIABLE + ",");
            }
            sb.Append($"{fcb.facilityType} : {sbb}/{fcb.temperature}");
            break;
          default:
            sb.Append($"{fcb.facilityType}");
            break;
        }
        sb.Append("\n");
      }
      
      value.GetComponentInChildren<TextMeshProUGUI>().text = sb.ToString();
    }
  }
  
  public void RemoveCustomerInfoUI(Customer customer)
  {
    Debug.Log("RemoveCustomerInfoUI");
    if (customerInfoUIDictionary.TryGetValue(customer, out var value))
    {
      Destroy(value);
      customerInfoUIDictionary.Remove(customer);
    }
    else
    {
      Debug.Log("Cannot find fit Customer for delete UI");
    }
  }
}
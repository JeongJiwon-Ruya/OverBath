using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using R3;
using TMPro;
using UnityEngine;

public class GameManger : MonoBehaviour
{
  [SerializeField]private CustomerManager customerManager;
  [SerializeField]private RectTransform customerInfoScrollView;
  [SerializeField]private GameObject customerInfoUIPrefab;
  private Dictionary<Customer, GameObject> customerInfoUIDictionary;

  [SerializeField] private TextMeshProUGUI timerText;

  private void Awake()
  {
    customerInfoUIDictionary = new Dictionary<Customer, GameObject>();
  }

  private void Start()
  {
    StartCoroutine(MainTimer());
  }

  private IEnumerator MainTimer()
  {
    int timeLimit = 30;
    int currentTime = timeLimit;
    while (currentTime > 0)
    {
      currentTime--;
      timerText.text = currentTime.ToString();
      yield return new WaitForSeconds(1f);
    }

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
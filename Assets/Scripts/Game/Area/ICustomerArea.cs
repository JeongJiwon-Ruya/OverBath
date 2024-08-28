using System.Collections;
using ObservableCollections;

public interface ICustomerArea
{
  public FacilityType FacilityType { get; set; }
  public IObservableCollection<Customer> customers { get; set; }
  
  public void AddCustomer(Customer customer); //Collider에서 Customer 충돌 이벤트가 발생했을때 추가 처리하는 메소드.
  public void RemoveCustomer(Customer customer = null);
  public void PlaceNewCustomer(Customer customer); //ObservableCollection.ObserveAdd에 붙는 메소드
  public void ReleaseAndReplaceCustomer(Customer releasedCustomer); //ObservableCollection.ObserveRemove에 붙는 메소드
}
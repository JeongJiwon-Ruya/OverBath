using System.Collections;
using ObservableCollections;

public interface ICustomerArea
{
  public FacilityType facilityType { get; set; }
  public IObservableCollection<Customer> customers { get; set; }

  public void AddCustomer(Customer customer);
  public void RemoveCustomer();
  public void PlaceNewCustomer(Customer customer);
  public void ReleaseAndReplaceCustomer(Customer releasedCustomer);
}
public interface IWarehouse
{
  public BathItem BathItem { get; set; }
  public BathItemType BathItemType { get; set; }
  public BathItemType BathItemOut();
}

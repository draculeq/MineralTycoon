namespace Assets.GoogleSheet
{
    public interface IProductionBuilding
    {
        ProductType ProductType { get; }
        int ProduceAmount { get; }
        int ProducePeriod { get; }
    }
}

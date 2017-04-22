namespace Assets.GoogleSheet
{
    interface ILaboratoryBuilding
    {
        ProductType ProductType { get; }
        int ProduceAmount { get; }
        int ProducePeriod { get; }
    }
}

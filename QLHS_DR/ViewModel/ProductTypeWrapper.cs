using QLHS_DR.ChatAppServiceReference;

namespace QLHS_DR.ViewModel
{
    public class ProductTypeWrapper
    {
        public ProductTypeWrapper(ProductType productType)
        {
            EnumValue = productType;
            Name = productType.ToString(); // hoặc có thể thay đổi tên ở đây
        }

        public ProductType EnumValue { get; set; }
        public string Name { get; set; }
    }
}

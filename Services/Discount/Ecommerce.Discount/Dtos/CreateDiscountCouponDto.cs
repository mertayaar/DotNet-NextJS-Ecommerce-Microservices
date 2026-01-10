namespace Ecommerce.Discount.Dtos
{
    public class CreateDiscountCouponDto
    {
        public string CouponCode { get; set; }
        public int CouponRate { get; set; }
        public bool IsActive { get; set; }
        public DateTime ValidDate { get; set; }
    }
}

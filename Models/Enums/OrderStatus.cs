namespace API.Models.Enums
{
    /// <summary>
    /// Order status enumeration
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Chờ xác nhận
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Đã xác nhận
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// Đang giao hàng
        /// </summary>
        Shipping = 2,

        /// <summary>
        /// Đã hoàn thành
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Đã hủy
        /// </summary>
        Cancelled = 4
    }
}

namespace DominosStockOrder.Shared
{
    public class Status
    {
        public bool IsConnectedToBrowser { get; set; } = false;

        public bool? IsOrderSuccessful { get; set; } = null;
        public string OrderError { get; set; } = string.Empty;
        public bool EODRun { get; set; } = false;
    }
}

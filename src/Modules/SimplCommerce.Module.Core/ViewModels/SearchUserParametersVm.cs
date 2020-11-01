namespace SimplCommerce.Module.Core.ViewModels
{
    public class SearchUserParametersVm
    {
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string CustomerGroup { get; set; }
        public PeriodVm CreatedOn { get; set; }
    }
}

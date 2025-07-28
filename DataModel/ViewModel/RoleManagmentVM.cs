using DataModel.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace DataModel.ViewModel
{
    public class RoleManagmentVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<SelectListItem> CompanyList {  get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }

    }
}

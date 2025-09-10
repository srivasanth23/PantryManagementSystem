namespace PantryManagementSystem.Models.DTO
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string SelectedRole { get; set; } // Only one role allowed
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
    }

    public class RoleViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}

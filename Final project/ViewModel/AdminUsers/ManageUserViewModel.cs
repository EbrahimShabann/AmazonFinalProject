namespace Final_project.ViewModel.AdminUsers
{
    public class ManageUserViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool IsActive { get; set; }
        public List<string> CurrentRoles { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}

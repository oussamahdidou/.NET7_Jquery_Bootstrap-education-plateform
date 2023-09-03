namespace WEBAPP.VModels
{
    public class RegisterVM
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string ConfirmedPassword { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public IFormFile image { get; set; }
    }
}

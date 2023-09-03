namespace WEBAPP.VModels
{
    public class CreateProjectVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile file { get; set; }
    }
}

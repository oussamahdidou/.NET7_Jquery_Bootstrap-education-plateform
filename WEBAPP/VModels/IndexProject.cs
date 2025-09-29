using WEBAPP.Models;

namespace WEBAPP.VModels
{
    public class IndexProject
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectPath { get; set; }
        public bool IsLiked { get; set; }
        public int likesnumber { get; set; }
        public User Author{ get; set; }
    }
}

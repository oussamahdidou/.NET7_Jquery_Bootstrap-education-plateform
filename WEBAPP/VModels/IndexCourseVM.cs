namespace WEBAPP.VModels
{
    public class IndexCourseVM
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CoursePath { get; set; }
        public bool  IsLiked { get; set; }
        public int likesnumber { get; set; }
    }
}

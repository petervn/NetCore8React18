using MovieAPI2.Entities;
using System.ComponentModel.DataAnnotations;

namespace MovieAPI2.Model
{
    public class CreateViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="required")]
        public string Title { get; set; }
        public string Description { get; set; }

        public List<int> Actors { get; set; }

        [Required(ErrorMessage = "required")]
        public string Language { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string CoverImage { get; set; }
    }
}

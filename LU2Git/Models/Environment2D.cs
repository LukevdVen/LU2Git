using System.ComponentModel.DataAnnotations;

    public class Environment2D
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Naam { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public int MaxHeight { get; set; }

        [Required]
        public int MaxWidth { get; set; }


    }


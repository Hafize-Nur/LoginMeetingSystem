using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginMeetingSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Phone { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }

        public string? ProfileImagepath { get; set; }

        public ICollection<Meeting>? Meetings { get; set; }
    }
}
namespace EggProductionProject_MVC.Models
{
    public class UserDTO
    {
        public int MemberSid { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateOnly? BirthDate { get; set; }

        public int? IsChickFarm { get; set; }

        public byte? IsBlocked { get; set; }

        public string Chickcode { get; set; }

        public string AspUserId { get; set; }

        public IFormFile? ProfilePic { get; set; }
    }
}

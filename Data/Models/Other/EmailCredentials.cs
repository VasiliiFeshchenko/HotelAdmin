namespace MvcTest.Models.HotelManagerModels
{
    public class EmailCredentials
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public Hotel Hotel { get; set; }
        public int HotelId { get; set; }
    }
}

using HotelAdmin.Data.Models.Order;
using MvcTest.Controllers;
using MvcTest.Models.HotelManagerModels;

namespace MvcTest.Sevices.EmailService
{
    public class EmailBuilder
    {
        private Dictionary<string, object> placeholders { get; set; }

        private Order order { get; set; }
        private EmailCredentials credentials { get; set; }
        public EmailBuilder(Order order)
        {
            this.order = order;           
        }
        public EmailBuilder(Order order, EmailCredentials credentials) : this(order)
        {            
            this.credentials = credentials;
        }
        public (string, string) BuildClientEmail()
        {
            string subject = credentials.EmailSubject;
            string body = credentials.EmailBody;
            var placeholders = new Dictionary<string, object>()
            {
                { "Имя", order.Client.Name },
                { "Описание размещения", order.Information },
                { "Общая стоимость", order.Price },
                { "Предоплата", order.Prepayment }
            };
            foreach (var placeholder in placeholders)
            {
                subject = subject.Replace("{" + placeholder.Key + "}", placeholder.Value.ToString());
                body = body.Replace("{" + placeholder.Key + "}", placeholder.Value.ToString());
            }
            return (subject, body);
        }
        public string BuildAdministratorEmail()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();
            string url = configuration["URL"];
            string result = $"{url}/Orders/Edit?orderId={order.Id}&hotelId={order.HotelId}";

            return result;
        }
    }
}

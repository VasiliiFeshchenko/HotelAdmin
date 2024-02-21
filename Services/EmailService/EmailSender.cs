using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using HotelAdmin.Data.Models.Order;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MvcTest.Data;
using MvcTest.Models.HotelManagerModels;
using MvcTest.Sevices.NewContextGetter;

namespace MvcTest.Sevices.EmailService
{
    public class EmailSender
    {
        private MvcTestContext _context;
        public EmailSender(MvcTestContext context)
        {
            _context = context;
        }        
        public async Task SendEmail(Order order)
        {
            MvcTestContext context = ContextGetter.GetNewContext();
            EmailCredentials credentials = await context.EmailCredentials.Where(c => c.HotelId == order.HotelId).FirstOrDefaultAsync();

            EmailBuilder builder = new EmailBuilder(order, credentials);
            (string, string) email = builder.BuildClientEmail();
            string subject = email.Item1;
            string body = email.Item2;

            using (SmtpClient smtpClient = new SmtpClient(credentials.Host, credentials.Port))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(credentials.Username, credentials.Password);
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(credentials.Username);
                mailMessage.To.Add(order.Client.EMail);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                try
                {
                    smtpClient.Send(mailMessage);
                    Console.WriteLine("Email sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send email. Error: " + ex.Message);
                }
            }
        }
        public async Task SendEmail(string toAddress, string subject, string body, int hotelId)
        {
            MvcTestContext context = ContextGetter.GetNewContext();
            EmailCredentials credentials = await context.EmailCredentials.Where(c => c.HotelId == hotelId).FirstOrDefaultAsync();

            using (SmtpClient smtpClient = new SmtpClient(credentials.Host, credentials.Port))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(credentials.Username, credentials.Password);
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(credentials.Username);
                mailMessage.To.Add(toAddress);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                try
                {
                    smtpClient.Send(mailMessage);
                    Console.WriteLine("Email sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send email. Error: " + ex.Message);
                }
            }
        }
        public async Task SendEmailToMyself(string subject, string body, int hotelId)
        {
            MvcTestContext context = ContextGetter.GetNewContext();
            EmailCredentials credentials = await context.EmailCredentials.Where(c => c.HotelId == hotelId).FirstOrDefaultAsync();

            using (SmtpClient smtpClient = new SmtpClient(credentials.Host, credentials.Port))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(credentials.Username, credentials.Password);
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(credentials.Username);
                mailMessage.To.Add(credentials.Username);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                try
                {
                    smtpClient.Send(mailMessage);
                    Console.WriteLine("Email sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send email. Error: " + ex.Message);
                }
            }
        }
    }
}

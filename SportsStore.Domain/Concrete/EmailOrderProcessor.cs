using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace SportsStore.Domain.Concrete {

    public class EmailSettings {

        public string MailToAddress   = "wjackson_h@charter.net";
        public string MailFromAddress = "wjacksonh8@gmail.com";
        public bool   UseSsl          = true;
        public string Username        = "wjacksonh8";
        public string Password        = "M**nchild0";
        public string ServerName      = "smtp.gmail.com";
        public int    ServerPort      = 25;
        public bool   WriteAsFile     = false;
        public string FileLocation    = @"C:\Users\Walter\Source\Repos\SportsStore\sports_store_emails";
    }


    public class EmailOrderProcessor : IOrderProcessor {

        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings emailSettings) {
            this.emailSettings = emailSettings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails) {

            using (var smtpClient = new SmtpClient()) {

                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host      = emailSettings.ServerName;
                smtpClient.Port      = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials
                    = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile) {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder()
                    .AppendLine("A new order has been submitted")
                    .AppendLine("---")
                    .AppendLine("Items:");

                foreach (var line in cart.Lines) {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal: {2:c})",
                        line.Quantity,
                        line.Product.Name,
                        subtotal);
                }

                body.AppendFormat("Total order value: {0:c}", cart.ComputeTotalValue())
                    .AppendLine("---")
                    .AppendLine("Ship To:")
                    .AppendLine(shippingDetails.Name)
                    .AppendLine(shippingDetails.Line1)
                    .AppendLine(shippingDetails.Line2 ?? "")
                    .AppendLine(shippingDetails.Line3 ?? "")
                    .AppendLine(shippingDetails.City)
                    .AppendLine(shippingDetails.State ?? "")
                    .AppendLine(shippingDetails.Country)
                    .AppendLine(shippingDetails.Zip)
                    .AppendLine("---")
                    .AppendFormat("Gift wrap: {0}", shippingDetails.GiftWrap ? "Yes" : "No");

                MailMessage mailMessage = new MailMessage(
                    emailSettings.MailFromAddress,
                    emailSettings.MailToAddress,
                    "New order submitted",
                    body.ToString());

                if (emailSettings.WriteAsFile) {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                smtpClient.Send(mailMessage);
            }
        }
    }
}

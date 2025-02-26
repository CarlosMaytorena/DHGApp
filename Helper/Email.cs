using SendGrid.Helpers.Mail;
using SendGrid;
using System.Globalization;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;

namespace AgricolaDH_GApp.Helper
{
    public class Email
    {

        public Email()
        {         
        }

        public async void SendMail(string SendgridKey, string defaultEmailFrom, string recipient, string proveedor, string requisicion, string fecha, string cultivo, string rancho, string etapa, string temporada, List<ProductoOrdenarSelected> productos)
        {

            try
            {
                var apiKey = SendgridKey;
                var client = new SendGridClient(apiKey);

                var from = new EmailAddress(defaultEmailFrom, "DH&G Agrícola");

                var subject = "Requisición";
                var to = new EmailAddress(recipient);

                var htmlContent = CreateEmailBody(proveedor, requisicion, fecha, cultivo, rancho, etapa, temporada, productos, @"/wwwroot/Email/Requisicion.html");
                var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine("No se pudo enviar el email: " + e);
            }
        }

        private static string CreateEmailBody(string Proveedor, string Requisicion, string Fecha, string Cultivo, string Rancho, string Etapa, string Temporada, List<ProductoOrdenarSelected> productos, string templateRoute)
        {
            string body = string.Empty;
            string path = Directory.GetCurrentDirectory();
            string route = path + templateRoute;
            string rows = string.Empty;
            string detail = string.Empty;

            if (productos != null && productos.Count > 0)
            {
                foreach (var item in productos)
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
                    rows = rows + @"<tr>         
                                <td>" + item.Producto + @"</td>
                                <td>" + item.Cantidad + @"</td>      
                            </tr>";                    
                }
            }

            using (StreamReader reader = File.OpenText(route))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("[Proveedor]", Proveedor);
            body = body.Replace("[Requisicion]", Requisicion);
            body = body.Replace("[Fecha]", Fecha);
            body = body.Replace("[Cultivo]", Cultivo);
            body = body.Replace("[Rancho]", Rancho);
            body = body.Replace("[Etapa]", Etapa);
            body = body.Replace("[Temporada]", Temporada);
            body = body.Replace("[Productos]", rows);

            return body;
        }

    }
}

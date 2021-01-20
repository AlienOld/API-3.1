using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace API_3._1.Resource.Api.Classes
{
    public class SendMail
    {
        public static void sendEmail(string mail, string text)
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("nurelibrary@gmail.com", "NURE LIBRARY");
            // кому отправляем
            MailAddress to = new MailAddress(mail);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Возвращение книги";
            // текст письма
            m.Body = $"<h2>{text}</h2>";
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("hlib.holubov@nure.ua", "********");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}

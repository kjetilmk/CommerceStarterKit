using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using OxxCommerceStarterKit.Web.Services.Email.Models;

namespace OxxCommerceStarterKit.Web.Services.Email
{
    public class EmailDispatcher : IEmailDispatcher
    {
        private readonly ILogger _logger;

        public EmailDispatcher(ILogger logger)
        {
            _logger = logger;
        }

        public SendEmailResponse SendEmail(Postal.Email email)
        {
            var log = LogManager.GetLogger();
            return SendEmail(email, log);
        }

        public SendEmailResponse SendEmail(Postal.Email email, ILogger log)
        {
            var output = new SendEmailResponse();

#if !DEBUG
			try
			{
#endif
            // Process email with Postal
            var emailService = ServiceLocator.Current.GetInstance<Postal.IEmailService>();
            using (var message = emailService.CreateMailMessage(email))
            {
                var htmlView = message.AlternateViews.FirstOrDefault(x => x.ContentType.MediaType.ToLower() == "text/html");
                if (htmlView != null)
                {
                    string body = new StreamReader(htmlView.ContentStream).ReadToEnd();

                    // move ink styles inline with PreMailer.Net
                    var result = PreMailer.Net.PreMailer.MoveCssInline(body, false, "#ignore");

                    htmlView.ContentStream.SetLength(0);
                    var streamWriter = new StreamWriter(htmlView.ContentStream);

                    streamWriter.Write(result.Html);
                    streamWriter.Flush();

                    htmlView.ContentStream.Position = 0;
                }

                // send email with default smtp client. (the same way as Postal)
                using (var smtpClient = new SmtpClient())
                {
                    try
                    {
                        smtpClient.Send(message);
                        output.Success = true;
                    }
                    catch (SmtpException exception)
                    {
                        _logger.Error("Exception: {0}, inner message {1}",exception.Message,(exception.InnerException!=null) ? exception.InnerException.Message : string.Empty);
                        output.Success = false;
                        output.Exception = exception;
                    }
                }
            }            

#if !DEBUG
			}


			catch (Exception ex)
			{
				log.Error(ex);
				output.Exception = ex;
			}

#endif
            return output;
        }
    }
}
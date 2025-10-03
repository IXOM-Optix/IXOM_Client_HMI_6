#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.UI;
using FTOptix.Retentivity;
using FTOptix.NativeUI;
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
#endregion

public class Email : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
    [ExportMethod]
    public void AddNumbers(int number1, int number2, out int result)
    {
        result = number1 + number2;
    }
    [ExportMethod]
    public void Send_Email(
        string senderEmail,
        string senderPassword,
        string smtpClient,
        int portNumber,
        string recipientEmail,
        string subject,
        string body,
        string fileLocation,
        out string errorMessage)
    {
        errorMessage = string.Empty;

        // Default to Gmail SMTP if not provided
        var host = string.IsNullOrWhiteSpace(smtpClient) ? "smtp.gmail.com" : smtpClient;
        var port = (portNumber <= 0) ? 587 : portNumber;

        try
        {
            using (var mail = new System.Net.Mail.MailMessage())
            {
                mail.From = new System.Net.Mail.MailAddress(senderEmail);
                mail.To.Add(recipientEmail);
                mail.Subject = subject ?? string.Empty;
                mail.Body = body ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(fileLocation))
                {
                    var attachment = new System.Net.Mail.Attachment(fileLocation);
                    mail.Attachments.Add(attachment);
                }

                using (var smtp = new System.Net.Mail.SmtpClient(host, port))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            errorMessage = $"Email sent to {recipientEmail} via {host}:{port}";
            Log.Info("AddNetLogic", $"Email sent to {recipientEmail} via {host}:{port}");
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            Log.Error("AddNetLogic", $"Failed to send email to {recipientEmail} via {host}:{port} - {ex.Message}");
        }
    }

}

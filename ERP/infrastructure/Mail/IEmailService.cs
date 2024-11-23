﻿namespace ERP.infrastructure.Mail
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);

    }
}

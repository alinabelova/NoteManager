﻿using System.Threading.Tasks;

namespace NoteManager.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}

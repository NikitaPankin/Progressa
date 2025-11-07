using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string GetEmailConfirmationTemplate(string userName, string confirmationUrl)
        {
            return $@"
                <h2>Hello, {userName}</h2>
                <p>Thank you for registering at Progressa. Please confirm your email by clicking the link below:</p>
                <a href='{confirmationUrl}'>Confirm Email</a>
                <p>If you did not register, ignore this email.</p>
            ";
        }
    }
}

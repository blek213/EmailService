using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message, string callbackUrl);

    }
}

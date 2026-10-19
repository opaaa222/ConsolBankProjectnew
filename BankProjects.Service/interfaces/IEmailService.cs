using System;

namespace BankProjects.Service.interfaces
{
    public interface IEmailService
    {
        void SendLoanApprovalEmail(string toEmail, string userName, decimal loanAmount);
        void SendLoanRejectionEmail(string toEmail, string userName, decimal loanAmount);
    }
}
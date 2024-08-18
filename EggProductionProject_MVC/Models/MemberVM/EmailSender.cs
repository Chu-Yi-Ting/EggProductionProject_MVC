using Microsoft.AspNetCore.Identity.UI.Services;

namespace EggProductionProject_MVC.Models.MemberVM
{
	public class EmailSender : IEmailSender
	{
		public Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			// 在這裡實現發送電子郵件的邏輯。
			// 例如，你可以使用第三方服務如 SendGrid、SMTP 服務器等來發送郵件。
			// 這裡為演示使用Console輸出，實際應根據需要實作
			Console.WriteLine($"Send email to {email}, subject: {subject}, message: {htmlMessage}");
			return Task.CompletedTask;
		}
	}
}

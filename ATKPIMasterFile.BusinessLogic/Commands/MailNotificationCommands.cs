using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ShooryMoory.BusinessLogic.Resources;
using ShooryMoory.BusinessLogic.Services;
using ShooryMoory.BusinessLogic.ViewModels;
using ShooryMoory.DataAccess.Managers;
using ShooryMoory.DataAccess.Model;
using ShooryMoory.DataAccess.Model.NoSql;
using ShooryMoory.Infrastructure.Helpers;
using ShooryMoory.Infrastructure.Services;


namespace ShooryMoory.BusinessLogic.Commands
{
    public class MailNotificationCommands
    {
        private readonly ILogService _logService;
        readonly BlobManager _blobManager;
        private readonly WebContextService _contextService;

        public MailNotificationCommands(ILogService logger, WebContextService contextService)
        {
            _logService = logger;
            _contextService = contextService;
            //_blobManager = new BlobManager(new string[] { "mailslog" });
            _blobManager = new BlobManager(Infrastructure.Services.ConfigSettingsService.Get("DataConnectionString"));
            _blobManager.CreatePrivateContainerIfNotExists("mailslog");
        }


        public string FormatUrl(string url, long userId, string utm_medium, bool addTokem = true, string utm_campaign = null)
        {
            if (url == null)
                return null;
            url += (url.Contains("?") ? "&" : "?");
            if (addTokem)
                url += TokenName + "=" + FormatToken(userId, 48) + "&";
            url += "utm_source=sitemail&utm_medium=" + utm_medium.ToLower() + "&utm_content=" + userId + (string.IsNullOrWhiteSpace(utm_campaign) ? "" : ("&utm_campaign=" + utm_campaign));//&utm_campaign=kinkymail
            if (!url.StartsWith("http"))
                url = System.Configuration.ConfigurationManager.AppSettings.Get("host") + url;
            return url;
        }

        public void SendRegistrationConfirmationTemplate(string token, string templatePath, User user)
        {
            Task.Run(() =>
            {
                try
                {
                    using (new LocaleSwitcher(user.Locale))
                    {
                        var model = new EmailConfirmation
                        {
                            Name = user.Name,
                            Link = FormatUrl(_contextService.UrlRoute(RouteEnum.Default, new { controller = "Account", action = "EmailConfirmation", value = token, user.UserId }), user.UserId, "EmailConfirmation", false)
                        };
                        var mailSender = new MailServiceSendGrid(ConfigSettingsService.Get("NotificationMailSettingsSendGrid"), _logService);
                        var template = mailSender.SendMailUsingTemplateFromFile(user.Email, MailResources.Registration_confirmation, model, templatePath);
                        MailsLog(template, user.Email, user.UserId, MailResources.Registration_confirmation, templatePath);
                    }
                }
                catch (Exception ex)
                {
                    _logService.Error("SendRegistrationConfirmationTemplate", ex);
                }
            });
        }

        public void SendRegistrationMailWithPasswordTemplate(string token, string templatePath, User user)
        {
            Task.Run(() =>
            {
                try
                {
                    using (new LocaleSwitcher(user.Locale))
                    {
                        var model = new EmailWithPassword
                        {
                            Name = user.Name,
                            Link = FormatUrl(_contextService.UrlRoute(RouteEnum.Default, new { controller = "Account", action = "EmailConfirmation", value = token, user.UserId }), user.UserId, "EmailConfirmation", false),
                            Login = user.Email,
                            Password = user.AuthPassword
                        };
                        var mailSender = new MailServiceSendGrid(ConfigSettingsService.Get("NotificationMailSettingsSendGrid"), _logService);
                        var template = mailSender.SendMailUsingTemplateFromFile(user.Email, MailResources.Registration_confirmation, model, templatePath);
                        MailsLog(template, user.Email, user.UserId, MailResources.Registration_confirmation, templatePath);
                    }
                }
                catch (Exception ex)
                {
                    _logService.Error("SendRegistrationConfirmationTemplate", ex);
                }
            });
        }

        public void SendPasswordRecoveryTemplate(string token, string templatePath, User user)
        {
            Task.Run(() =>
            {
                try
                {
                    using (new LocaleSwitcher(user.Locale))
                    {
                        var mailSender = new MailServiceSendGrid(ConfigSettingsService.Get("NotificationMailSettingsSendGrid"), _logService);
                        var model = new ResetPassword
                        {
                            Name = user.Name,
                            Link = FormatUrl(_contextService.UrlRoute(RouteEnum.Default, new { controller = "Account", action = "ResetPassword", value = token, user.UserId }), user.UserId, "ResetPassword", false)
                        };
                        var template = mailSender.SendMailUsingTemplateFromFile(user.Email, MailResources.Password_recovery, model, templatePath);
                        MailsLog(template, user.Email, user.UserId, MailResources.Password_recovery, templatePath);
                    }
                }
                catch (Exception ex)
                {
                    _logService.Error("SendPasswordRecoveryTemplate", ex);
                }
            });
        }


        public void SendShareMessageEmails(IList<string> Emails, string templatePath, User user)
        {
            Task.Run(() =>
            {
                try
                {
                    var viewUser = new UserViewModel(_contextService, user);
                    using (new LocaleSwitcher(user.Locale))
                    {
                        foreach (var mail in Emails)
                            SendShareMessageEmail(mail, templatePath, viewUser, 0);
                    }
                }
                catch (Exception ex)
                {
                    _logService.Error("SendShareMessageEmails", ex);
                }
            });
        }

        public void SendShareMessageEmail(string mail, string templatePath, UserViewModel user, int numberOfShipments)
        {
            var mailSender = new MailServiceSendGrid(ConfigSettingsService.Get("NotificationMailSettingsSendGrid"), _logService);
            var model = new ShareMessageEmail
            {
                User = user,
                Email = mail,
                Link = FormatUrl("/register?share=" + user.UserId, user.UserId, "ShareForPremium", false, (numberOfShipments == 0 ? null : numberOfShipments.ToString()))
            };
            var subj = user.NameFull + " " + MailResources.Sent_you + " " + MailResources.Message;
            var template = mailSender.SendMailUsingTemplateFromFile(mail, subj, model, templatePath, user.Name);
            MailsLog(template, mail, user.UserId, subj, templatePath);
        }

        public void SendShareMessageEmailOnRegistration(string mail, string templatePath, long userId, int numberOfShipments)
        {
            var mailSender = new MailServiceSendGrid(ConfigSettingsService.Get("NotificationMailSettingsSendGrid"), _logService);
            var model = new ShareMessageEmail
            {
                Email = mail,
                Link = FormatUrl("/register?share=" + userId, userId, "findfriend", false, (numberOfShipments == 0 ? null : numberOfShipments.ToString()))
            };
            var subj = "Одна из ваших подруг зарегистрировалась на KinkyLove";
            var template = mailSender.SendMailUsingTemplateFromFile(mail, subj, model, templatePath);
            MailsLog(template, mail, userId, subj, templatePath);
        }

        public void SendTemplate(string email, string subject, object model, string templatePath, User user)
        {
            Task.Run(() =>
            {
                using (new LocaleSwitcher(user.Locale))
                {
                    SendTemplateSync(email, subject, model, templatePath, user.UserId);
                }
            });
        }

        public void SendTemplateSync(string email, string subject, object model, string templatePath, long userId, string displayName = null)
        {
            try
            {
                var mailSender = new MailServiceSendGrid(ConfigSettingsService.Get("NotificationMailSettingsSendGrid"), _logService);
                var template = mailSender.SendMailUsingTemplateFromFile(email, subject, model, templatePath, displayName);
                if (userId > -1)
                    MailsLog(template, email, userId, subject, templatePath);
            }
            catch (Exception ex)
            {
                _logService.Error("SendTemplateSync: " + templatePath + " userId:" + userId, ex);
            }

        }

        public void SendPasswordReset(string email, string resetLink)
        {
            var mailSender = new MailServiceSendGrid(ConfigSettingsService.Get("NotificationMailSettingsSendGrid"), _logService);
            const string template = "Ссылка для восстановления пароля <a target='_blank' href='{0}'>{0}</a>.";
            mailSender.SendMail(email, "Восстановление пароля", string.Format(template, resetLink));
        }

        public void SentMailToSupport(User user, string feedBackMail, string message)
        {
            var mailSender = new MailServiceSendGrid(ConfigSettingsService.Get("NotificationMailSettingsSendGrid"), _logService);
            string displayName = user.Name + "(" + user.UserId + ")";
            mailSender.SendMail(/*"maximmelnik@gmail.com"*/"mitya.dmitrov@gmail.com,support@kinkylove.com", "Заявка пользователя " + displayName
                , string.Format("<b>Заявка пользователя <a href='https://kinkylove.com/{0}' href>{1}</a></b>:<br/><br/>", user.UserId, displayName) + Environment.NewLine + message.Replace(Environment.NewLine, "<br/>")
                , feedBackMail, displayName);
        }

        public void SentComplainMail(User user, string url, string reason)
        {
            var mailSender = new MailServiceSendGrid(ConfigSettingsService.Get("NotificationMailSettingsSendGrid"), _logService);
            string displayName = user.Name + "(" + user.UserId + ")";
            mailSender.SendMail("mitya.dmitrov@gmail.com,support@kinkylove.com", "Жалоба пользователя " + displayName
                , string.Format("<b>Жалоба пользователя на <a href='https://kinkylove.com{0}' href>изображение</a></b><br/><br/>", url) + Environment.NewLine + "Причина: " + reason
                , user.Email, displayName);
        }

        private void MailsLog(string template, string email, long userId, string subject, string templatePath)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(template);
                    writer.Flush();
                    stream.Position = 0;

                    DateTime d = DateTime.UtcNow;
                    string firtsPartName = d.ToString("yyyy_MM_dd") + "/" + d.ToString("HH_mm_ss_");
                    string secondPartName = "user" + userId.ToString() + "_" + (email ?? "__no_mail__").Replace("@", "_").Replace(".", "_") + ".html";
                    var filePath = "mailslog/" + firtsPartName + secondPartName;
                    var url = _blobManager.Save(filePath, stream);

                    MailLogEntity log = new MailLogEntity(userId);

                    log.Email = email;
                    log.FilePath = url;
                    log.Subject = subject;
                    var spl = (templatePath ?? "").Split('\\');
                    if (spl.Length > 0)
                        log.TemplatePath = spl[spl.Length - 1];

                    MailLogEntity.Add(log);
                }
            }
        }

        private const string encodFormat = "axgr-{0}-hello-{1}-kinkylove";
        public const string TokenName = "kinkyt";
        private const char encodValuespliter = 'l';
        private const long TiksToHourCoef = 36000000000;
        public string FormatToken(long userId, int timeoutInHours)
        {
            var dt = DateTime.UtcNow.AddHours(timeoutInHours).Ticks / TiksToHourCoef;
            var sign = Encode(userId, dt);
            return string.Join("" + encodValuespliter, userId, dt, sign);
        }

        private static string Encode(long userId, long dt)
        {
            byte[] bSignature = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format(encodFormat, userId, dt)));
            var sbSignature = new StringBuilder();
            foreach (byte b in bSignature)
                sbSignature.AppendFormat("{0:x2}", b);
            return sbSignature.ToString();
        }

        public static long ParseToken(string token)
        {
            try
            {
                var spl = token.Split(encodValuespliter);
                var userId = long.Parse(spl[0]);
                var dt = long.Parse(spl[1]);
                var time = new DateTime(dt * TiksToHourCoef, DateTimeKind.Utc);
                if (DateTime.UtcNow > time)
                    return 0;
                return Encode(userId, dt) == spl[2] ? userId : 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}

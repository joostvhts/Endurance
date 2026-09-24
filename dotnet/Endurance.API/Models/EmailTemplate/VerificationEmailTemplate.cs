namespace Endurance.API.Models.EmailTemplate;

public static class VerificationEmailTemplate
{
    public const string Body = @"<!doctype html>
<html lang=""en"">
  <head>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
    <title>Endurance - Verify your email address</title>
    <style media=""all"" type=""text/css"">
      @media only screen and (max-width: 640px) {
        .main p, .main td, .main span { font-size: 16px !important; }
        .wrapper { padding: 16px !important; }
        .content { padding: 0 !important; }
        .container { padding: 0 !important; padding-top: 8px !important; width: 100% !important; }
        .main { border-left-width: 0 !important; border-radius: 0 !important; border-right-width: 0 !important; }
        .button { display: block !important; width: auto !important; }
      }
    </style>
  </head>
  <body style=""font-family: Helvetica, sans-serif; -webkit-font-smoothing: antialiased; font-size: 16px; line-height: 1.3; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%; background-color: #f4f5f6; margin: 0; padding: 0;"">
    <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-collapse: separate; background-color: #f4f5f6; width: 100%;"" width=""100%"" bgcolor=""#f4f5f6"">
      <tr>
        <td>&nbsp;</td>
        <td class=""container"" style=""font-family: Helvetica, sans-serif; font-size: 16px; vertical-align: top; max-width: 600px; padding-top: 24px; width: 600px; margin: 0 auto;"" width=""600"" valign=""top"">
          <div class=""content"" style=""box-sizing: border-box; display: block; margin: 0 auto; max-width: 600px;"">
            <span style=""color: transparent; display: none; height: 0; max-height: 0; max-width: 0; opacity: 0; overflow: hidden; visibility: hidden; width: 0;"">Verify your email address to activate your Endurance class alert.</span>
            <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""main"" style=""border-collapse: separate; background: #ffffff; border: 1px solid #eaebed; border-radius: 16px; width: 100%;"" width=""100%"">
              <tr>
                <td class=""wrapper"" style=""font-family: Helvetica, sans-serif; font-size: 16px; vertical-align: top; box-sizing: border-box; padding: 24px;"" valign=""top"">
                  <h2>Endurance</h2>
                  <p style=""font-family: Helvetica, sans-serif; font-size: 16px; margin: 0 0 16px;"">Hi there,</p>
                  <p style=""font-family: Helvetica, sans-serif; font-size: 16px; margin: 0 0 24px;"">Confirm your email address to activate your class alert.</p>
                  <p style=""font-family: Helvetica, sans-serif; font-size: 16px; margin: 0 0 24px; text-align: center;"">
                    <a class=""button"" href=""{VerificationUrl}"" style=""background-color: #18181b; border-radius: 8px; color: #ffffff; display: inline-block; font-weight: bold; padding: 12px 20px; text-decoration: none;"">Verify email and activate alert</a>
                  </p>
                  <p style=""font-family: Helvetica, sans-serif; font-size: 13px; margin: 0; color: #6b7280;"">This link expires in 24 hours. If you did not request this alert, you can ignore this email.</p>
                </td>
              </tr>
            </table>
            <div style=""clear: both; padding-top: 24px; text-align: center; width: 100%; color: #9a9ea6; font-size: 13px;"">Endurance</div>
          </div>
        </td>
        <td>&nbsp;</td>
      </tr>
    </table>
  </body>
</html>";
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Status_200;
using Status_200.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Status_200WebAPI.Models;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;

namespace Status_200WebAPI.Controllers
{
    [Authorize(Roles = "1")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetAllReportData", Name = "GetAllReportData")]
        public async Task<IActionResult> GetAllReportData(DateTime dateStart, DateTime dateEnd)
        {
            var reports = await _context.Reports
                .Where(x=>x.DateCreate.Date >= dateStart && x.DateCreate.Date <= dateEnd)
                .Include(x => x.Status)
                .Include(x => x.UsersId)
                .ToListAsync();

            var reportGetDataDTO = reports.Select(rep => new ReportGetDataDTO
            {
                Id = rep.Id,
                ProjectName = rep.ProjectName,
                AssignedTasks = rep.AssignedTasks,
                WorkReport = rep.WorkReport,
                StatusName = rep.Status.StatusName,
                UserName = rep.UsersId.FirstName + " " + rep.UsersId.SecondName,
                UserId = rep.UserId,
                StatusTaskId = rep.StatusReports,
                DateCreate = rep.DateCreate,
                DateClosed = rep.DateClosed
            });

            return Ok(reportGetDataDTO);
        }

        [HttpGet]
        [Route("DownloadExcelFile", Name = "DownloadExcelFile")]
        public async Task<IActionResult> DownloadExcelFile(DateTime dateStart, DateTime dateEnd)
        {
            var fileName = "report" + DateTime.Now + ".xlsx";
            fileName = fileName.Replace(":", "_");
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Создаем новый Excel-файл
            using var package = new ExcelPackage(new FileInfo(filePath));

            // Добавляем лист в книгу
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            // Добавляем на лист заголовки столбцов
            worksheet.Cells[1, 1].Value = "ID";


            worksheet.Cells[1, 2].Value = "ProjectName";

            worksheet.Column(2).Width = 20; // изменение ширины колонки

            worksheet.Cells[1, 3].Value = "UserName";

            worksheet.Column(3).Width = 40; // изменение ширины колонки

            worksheet.Cells[1, 4].Value = "WorkReport";
            worksheet.Column(4).Width = 40; // изменение ширины колонки

            worksheet.Cells[1, 5].Value = "StatusName";
            worksheet.Column(5).Width = 20; // изменение ширины колонки

            worksheet.Cells[1, 6].Value = "AssignedTasks";
            worksheet.Column(6).Width = 40; // изменение ширины колонки
            worksheet.Cells[1, 7].Value = "DateCreate";
            worksheet.Column(7).Width = 20;
            worksheet.Cells[1, 8].Value = "DateClosed";
            worksheet.Column(8).Width = 20;


            for (int i = 1; i <= 8; i++)
            {
                worksheet.Cells[1, i].Style.Font.Bold = true;
                worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }


            // Заполняем лист данными из базы данных

            var reports = await _context.Reports
              .Include(x => x.Status)
              .Include(x => x.UsersId)
              .ToListAsync();

            var reportGetDataDTO = reports.Where(x => x.DateCreate.Date >= dateStart && x.DateCreate.Date <= dateEnd).Select(rep => new ReportGetDataDTO
            {
                Id = rep.Id,
                ProjectName = rep.ProjectName,
                AssignedTasks = rep.AssignedTasks,
                WorkReport = rep.WorkReport,
                StatusName = rep.Status.StatusName,
                UserName = rep.UsersId.FirstName + " " + rep.UsersId.SecondName,
                UserId = rep.UserId,
                StatusTaskId = rep.StatusReports,
                DateCreate = rep.DateCreate,
                DateClosed = rep.DateClosed
            });


            var row = 2;
            foreach (var item in reportGetDataDTO)
            {
                worksheet.Cells[row, 1].Value = item.Id;
                worksheet.Cells[row, 2].Value = item.ProjectName;
                worksheet.Cells[row, 3].Value = item.UserName;
                worksheet.Cells[row, 4].Value = item.WorkReport;
                worksheet.Cells[row, 5].Value = item.StatusName;
                worksheet.Cells[row, 6].Value = item.AssignedTasks;
                worksheet.Cells[row, 7].Value = item.DateCreate.ToString("dd.MM.yyyy");
                worksheet.Cells[row, 8].Value = item.DateClosed?.ToString("dd.MM.yyyy");

                row++;
            }

            // Сохраняем изменения в Excel-файле
            await package.SaveAsync();

            // Отдаем файл на скачивание
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        [Route("SendEmail", Name = "SendEmail")]
        public async Task<IActionResult> SendEmai([FromForm] SendMailDTO sendMail)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("SStatus200", "sstatus200@mail.ru"));
                emailMessage.To.Add(new MailboxAddress("", sendMail.email));
                emailMessage.Subject = sendMail.subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = sendMail.message;

                if (sendMail.formFile != null && sendMail.formFile.Length != 0)
                {
                    byte[] fileBytes;
                    using (var ms = new MemoryStream())
                    {
                        await sendMail.formFile.CopyToAsync(ms);
                        fileBytes = ms.ToArray();
                    }
                    var attachment = bodyBuilder.Attachments.Add(sendMail.formFile.FileName, fileBytes);
                }
                emailMessage.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.mail.ru", 465, true);
                    await client.AuthenticateAsync("sstatus200@mail.ru", "hXARKW6STWhNJcQgxh6q");
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
                return Ok("File sent successfully"); //Файл успешно отправлен
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while sending: {ex.Message}");   //Произошла ошибка при отправке:
            }

        }

        [HttpPut]
        [Route("ChangeUserRole", Name = "ChangeUserRole")]
        public async Task<IActionResult> ChangeUserRole(int userId, byte newRole)
        {

            Role role = await _context.Roles.SingleOrDefaultAsync(x => x.Id == newRole);
            if (role == null)
                return BadRequest("Role not found"); //Роль не найдена

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                try
                {

                    user.RoleId = newRole;
                    await _context.SaveChangesAsync();
                    return Ok("User role " + user.FirstName + " " + user.SecondName + " successfully modified");
                    //Роль у пользователя+ +успешно изменена
                }
                catch (Exception ex)
                {
                    return BadRequest("Something went wrong " + ex.Message); //Что то пошло не так
                }

            }

            return BadRequest();
        }
    }
}

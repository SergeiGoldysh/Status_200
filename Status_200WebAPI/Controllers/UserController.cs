using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Status_200;
using Status_200.Models;
using Microsoft.EntityFrameworkCore;
using Status_200WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Status_200WebAPI.Controllers
{
    [Authorize(Roles = "2")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("CreateReport", Name = "CreateReports")]
        public async Task<IActionResult> CreateReports([FromBody] ReportAddDTO reportData)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = new Report();
            report.ProjectName = reportData.ProjectName;
            report.StatusReports = 1;
            report.UserId = reportData.UserId;
            report.WorkReport = reportData.WorkReport;
            report.AssignedTasks = reportData.AssignedTasks;
            report.DateCreate = DateTime.Now;
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return Ok("Report added"); //Отчет добавлен
        }

        [HttpGet]
        [Route("GetAllReportDataForUser", Name = "GetAllReportDataForUser")]
        public async Task<IActionResult> GetAllReportData(DateTime dateStart, DateTime dateEnd, int UserId)
        {
            var reports = await _context.Reports
                .Where(x => x.DateCreate.Date >= dateStart && x.DateCreate.Date <= dateEnd && x.UserId == UserId)
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

        [HttpPut]
        [Route("ChangeStatusTask", Name = "ChangeStatusTask")]
        public async Task<IActionResult> ChangeStatusTask(int reportId, byte StatusTaskId)
        {

            Report report = await _context.Reports.SingleOrDefaultAsync(x => x.Id == reportId);
            if (report == null)
                return BadRequest("Task not found"); //Задача не найдена

            StatusTask statusTask = await _context.StatusesTask.SingleOrDefaultAsync(x => x.Id == StatusTaskId);
            if (statusTask == null)
                return BadRequest("Task status not found"); //Статус задачи не найден

            var reportChange = await _context.Reports.FirstOrDefaultAsync(x => x.Id == reportId);
            if (reportChange != null)
            {
                try
                {
                    if (StatusTaskId == 4)
                        reportChange.DateClosed = DateTime.Now;
                    else
                        reportChange.DateClosed = null;
                    reportChange.StatusReports = StatusTaskId;
                    await _context.SaveChangesAsync();
                    return Ok("Статус задачи " + reportChange.ProjectName + " успешно изменен");
                    //Task status ++ changed successfully
                }
                catch (Exception ex)
                {
                    return BadRequest("Something went wrong " + ex.Message);
                    //Что то пошло не так
                }
            }
            return BadRequest();
        }


    }
}

using LyteAppointmentV3.Context;
using LyteAppointmentV3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LyteAppointmentV3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly ILogger<AppointmentController> _logger;
        private readonly AppointmentContext _context;

        public AppointmentController(ILogger<AppointmentController> logger, AppointmentContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Init data sample
        private static List<AppointmentModel> appointmentModels = new List<AppointmentModel>
        {
            new AppointmentModel
            {
                Id = 1,
                Subject = "Lyte Daily Sync-up Meeting",
                StartTime = new DateTime(2022,09,01,09,00,00),
                EndTime = new DateTime(2022,09,01,09,30,00),
                Location = "Google Meets",
                Description = "Use this placeholder for daily 30-minutes pulse checks with team whether they got any blockers, updates, goals and target completion for current sprint.",
                IsAllDay = false,
                IsBlock = true,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            },
            new AppointmentModel
            {
                Id = 2,
                Subject = "Lyte Daily Sync-up Meeting",
                StartTime = new DateTime(2022,09,01,09,00,00),
                EndTime = new DateTime(2022,09,01,09,30,00),
                Location = "Google Meets",
                Description = "Use this placeholder for daily 30-minutes pulse checks with team whether they got any blockers, updates, goals and target completion for current sprint.",
                IsAllDay = false,
                IsBlock = true,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            },
            new AppointmentModel
            {
                Id = 3,
                Subject = "Lyte Daily Sync-up Meeting",
                StartTime = new DateTime(2022,09,01,09,00,00),
                EndTime = new DateTime(2022,09,01,09,30,00),
                Location = "Google Meets",
                Description = "Use this placeholder for daily 30-minutes pulse checks with team whether they got any blockers, updates, goals and target completion for current sprint.",
                IsAllDay = false,
                IsBlock = true,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            }
        };

        [HttpGet]
        public async Task<ActionResult<List<AppointmentModel>>> GetAppointment()
        {
            try
            {
                return Ok(await _context.Appointments.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest("Error getting on appointment data");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<AppointmentModel>>> GetAppointment(int id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return BadRequest("Appointment not found");
                }
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest("Error on Appointment");
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<AppointmentModel>>> AddAppointment(AppointmentModel model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Appointment not found");
                //Start time should be earlier than End time
                if (model.StartTime > model.EndTime)
                    return BadRequest("Appointment start date should be earlier than end date.");

                //Need to handle if there were the same start time
                var appointmentsList = await _context.Appointments.ToListAsync();
                if (AppointmentConflictsWithOthers(appointmentsList, model.StartTime, model.EndTime))
                    return BadRequest("Appointment conflict found.");

                _context.Appointments.Add(model);
                await _context.SaveChangesAsync();
                return Ok(await _context.Appointments.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest("Error on Appointment");
            }
        }

        public static bool AppointmentConflictsWithOthers(List<AppointmentModel> appointmentModels, DateTime startDateTime, DateTime endDateTime)
        {
            return appointmentModels.Any(e =>
            {
                var startsInOtherEvent = startDateTime >= e.StartTime && startDateTime <= e.EndTime;
                var endsInOtherEvent = endDateTime >= e.StartTime && endDateTime <= e.EndTime;

                return startsInOtherEvent || endsInOtherEvent;
            });
        }

        [HttpPut]
        public async Task<ActionResult<List<AppointmentModel>>> UpdateAppointment(AppointmentModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Appointment not found");
                }
                var getModelById = await _context.Appointments.FindAsync(model.Id);
                if (getModelById == null)
                    return BadRequest("Appointment not found");

                getModelById.Description = model.Description;
                getModelById.Subject = model.Subject;
                getModelById.StartTime = model.StartTime;
                getModelById.EndTime = model.EndTime;
                getModelById.Location = model.Location;
                getModelById.IsAllDay = model.IsAllDay;
                getModelById.ModifiedBy = model.ModifiedBy;
                getModelById.ModifiedOn = model.ModifiedOn;
                await _context.SaveChangesAsync();
                return Ok(await _context.Appointments.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest("Error on Appointment");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<AppointmentModel>>> DeleteAppointment(int? id)
        {
            try
            {
                if (!id.HasValue)
                    return NotFound("Appointment not found");
                var appointment = await _context.Appointments.FindAsync(id.Value);
                if (appointment == null)
                    return NotFound("Appointment not found");

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                return Ok(await _context.Appointments.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest("Error on Appointment");
            }
        }        
    }
}

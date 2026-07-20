using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ClinicaVeterinariaAPI.Controllers
{
    public class AppointmentController : ApiController
    {
        ClinicaVeterinariaDataContext db = new ClinicaVeterinariaDataContext
            (
                ConfigurationManager
                    .ConnectionStrings["ClinicaVeterinariaConnectionString"].ConnectionString
            );

        // GET: api/Appointment
        /// <summary>
        /// All Appointments
        /// </summary>
        /// <returns>Appointments list</returns>
        public List<Appointment> Get()
        {
            var list = from Appointment in db.Appointments orderby Appointment.Id select Appointment;
            return list.ToList();
        }

        // GET: api/Appointment/5
        /// <summary>
        /// Specific (one) Appointment
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Appointment</returns>
        public IHttpActionResult Get(int id)
        {
            var appointment = db.Appointments.FirstOrDefault(a => a.Id == id);

            if(appointment == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Consulta não existe"));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, appointment));
        }

        // POST: api/Appointment
        /// <summary>
        /// Creates a new Appointment
        /// </summary>
        /// <param name="newAppointment">Appointment</param>
        public IHttpActionResult Post([FromBody]Appointment newAppointment)
        {
            var appointment = db.Appointments.FirstOrDefault(a => a.Id == newAppointment.Id);
            var animal = db.Animals.FirstOrDefault(an => an.Id == newAppointment.AnimalId);
            var doctor = db.Doctors.FirstOrDefault(d => d.Id == newAppointment.DoctorId);
            var room = db.Rooms.FirstOrDefault(r => r.Id == newAppointment.RoomId);

            if (appointment != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Já existe uma consulta registada com esse id"));
            }

            if (animal == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não existe um animal registado com esse id"));
            }

            if (doctor == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não existe um doutor registado com esse id"));
            }

            if (room == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não existe uma sala registada com esse id"));
            }

            db.Appointments.InsertOnSubmit(newAppointment);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e.Message));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        // PUT: api/Appointment/5
        /// <summary>
        /// Updates (one) Appointment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedAppointment">Appointment</param>
        public IHttpActionResult Put(int id, [FromBody]Appointment updatedAppointment)
        {
            var appointment = db.Appointments.FirstOrDefault(a => a.Id == updatedAppointment.Id);
            var animal = db.Animals.FirstOrDefault(an => an.Id == updatedAppointment.AnimalId);
            var doctor = db.Doctors.FirstOrDefault(d => d.Id == updatedAppointment.DoctorId);
            var room = db.Rooms.FirstOrDefault(r => r.Id == updatedAppointment.RoomId);

            if (appointment == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não existe uma consulta registada com esse id"));
            }

            if (animal == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não existe um animal registado com esse id"));
            }

            if (doctor == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não existe um doutor registado com esse id"));
            }

            if (room == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não existe uma sala registada com esse id"));
            }

            appointment.AnimalId = updatedAppointment.AnimalId;
            appointment.DoctorId = updatedAppointment.DoctorId;
            appointment.RoomId = updatedAppointment.RoomId;
            appointment.Motive = updatedAppointment.Motive;
            appointment.Treatment = updatedAppointment.Treatment;
            appointment.Canceled = updatedAppointment.Canceled;
            appointment.Date = updatedAppointment.Date;
            appointment.StartTime = updatedAppointment.StartTime;
            appointment.EndTime = updatedAppointment.EndTime;

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e.Message));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        // DELETE: api/Appointment/5
        /// <summary>
        /// Deletes (one) Appointment
        /// </summary>
        /// <param name="id"></param>
        public IHttpActionResult Delete(int id)
        {
            var appointment = db.Appointments.FirstOrDefault(a => a.Id == id);

            if (appointment == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhuma consulta com esse ID para poder eliminar"));
            }

            db.Appointments.DeleteOnSubmit(appointment);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e.Message));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }
    }
}

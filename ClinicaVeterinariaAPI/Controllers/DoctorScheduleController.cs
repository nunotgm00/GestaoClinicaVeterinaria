using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ClinicaVeterinariaAPI.Controllers
{
    public class DoctorScheduleController : ApiController
    {
        ClinicaVeterinariaDataContext db = new ClinicaVeterinariaDataContext
           (
               ConfigurationManager
                   .ConnectionStrings["ClinicaVeterinariaConnectionString"].ConnectionString
           );

        // GET: api/DoctorSchedule
        /// <summary>
        /// All DoctorSchedules
        /// </summary>
        /// <returns>DoctorSchedules list</returns>
        public List<DoctorSchedule> Get()
        {
            var list = from DoctorSchedule in db.DoctorSchedules orderby DoctorSchedule.Id select DoctorSchedule;
            return list.ToList();
        }

        // GET: api/DoctorSchedule/5
        /// <summary>
        /// Specific (one) DoctorSchedule
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DoctorSchedule</returns>
        public IHttpActionResult Get(int id)
        {
            var doctorSchedule = db.DoctorSchedules.FirstOrDefault(ds => ds.Id == id);

            if (doctorSchedule == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Horário não existe"));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, doctorSchedule));
        }

        // POST: api/DoctorSchedule
        /// <summary>
        /// Creates a new DoctorSchedule
        /// </summary>
        /// <param name="newDoctorSchedule">DoctorSchedule</param>
        public IHttpActionResult Post([FromBody] DoctorSchedule newDoctorSchedule)
        {
            var doctorSchedule = db.DoctorSchedules.FirstOrDefault(ds => ds.Id == newDoctorSchedule.Id);
            var doctor = db.Doctors.FirstOrDefault(d => d.Id == newDoctorSchedule.DoctorId);

            if (doctorSchedule != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Já existe um horário registado com esse id"));
            }

            if (doctor == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                        "Não existe um doutor registado com esse id"));
            }

            db.DoctorSchedules.InsertOnSubmit(newDoctorSchedule);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e.Message));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, newDoctorSchedule));
        }

        // PUT: api/DoctorSchedule/5
        /// <summary>
        /// Updates (one) DoctorSchedule
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedDoctorSchedule">DoctorSchedule</param>
        public IHttpActionResult Put(int id, [FromBody] DoctorSchedule updatedDoctorSchedule)
        {
            var doctorSchedule = db.DoctorSchedules.FirstOrDefault(ds => ds.Id == updatedDoctorSchedule.Id);
            var doctor = db.Doctors.FirstOrDefault(d => d.Id == updatedDoctorSchedule.DoctorId);

            if (doctorSchedule == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não existe um horário registado com esse id"));
            }

            if (doctor == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não existe um doutor registado com esse id"));
            }

            doctorSchedule.DoctorId = updatedDoctorSchedule.DoctorId;
            doctorSchedule.DayOfWeek = updatedDoctorSchedule.DayOfWeek;
            doctorSchedule.StartTime = updatedDoctorSchedule.StartTime;
            doctorSchedule.EndTime = updatedDoctorSchedule.EndTime;

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

        // DELETE: api/DoctorSchedule/5
        /// <summary>
        /// Deletes (one) DoctorSchedule
        /// </summary>
        /// <param name="id"></param>
        public IHttpActionResult Delete(int id)
        {
            var doctorSchedule = db.DoctorSchedules.FirstOrDefault(ds => ds.Id == id);

            if (doctorSchedule == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                   "Não existe nenhum horário com esse ID para poder eliminar"));
            }

            db.DoctorSchedules.DeleteOnSubmit(doctorSchedule);

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

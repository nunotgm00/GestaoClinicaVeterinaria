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
        public List<DoctorSchedule> Get()
        {
            var list = from DoctorSchedule in db.DoctorSchedules orderby DoctorSchedule.Id select DoctorSchedule;
            return list.ToList();
        }

        // GET: api/DoctorSchedule/5
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

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        // PUT: api/DoctorSchedule/5
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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ClinicaVeterinariaAPI.Controllers
{
    public class DoctorController : ApiController
    {
        ClinicaVeterinariaDataContext db = new ClinicaVeterinariaDataContext
            (
                ConfigurationManager
                    .ConnectionStrings["ClinicaVeterinariaConnectionString"].ConnectionString
            );

        // GET: api/Doctor
        /// <summary>
        /// All Doctors
        /// </summary>
        /// <returns>Doctors list</returns>
        public List<Doctor> Get()
        {
            var list = from Doctor in db.Doctors orderby Doctor.Name select Doctor;
            return list.ToList();
        }

        // GET: api/Doctor/5
        /// <summary>
        /// Specific (one) Doctor
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Doctor</returns>
        public IHttpActionResult Get(int id)
        {
            var doctor = db.Doctors.FirstOrDefault(d => d.Id == id);

            if( doctor == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Doutor não existe"));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, doctor));
        }

        // POST: api/Doctor
        /// <summary>
        /// Creates a new Doctor
        /// </summary>
        /// <param name="newDoctor">Doctor</param>
        public IHttpActionResult Post([FromBody]Doctor newDoctor)
        {
            var doctor = db.Doctors.FirstOrDefault(r => r.Id == newDoctor.Id);

            if( doctor != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Já existe um doutor registado com esse id"));
            }

            db.Doctors.InsertOnSubmit(newDoctor);

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

        // PUT: api/Doctor/5
        /// <summary>
        /// Updates (one) Doctor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedDoctor">Doctor</param>
        public IHttpActionResult Put(int id, [FromBody]Doctor updatedDoctor)
        {
            var doctor = db.Doctors.FirstOrDefault(d => d.Id == id);

            if( doctor == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não existe nenhum doutor com esse ID para poder alterar"));
            }

            doctor.Name = updatedDoctor.Name;
            doctor.PhoneNumber = updatedDoctor.PhoneNumber;
            doctor.Speciality = updatedDoctor.Speciality;
            doctor.Active = updatedDoctor.Active;

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

        // DELETE: api/Doctor/5
        /// <summary>
        /// Deletes (one) Doctor
        /// </summary>
        /// <param name="id"></param>
        public IHttpActionResult Delete(int id)
        {
            var doctor = db.Doctors.FirstOrDefault(d => d.Id == id);

            if( doctor == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum doutor com esse ID para poder eliminar"));
            }

            db.Doctors.DeleteOnSubmit(doctor);

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

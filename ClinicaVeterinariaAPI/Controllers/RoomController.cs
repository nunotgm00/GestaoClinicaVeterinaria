using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ClinicaVeterinariaAPI.Controllers
{
    public class RoomController : ApiController
    {
        ClinicaVeterinariaDataContext db = new ClinicaVeterinariaDataContext
            (
                ConfigurationManager
                    .ConnectionStrings["ClinicaVeterinariaConnectionString"].ConnectionString
            );

        // GET: api/Room
        /// <summary>
        /// All Rooms
        /// </summary>
        /// <returns>Rooms list</returns>
        public List<Room> Get()
        {
           var list = from Room in db.Rooms orderby Room.Id select Room;
            return list.ToList();
        }

        // GET: api/Room/5
        /// <summary>
        /// Specific (one) Room
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Room</returns>
        public IHttpActionResult Get(int id)
        {
            var room = db.Rooms.FirstOrDefault( r => r.Id == id );

            if ( room == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "A sala não existe"));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, room));
        }

        // POST: api/Room
        /// <summary>
        /// Creates a new Room
        /// </summary>
        /// <param name="newRoom">Room</param>
        public IHttpActionResult Post([FromBody]Room newRoom)
        {
            var room = db.Rooms.FirstOrDefault(r => r.Id == newRoom.Id);

            if ( room != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Já existe uma sala registada com esse id"));
            }

            db.Rooms.InsertOnSubmit(newRoom);

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

        // PUT: api/Room/5
        /// <summary>
        /// Updates (one) Room
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedRoom">Room</param>
        public IHttpActionResult Put(int id, [FromBody]Room updatedRoom)
        {
            var room = db.Rooms.FirstOrDefault(r =>r.Id == updatedRoom.Id);

            if ( room == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não existe nenhuma sala com esse ID para poder alterar"));
            }

            room.Type = updatedRoom.Type;
            room.UnderMaintenance = updatedRoom.UnderMaintenance;

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

        // DELETE: api/Room/5
        /// <summary>
        /// Deletes (one) Room
        /// </summary>
        /// <param name="id"></param>
        public IHttpActionResult Delete(int id)
        {
            var room = db.Rooms.FirstOrDefault(r => r.Id == id);

            if( room == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhuma sala com esse ID para poder eliminar"));
            }

            db.Rooms.DeleteOnSubmit(room);

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

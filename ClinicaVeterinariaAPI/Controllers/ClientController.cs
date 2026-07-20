using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ClinicaVeterinariaAPI.Controllers
{
    public class ClientController : ApiController
    {
        ClinicaVeterinariaDataContext db = new ClinicaVeterinariaDataContext
            (
                ConfigurationManager
                    .ConnectionStrings["ClinicaVeterinariaConnectionString"].ConnectionString
            );

        // GET: api/Client
        /// <summary>
        /// All Clients
        /// </summary>
        /// <returns>Clients ist</returns>
        public List<Client> Get()
        {
            var list = from Client in db.Clients orderby Client.Name select Client;
            return list.ToList();
        }

        // GET: api/Client/5
        /// <summary>
        /// Specific (one) Client
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Client</returns>
        public IHttpActionResult Get(int id)
        {
            var client = db.Clients.FirstOrDefault(c => c.Id == id);
            if (client == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, 
                    "Cliente não existe"));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, client));
        }

        // POST: api/Client
        /// <summary>
        /// Creates a new Client
        /// </summary>
        /// <param name="newClient">Client</param>
        public IHttpActionResult Post([FromBody]Client newClient)
        {
            var client = db.Clients.FirstOrDefault(c => c.Id == newClient.Id);

            if(client != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, 
                    "Já existe um cliente registado com esse id"));
            }

            db.Clients.InsertOnSubmit(newClient);

            try
            {
                db.SubmitChanges();
            }
            catch(Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e.Message));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        // PUT: api/Client/5
        /// <summary>
        /// Updates (one) Client
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedClient">Client</param>
        public IHttpActionResult Put(int id, [FromBody]Client updatedClient)
        {
            var client = db.Clients.FirstOrDefault(c => c.Id == id);
            if(client == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum cliente com esse ID para poder alterar"));
            }

            client.Name = updatedClient.Name;
            client.Address = updatedClient.Address;
            client.Nif = updatedClient.Nif;
            client.PhoneNumber = updatedClient.PhoneNumber;
            client.Email = updatedClient.Email;

            try
            {
                db.SubmitChanges();
            }
            catch(Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e.Message));

            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        // DELETE: api/Client/5
        /// <summary>
        /// Deletes (one) Client
        /// </summary>
        /// <param name="id"></param>
        public IHttpActionResult Delete(int id)
        {
            var client = db.Clients.FirstOrDefault(c => c.Id == id);

            if( client == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum cliente com esse ID para poder eliminar"));
            }

            db.Clients.DeleteOnSubmit(client);

            try
            {
                db.SubmitChanges();
            }
            catch(Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e.Message));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }
    }
}

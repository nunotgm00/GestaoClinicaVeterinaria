using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ClinicaVeterinariaAPI.Controllers
{
    public class AnimalController : ApiController
    {
        ClinicaVeterinariaDataContext db = new ClinicaVeterinariaDataContext
            (
                ConfigurationManager
                    .ConnectionStrings["ClinicaVeterinariaConnectionString"].ConnectionString
            );

        // GET: api/Animal
        public List<Animal> Get()
        {
            var list = from Animal in db.Animals orderby Animal.Name select Animal;
            return list.ToList();
        }

        // GET: api/Animal/5
        public IHttpActionResult Get(int id)
        {
            var animal = db.Animals.FirstOrDefault(a => a.Id == id);
            if(animal == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, 
                    "Animal não existe"));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, animal));
        }

        // POST: api/Animal
        public IHttpActionResult Post([FromBody]Animal newAnimal)
        {
            var animal = db.Animals.FirstOrDefault(a => a.Id == newAnimal.Id);

            if( animal != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, 
                    "Já existe um animal registado com esse id"));
            }

            if (newAnimal.ClientId.HasValue)
            {
                var client = db.Clients.FirstOrDefault(c => c.Id == newAnimal.ClientId);
                if (client == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                        "Não existe um cliente registado com esse id"));
                }
            }

            db.Animals.InsertOnSubmit(newAnimal);

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

        // PUT: api/Animal/5
        public IHttpActionResult Put(int id, [FromBody]Animal updatedAnimal)
        {
            var animal = db.Animals.FirstOrDefault(a => a.Id == updatedAnimal.Id);

            if (animal == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não existe nenhum animal com esse ID para poder alterar"));
            }

            if (updatedAnimal.ClientId.HasValue)
            {
                var client = db.Clients.FirstOrDefault(c => c.Id == updatedAnimal.ClientId);
                if (client == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                        "Não existe um cliente registado com esse id"));
                }
            }

            animal.Name = updatedAnimal.Name;
            animal.Species = updatedAnimal.Species;
            animal.Breed = updatedAnimal.Breed;
            animal.Age = updatedAnimal.Age;
            animal.Weight = updatedAnimal.Weight;
            animal.Color = updatedAnimal.Color;
            animal.Sex = updatedAnimal.Sex;
            animal.ClientId = updatedAnimal.ClientId;

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

        // DELETE: api/Animal/5
        public IHttpActionResult Delete(int id)
        {
            var animal = db.Animals.FirstOrDefault(a => a.Id == id);

            if(animal == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                   "Não existe nenhum animal com esse ID para poder eliminar"));
            }

            db.Animals.DeleteOnSubmit(animal);

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

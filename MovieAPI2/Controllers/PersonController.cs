using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI2.Data;
using MovieAPI2.Entities;
using MovieAPI2.Model;
using System.Collections.Generic;

namespace MovieAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly MovieDBContext _dbContext;
        private readonly IMapper _mapper;
        public PersonController(MovieDBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                var actorCount = _dbContext.Movie.Count();
                var actorList = _mapper.Map<List<ActorViewModel>> (_dbContext.Person.Skip(pageIndex * pageSize).Take(pageSize).ToList());

                //.Select(x => new ActorViewModel
                // {
                //     Id = x.Id,
                //     Name = x.Name,
                //     DateOfBirth = x.DateOfBirth,

                // }).
                response.Status = true;
                response.Message = "Success";
                response.Data = new
                {
                    Person = actorList,
                    Count = actorCount

                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // TODO; do logging exception
                response.Status = false;
                response.Message = "Somthing went wrong";
                return BadRequest(response);
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetPersonById(int id)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {

                var person = _dbContext.Person.Where(x => x.Id == id).FirstOrDefault();
                if (person == null)
                {
                    response.Status = false;
                    response.Message = " Record not exist.";
                    return BadRequest(response);
                }
                var personData = new ActorDetailsViewModel
                {
                    Id = person.Id,
                    DateOfBirth = person.DateOfBirth,
                    Name = person.Name,
                    Movies = _dbContext.Movie.Where(x => x.Actors.Contains(person)).Select(x => x.Title).ToArray(),
                };
                response.Status = true;
                response.Message = "Success";
                response.Data = personData;
                return Ok(response);
            }
            catch (Exception ex)
            {
                // TODO; do logging exception
                response.Status = false;
                response.Message = "Somthing went wrong";
                return BadRequest(response);
            }

        }

        [HttpGet]
        [Route("Search/{searchText}")]
        public IActionResult Get(string searchText) {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                var searchedPerson = _dbContext.Person.Where(x => x.Name.Contains(searchText)).Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList( );

                response.Status = true;
                response.Message = "Success";
                response.Data = searchedPerson;

                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Somthing went wrong";
                return BadRequest(response);
            }
        }

        [HttpPost]
        public IActionResult Post(ActorViewModel model)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                if (ModelState.IsValid)
                {
                    var postedModel = new Person()
                    {
                        Name = model.Name,
                        DateOfBirth = model.DateOfBirth
                    };
                    _dbContext.Person.Add(postedModel);
                    _dbContext.SaveChanges();

                    model.Id = postedModel.Id;

             

                    response.Status = true;
                    response.Message = "Created Successfully";
                    response.Data = model;

                    return Ok(response);
                }
                else
                {
                    response.Status = false;
                    response.Message = "Validationn failed";
                    response.Data = ModelState;
                    return BadRequest(response);
                }

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Something went wrong";
                return BadRequest(response);

            }
        }


        [HttpPut]
        public IActionResult Put(ActorViewModel model)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                if (ModelState.IsValid)
                {
                    var postedModel = _mapper.Map<Person>(model);

                    var personDetail = _dbContext.Person.Where(x => x.Id == model.Id).AsNoTracking().FirstOrDefault();
                    if (personDetail == null)
                    {
                        response.Status = false;
                        response.Message = "Invalid Person Record";
                        return BadRequest(response);
                    }
                    _dbContext.Update(postedModel);
                    _dbContext.SaveChanges();

                    response.Status = true;
                    response.Message = "Update successfully";
                    response.Data = postedModel;
                    return Ok(response);

                }

                response.Status = false;
                response.Message = "Something went wrong";
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Something went wrong";
                return BadRequest(response);
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                var person = _dbContext.Person.Where(x => x.Id == id).FirstOrDefault();
                if (person == null)
                {
                    response.Status = false;
                    response.Message = " Invalid Person Record.";
                    return BadRequest(response);
                }
                _dbContext.Remove(person);
                _dbContext.SaveChanges();
                response.Status = true;
                response.Message = "Delete successfully";
                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Something went wrong";
                return BadRequest(response);
            }
        }

    }
}

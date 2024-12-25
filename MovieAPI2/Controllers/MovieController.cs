using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI2.Data;
using MovieAPI2.Entities;
using MovieAPI2.Model;
using System.Net.Http.Headers;

namespace MovieAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieDBContext _dbContext;
        private readonly IMapper _mapper;
        public MovieController(MovieDBContext dbContext, IMapper mapper)
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
                var movieCount = _dbContext.Movie.Count();
                var movieList = _dbContext.Movie.Include(x => x.Actors).Skip(pageIndex * pageSize).Take(pageSize)
                    .Select(x => new MovieListViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Actors = x.Actors.Select(y => new ActorViewModel
                        {
                            Id = y.Id,
                            Name = y.Name,
                            DateOfBirth = y.DateOfBirth,
                        }).ToList(),
                        CoverImage = x.CoverImage,
                        Language = x.Language,
                        ReleaseDate = x.ReleaseDate,
                        Description = x.Description,

                    }).
                    ToList();
                response.Status = true;
                response.Message = "Success";
                response.Data = new
                {
                    Movie = movieList,
                    Count = movieCount

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
        public IActionResult GetMovieById(int id)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {

                var movie = _dbContext.Movie.Include(x => x.Actors).Where(x => x.Id == id).FirstOrDefault();
                if (movie == null)
                {
                    response.Status = false;
                    response.Message = " Record not exist.";
                    return BadRequest(response);
                }
                var movieData = _mapper.Map<MovieDetailsViewModel>(movie);
                response.Status = true;
                response.Message = "Success";
                response.Data = movieData;
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

        [HttpPost]
        public IActionResult Post(CreateViewModel model)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                if (ModelState.IsValid)
                {
                    var actors = _dbContext.Person.Where(x => model.Actors.Contains(x.Id)).ToList();
                    if (actors.Count != model.Actors.Count) {
                        response.Status = false;
                        response.Message = "Invalid Actors";
                        return BadRequest(response);

                    }
                    var postedModel = _mapper.Map<Movie>(model);
                    postedModel.Actors = actors;

                    //new Movie()
                    //{
                    //    Title = model.Title,
                    //    ReleaseDate = model.ReleaseDate,
                    //    Language = model.Language,
                    //    CoverImage = model.CoverImage,
                    //    Description = model.Description,
                    //    Actors = actors
                    //};


                    _dbContext.Movie.Add(postedModel);
                    _dbContext.SaveChanges();

                    var responseData = _mapper.Map<MovieDetailsViewModel>(postedModel);

                    //new MovieListViewModel
                    //{
                    //    Id = postedModel.Id,
                    //    Title = postedModel.Title,
                    //    Actors = postedModel.Actors.Select(y => new ActorViewModel
                    //    {
                    //        Id = y.Id,
                    //        Name = y.Name,
                    //        DateOfBirth = y.DateOfBirth,
                    //    }).ToList(),
                    //    CoverImage = postedModel.CoverImage,
                    //    Language = postedModel.Language,
                    //    ReleaseDate = postedModel.ReleaseDate,
                    //    Description = postedModel.Description,

                    //};


                    response.Status = true;
                    response.Message = "Created Successfully";
                    response.Data = responseData;

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
        public IActionResult Put(CreateViewModel model)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id <= 0)
                    {
                        response.Status = false;
                        response.Message = "Invalid Movie Record";
                        return BadRequest(response);
                    }
                    var actors = _dbContext.Person.Where(x => model.Actors.Contains(x.Id)).ToList();
                    if (actors.Count != model.Actors.Count)
                    {
                        response.Status = false;
                        response.Message = "Invalid Actors";
                        return BadRequest(response);

                    }
                    var movieDetails = _dbContext.Movie.Include(x => x.Actors).Where(x => x.Id == model.Id).FirstOrDefault();

                    if (movieDetails == null)
                    {
                        response.Status = false;
                        response.Message = "Invalid Movie Record";
                        return BadRequest(response);
                    }

                    movieDetails.CoverImage = model.CoverImage;
                    movieDetails.Description = model.Description;
                    movieDetails.Language = model.Language;
                    movieDetails.Title = model.Title;
                    movieDetails.ReleaseDate = model.ReleaseDate;

                    // find removed actors
                    var removedActor = movieDetails.Actors.Where(x => !model.Actors.Contains(x.Id)).ToList();

                    foreach (var actor in removedActor)
                    {
                        movieDetails.Actors.Remove(actor);
                    }

                    // find added Actors

                    var addedActorIds = actors.Except(movieDetails.Actors).ToList();

                    foreach (var actor in addedActorIds)
                    {
                        movieDetails.Actors.Add(actor);
                    }

                    _dbContext.SaveChanges();

                    var responseData = new MovieListViewModel
                    {
                        Id = movieDetails.Id,
                        Title = movieDetails.Title,
                        Actors = movieDetails.Actors.Select(y => new ActorViewModel
                        {
                            Id = y.Id,
                            Name = y.Name,
                            DateOfBirth = y.DateOfBirth,
                        }).ToList(),
                        CoverImage = movieDetails.CoverImage,
                        Language = movieDetails.Language,
                        ReleaseDate = movieDetails.ReleaseDate,
                        Description = movieDetails.Description,

                    };


                    response.Status = true;
                    response.Message = "Update Successfully";
                    response.Data = responseData;

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

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                var movie = _dbContext.Movie.Where(x => x.Id == id).FirstOrDefault();
                if (movie == null)
                {
                    response.Status = false;
                    response.Message = " Invalid Movie Record.";
                    return BadRequest(response);
                }
                _dbContext.Remove(movie);
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
        [HttpPost]
        [Route("upload-movie-poster")]
        public async Task<IActionResult> UploadMoviePoster(IFormFile imageFile)
        {
            try
            {
                var filename = ContentDispositionHeaderValue.Parse(imageFile.ContentDisposition).FileName.TrimStart('\"').TrimEnd('\"');
                string newPath = @"E:\to-delete";
                string extension = Path.GetExtension(filename);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                string[] allowedImageExtensions = new string[] { ".jpg", ".jpeg", ".png" };
                if (!allowedImageExtensions.Contains(extension))
                {
                    return BadRequest(new BaseResponseModel
                    {
                        Status = false,
                        Message = "Ony jpg, jpeg, png are allowed",
                    });
                }
                string newFileName = Guid.NewGuid() + extension;
                string fullFilePath = Path.Combine(newPath, newFileName);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);

                }
                return Ok(new
                {
                    ProfileImage = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/StaticFiles/{newFileName}",
                });


            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel
                {
                    Status = false,
                    Message = ex.Message,
                });
            }
        }
    }
}

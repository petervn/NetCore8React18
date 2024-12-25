using AutoMapper;
using MovieAPI2.Entities;
using MovieAPI2.Model;

namespace MovieAPI2
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Movie, MovieListViewModel>();
            CreateMap<Movie, MovieDetailsViewModel>();

            CreateMap<MovieListViewModel, Movie>();
            CreateMap<CreateViewModel, Movie>().ForMember(x => x.Actors, y => y.Ignore());

            CreateMap<Person, ActorViewModel>();
            CreateMap<Person, ActorDetailsViewModel>();

            CreateMap<ActorViewModel, Person>();
        }

    }
}

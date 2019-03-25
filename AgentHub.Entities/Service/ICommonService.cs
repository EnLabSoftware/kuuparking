using System.Collections.Generic;
using System.Threading.Tasks;
using AgentHub.Entities.Models.Common;

namespace AgentHub.Entities.Service
{
    public interface ICommonService : IBaseService<Country>
    {
        Task<List<Country>> GetCountries();
        Task<List<State>> GetStates(int countryId);
        Task<List<City>> GetCities(int stateId);
        Task<List<District>> GetDistricts(int cityId);
        Task<Country> GetCountry(string country);
        Task<State> GetState(string state, int? countryId);
        Task<State> GetState(int stateId);
        Task<City> GetCity(string city, int? stateId);
        Task<City> GetCity(int cityId);
        Task<District> GetDistrict(string district, int? cityId);
        void UpdateAddress(Address address);
        Task<UserProfile> GetUserProfile(string email);
        void UpdateUserProfile(UserProfile userProfile);
    }
}

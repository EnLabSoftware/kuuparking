using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Repositories;
using AgentHub.Entities.Service;
using AgentHub.Entities.UnitOfWork;

namespace AgentHub.Service
{
    public class CommonService : BaseService<Country>, ICommonService
    {
        private readonly IRepositoryAsync<State> _stateRepositoryAsync;
        private readonly IRepositoryAsync<City> _cityRepositoryAsync;
        private readonly IRepositoryAsync<District> _districtRepositoryAsync;
        private readonly IRepositoryAsync<Address> _addressRepositoryAsync;
        private readonly IRepositoryAsync<UserProfile> _userProfileRepositoryAsync;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public CommonService(IRepositoryAsync<Country> repository, IRepositoryAsync<State> stateRepositoryAsync,
            IRepositoryAsync<City> cityRepositoryAsync, IRepositoryAsync<District> districtRepositoryAsync,
            IRepositoryAsync<Address> addressRepositoryAsync, IRepositoryAsync<UserProfile> userProfileRepository, IUnitOfWorkAsync unitOfWorkAsync)
            : base(repository)
        {
            _stateRepositoryAsync = stateRepositoryAsync;
            _cityRepositoryAsync = cityRepositoryAsync;
            _districtRepositoryAsync = districtRepositoryAsync;
            _addressRepositoryAsync = addressRepositoryAsync;
            _userProfileRepositoryAsync = userProfileRepository;
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        public Task<List<Country>> GetCountries()
        {
            var countries = Queryable().ToList();

            return Task.FromResult(countries);
        }

        public Task<List<State>> GetStates(int countryId)
        {
            var states = _stateRepositoryAsync.Queryable().Where(_ => _.CountryId == countryId).ToList();

            return Task.FromResult(states);
        }

        public Task<List<City>> GetCities(int stateId)
        {
            var cities = _cityRepositoryAsync.Queryable().Where(_ => _.StateId == stateId).ToList();

            return Task.FromResult(cities);
        }

        public Task<List<District>> GetDistricts(int cityId)
        {
            var districts = _districtRepositoryAsync.Queryable().Where(_ => _.CityId == cityId).ToList();

            return Task.FromResult(districts);
        }

        public Task<Country> GetCountry(string country)
        {
            var result = Queryable().FirstOrDefault(_ => _.Name.ToLower().Contains(country.ToLower()));

            return Task.FromResult(result);
        }

        public Task<State> GetState(string state, int? countryId)
        {
            if(string.IsNullOrEmpty(state))
                return Task.FromResult<State>(null); 

            var query = _stateRepositoryAsync.Queryable();
            if (countryId != null)
                query = query.Where(_ => _.CountryId == countryId);

            var result = query.FirstOrDefault(_ => _.Name.ToLower().Contains(state.ToLower()));

            return Task.FromResult(result);
        }

        public Task<State> GetState(int stateId)
        {
            var state = _stateRepositoryAsync.Queryable().FirstOrDefault(_ => _.ID == stateId);

            return Task.FromResult(state);
        }

        public Task<City> GetCity(string city, int? stateId)
        {
            if (string.IsNullOrEmpty(city))
                return Task.FromResult<City>(null);

            var query = _cityRepositoryAsync.Queryable();
            if (stateId != null)
                query = query.Where(_ => _.StateId == stateId);

            var result = query.FirstOrDefault(_ => _.Name.ToLower().Contains(city.ToLower()));

            return Task.FromResult(result);
        }

        public Task<City> GetCity(int cityId)
        {
            var result = _cityRepositoryAsync.Queryable().Include(_=>_.State).FirstOrDefault(_ => _.ID == cityId);

            return Task.FromResult(result);
        }

        public Task<District> GetDistrict(string district, int? cityId)
        {
            if (string.IsNullOrEmpty(district))
                return Task.FromResult<District>(null);

            var query = _districtRepositoryAsync.Queryable();
            if (cityId != null)
                query = query.Where(_ => _.CityId == cityId);

            var result = query.FirstOrDefault(_ => _.Name.ToLower().Contains(district.ToLower()));

            return Task.FromResult(result);
        }

        public void UpdateAddress(Address address)
        {
            _addressRepositoryAsync.Update(address);
            _unitOfWorkAsync.SaveChanges();
        }

        public Task<UserProfile> GetUserProfile(string email)
        {
            var userProfile =
                _userProfileRepositoryAsync.Queryable().FirstOrDefault(_ => _.Email == email || _.PhoneNumber == email);

            return Task.FromResult(userProfile);
        }

        public void UpdateUserProfile(UserProfile userProfile)
        {
            _userProfileRepositoryAsync.Update(userProfile);
            _unitOfWorkAsync.SaveChanges();
        }
    }
}

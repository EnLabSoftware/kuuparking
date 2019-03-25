using System.Linq;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Repositories;
using AgentHub.Entities.ViewModels;

namespace AgentHub.Repository.Repositories
{
    public static class CommonRepository
    {
        public static AddressViewModels GetAddressViewModelByDistrictId(this IRepositoryAsync<District> repository, int districtId)
        {
            var addressViewModel =
                    repository.Queryable()
                        .Where(_ => _.ID == districtId)
                        .Select(_ => new AddressViewModels { DistrictName = _.Name, CityName = _.City.Name, StateName = _.City.State.Name, CountryName = _.City.State.Country.Name}).FirstOrDefault();

            return addressViewModel;
        }

        public static AddressViewModels GetAddressViewModelByCityId(this IRepositoryAsync<City> repository, int cityId)
        {
            var addressViewModel =
                    repository.Queryable()
                        .Where(_ => _.ID == cityId)
                        .Select(_ => new AddressViewModels { CityName = _.Name, StateName = _.State.Name, CountryName = _.State.Country.Name }).FirstOrDefault();

            return addressViewModel;
        }

        public static AddressViewModels GetAddressViewModelByStateId(this IRepositoryAsync<State> repository, int stateId)
        {
            var addressViewModel =
                    repository.Queryable()
                        .Where(_ => _.ID == stateId)
                        .Select(_ => new AddressViewModels { StateName = _.Name, CountryName = _.Country.Name }).FirstOrDefault();

            return addressViewModel;
        }
        public static AddressViewModels GetAddressViewModelByCountryId(this IRepositoryAsync<Country> repository, int countryId)
        {
            var addressViewModel =
                    repository.Queryable()
                        .Where(_ => _.ID == countryId)
                        .Select(_ => new AddressViewModels {CountryName = _.Name }).FirstOrDefault();

            return addressViewModel;
        }
    }
}

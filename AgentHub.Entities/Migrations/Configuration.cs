using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AgentHub.Entities.Infrastructure;
using AgentHub.Entities.Models;
using AgentHub.Entities.Models.Application;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Models.KuuParking;
using AgentHub.Entities.Utilities;

namespace AgentHub.Entities.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AgentHubDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(AgentHubDataContext context)
        {
            InitDevelopmentApplicationService(context);

            ImportAddressData(context);

            InitUserProfiles(context);

            InitSlotProviders(context);

            InitPages(context);
        }

        private void InitDevelopmentApplicationService(AgentHubDataContext context)
        {
            var devApplication = new Application()
            {
                ID = 1,
                ActivatedOn = DateTime.Now,
                ApplicationDescription = "Development purpose",
                ApplicationKey = "E6C4AC5F-7CFE-4B3B-ADBB-4EE9A6AD86FC",
                ApplicationName = "Development application",
                RequestedOn = DateTime.Now,
                ObjectState = ObjectState.Added
            };
            context.Applications.AddOrUpdate(devApplication);

            var devService = new ApplicationService()
            {
                ID = 1,
                ApplicationId = devApplication.ID,
                ServiceKey = "857E30F4-26D0-46CC-8679-D440E5538C4C",
                ServiceDescription = "Development purpose",
                ServiceName = "Development service",
                CultureInfoCode = "vi-VN",
                AllowedDomains = "localhost;192.168.2.100;dev.kuuparking.com;kuuparking.com;www.kuuparking.com;",
                ObjectState = ObjectState.Added
            };
            context.ApplicationServices.AddOrUpdate(devService);
            context.SaveChanges();
        }

        private void ImportAddressData(AgentHubDataContext context)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();

                    var addressBookResource = "AgentHub.Entities.AddressesBook.txt";
#if DEBUG
                    addressBookResource = "AgentHub.Entities.AddressesBookDevelopment.txt";
#endif
                    using (var stream = assembly.GetManifestResourceStream(addressBookResource))
                    {
                        if (stream == null)
                            return;

                        using (var reader = new StreamReader(stream))
                        {
                            //
                            // Create the country first
                            var country = new Country() { ID = 1, Name = "Vietnam", PhoneCallCode = "+84", ObjectState = ObjectState.Added };
                            context.Countries.AddOrUpdate(country);
                            context.SaveChanges();

                            var readLine = reader.ReadLine();
                            var state = new State();
                            var stateId = 1;
                            var city = new City();
                            var cityId = 1;
                            var district = new District();
                            var districtId = 1;

                            while (readLine != null)
                            {
                                var cells = readLine.Split(',');

                                //
                                // Create the state first
                                if (state.Name != cells[0])
                                {
                                    state = new State
                                    {
                                        ID = stateId++,
                                        Name = cells[0],
                                        ReferenceCode = cells[1],
                                        PhoneCallCode = cells[2],
                                        CountryId = country.ID,
                                        ObjectState = ObjectState.Added
                                    };
                                    context.States.AddOrUpdate(state);
                                    context.SaveChanges();
                                }
                                //
                                // Create city second
                                if (city.Name != cells[3])
                                {
                                    city = new City
                                    {
                                        ID = cityId++,
                                        Name = cells[3],
                                        ReferenceCode = cells[4],
                                        StateId = state.ID,
                                        ObjectState = ObjectState.Added
                                    };
                                    context.Cities.AddOrUpdate(city);
                                    context.SaveChanges();
                                }
                                //
                                // Create district third: Will be saved in the next state or at the end of the import process
                                if (district.Name != cells[5])
                                {
                                    district = new District()
                                    {
                                        ID = districtId++,
                                        Name = cells[5],
                                        ReferenceCode = cells[6],
                                        CityId = city.ID,
                                        ObjectState = ObjectState.Added
                                    };
                                    context.Districts.AddOrUpdate(district);
                                }
                                readLine = reader.ReadLine();
                            }
                            //
                            // Save other pending changes left if any
                            context.SaveChanges();
                        }
                    }
                    //
                    // Other address sample data
                    var address = new Address()
                    {
                        ID = 1,
                        Street = "Tân An 4",
                        ZipCode = "59000",
                        District = context.Districts.FirstOrDefault(_ => _.Name == "Phường Khuê Trung" && _.City.Name == "Quận Cẩm Lệ"),
                        City = context.Cities.FirstOrDefault(_ => _.Name == "Quận Cẩm Lệ" && _.State.Name == "Thành phố Đà Nẵng"),
                        State = context.States.FirstOrDefault(_ => _.Name == "Thành phố Đà Nẵng"),
                        Country = context.Countries.Find(1),
                        ObjectState = ObjectState.Added
                    };
                    address.AddressLine = address.ToString();
                    context.Addresses.AddOrUpdate(address);
                    address = new Address()
                    {
                        ID = 2,
                        Street = "Lương Nhữ Hộc",
                        ZipCode = "59000",
                        District = context.Districts.FirstOrDefault(_ => _.Name == "Phường Hòa Cường Bắc" && _.City.Name == "Quận Hải Châu"),
                        City = context.Cities.FirstOrDefault(_ => _.Name == "Quận Hải Châu" && _.State.Name == "Thành phố Đà Nẵng"),
                        State = context.States.FirstOrDefault(_ => _.Name == "Thành phố Đà Nẵng"),
                        Country = context.Countries.Find(1),
                        ObjectState = ObjectState.Added
                    };
                    address.AddressLine = address.ToString();
                    context.Addresses.AddOrUpdate(address);
                    address = new Address()
                    {
                        ID = 3,
                        StreetNumber = "526",
                        Street = "Cửa Đại",
                        ZipCode = "50000",
                        District = context.Districts.FirstOrDefault(_ => _.Name == "Phường Sơn Phong" && _.City.Name == "Thành phố Hội An"),
                        City = context.Cities.FirstOrDefault(_ => _.Name == "Thành phố Hội An" && _.State.Name == "Tỉnh Quảng Nam"),
                        State = context.States.FirstOrDefault(_ => _.Name == "Tỉnh Quảng Nam"),
                        Country = context.Countries.Find(1),
                        ObjectState = ObjectState.Added
                    };
                    address.AddressLine = address.ToString();
                    context.Addresses.AddOrUpdate(address);
                    context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    LogHelper.LogException(exception);
                    transaction.Rollback();
                }
            }
        }

        private void InitUserProfiles(AgentHubDataContext context)
        {
            var user = new UserProfile()
            {
                ID = 1,
                Email = "vinh.tran@enterpriselab.net",
                FirstName = "Vinh",
                LastName = "Tran",
                BillingAddressId = 1,
                UserProfileType = UserProfileType.Provider,
                ObjectState = ObjectState.Added
            };
            context.Users.AddOrUpdate(user);

            user = new UserProfile()
            {
                ID = 2,
                Email = "thai.truong@enterpriselab.net",
                FirstName = "Thai",
                LastName = "Truong",
                BillingAddressId = 3,
                UserProfileType = UserProfileType.Provider,
                ObjectState = ObjectState.Added
            };
            context.Users.AddOrUpdate(user);
            context.SaveChanges();
        }

        private void InitSlotProviders(AgentHubDataContext context)
        {
            var provider = new SlotProvider()
            {
                ID = 1,
                SlotOwnerId = 1,
                Name = "Tân An 4, Khuê Trung, Đà Nẵng",
                MaximumSlots = 5,
                Latitude = 16.0375061,
                Longitude = 108.2125714,
                AddressId = 1,
                Price = 20000,
                Rating = 3,
                AvailableFromTime = DateTime.Now,
                AvailableToTime = DateTime.Now,
                IsBusParking = true,
                IsCoveredParking = true,
                IsOpen247 = true,
                IsOvernightParking = true,
                ImageLocation1 = "/ProviderImages/test/1.jpg",
                ImageLocation2 = "/ProviderImages/test/2.jpg",
                ImageLocation3 = "/ProviderImages/test/3.jpg",
                ObjectState = ObjectState.Added
            };
            AddSlotProvider(context, provider);

            provider = new SlotProvider()
            {
                ID = 2,
                SlotOwnerId = 1,
                Name = "Lương Nhữ Hộc, Hải Châu, Đà Nẵng",
                MaximumSlots = 10,
                Latitude = 16.039100,
                Longitude = 108.212104,
                AddressId = 2,
                Price = 30000,
                Rating = (decimal?)4.5,
                AvailableFromTime = DateTime.Now,
                AvailableToTime = DateTime.Now,
                IsBusParking = true,
                IsCoveredParking = true,
                IsOpen247 = true,
                IsOvernightParking = true,
                ImageLocation1 = "/ProviderImages/test/4.jpg",
                ImageLocation2 = "/ProviderImages/test/5.jpg",
                ImageLocation3 = "/ProviderImages/test/6.jpg",
                ObjectState = ObjectState.Added
            };
            AddSlotProvider(context, provider);

            provider = new SlotProvider()
            {
                ID = 3,
                SlotOwnerId = 2,
                Name = "526 Cửa Đại, Sơn Phong, tp. Hội An",
                MaximumSlots = 3,
                Latitude = 15.880920,
                Longitude = 108.339689,
                AddressId = 3,
                Price = 25000,
                Rating = 5,
                AvailableFromTime = DateTime.Now,
                AvailableToTime = DateTime.Now,
                IsBusParking = true,
                IsCoveredParking = true,
                IsOpen247 = true,
                IsOvernightParking = true,
                ImageLocation1 = "/ProviderImages/test/7.jpg",
                ImageLocation2 = "/ProviderImages/test/8.jpg",
                ImageLocation3 = "/ProviderImages/test/9.jpg",
                ObjectState = ObjectState.Added
            };
            AddSlotProvider(context, provider);
        }

        private void AddSlotProvider(AgentHubDataContext context, SlotProvider provider)
        {
            context.SlotProviders.AddOrUpdate(provider);
            context.SaveChanges();

            for (var i = 1; i <= provider.MaximumSlots; i++)
            {
                context.Slots.AddOrUpdate(new Slot()
                {
                    Name = String.Format("{0} {1}", StringTable.Number, i),
                    Price = provider.Price,
                    SlotProviderId = provider.ID,
                    ObjectState = ObjectState.Added
                });
            }
            context.SaveChanges();
        }

        private void InitPages(AgentHubDataContext context)
        {
            var page = new PageItem()
            {
                ID = 1,
                Title = StringTable.HomePageTitle,
                Description = StringTable.HomePageDescription,
                FriendlyUrl = "Home",
                ControllerName = "Home",
                ActionName = "Index",
                ObjectState = ObjectState.Added
            };
            context.PageItems.AddOrUpdate(page);

            page = new PageItem()
            {
                ID = 2,
                Title = StringTable.HomePageTitle,
                Description = StringTable.HomePageDescription,
                FriendlyUrl = "bai-dau-xe/dang-nhap",
                ControllerName = "Home",
                ActionName = "ProviderLogin",
                ObjectState = ObjectState.Added
            };
            context.PageItems.AddOrUpdate(page);

            page = new PageItem()
            {
                ID = 3,
                Title = StringTable.ProviderRegisterPageTitle,
                Description = StringTable.ProviderRegisterPageDescription,
                FriendlyUrl = "bai-dau-xe/dang-ky",
                ControllerName = "Provider",
                ActionName = "Register",
                ObjectState = ObjectState.Added
            };
            context.PageItems.AddOrUpdate(page);

            page = new PageItem()
            {
                ID = 4,
                Title = StringTable.HomePageTitle,
                Description = StringTable.HomePageDescription,
                FriendlyUrl = "quen-mat-khau",
                ControllerName = "Home",
                ActionName = "ForgetPassword",
                ObjectState = ObjectState.Added
            };
            context.PageItems.AddOrUpdate(page);

            page = new PageItem()
            {
                ID = 5,
                Title = StringTable.HomePageTitle,
                Description = StringTable.HomePageDescription,
                FriendlyUrl = "lay-lai-mat-khau",
                ControllerName = "Home",
                ActionName = "ResetPassword",
                ObjectState = ObjectState.Added
            };
            context.PageItems.AddOrUpdate(page);

            page = new PageItem()
            {
                ID = 6,
                Title = StringTable.ProviderAdminPageTitle,
                Description = StringTable.ProviderAdminPageDescription,
                FriendlyUrl = "bai-dau-xe/quan-ly",
                ControllerName = "Provider",
                ActionName = "Index",
                ObjectState = ObjectState.Added
            };
            context.PageItems.AddOrUpdate(page);

            page = new PageItem()
            {
                ID = 7,
                Title = StringTable.ProviderPrivacyStatementPageTitle,
                Description = StringTable.ProviderPrivacyStatementPageDescription,
                FriendlyUrl = "bai-dau-xe/dieu-khoan-ca-nhan",
                ControllerName = "Provider",
                ActionName = "PrivacyStatement",
                ObjectState = ObjectState.Added
            };
            context.PageItems.AddOrUpdate(page);

            page = new PageItem()
            {
                ID = 8,
                Title = StringTable.ProviderPrivacyStatementPageTitle,
                Description = StringTable.ProviderPrivacyStatementPageDescription,
                FriendlyUrl = "cau-hoi-thuong-gap",
                ControllerName = "Home",
                ActionName = "FAQ",
                ObjectState = ObjectState.Added
            };
            context.PageItems.AddOrUpdate(page);

            page = new PageItem()
            {
                ID = 9,
                Title = StringTable.ProviderPrivacyStatementPageTitle,
                Description = StringTable.ProviderPrivacyStatementPageDescription,
                FriendlyUrl = "gioi-thieu",
                ControllerName = "Home",
                ActionName = "AboutUs",
                ObjectState = ObjectState.Added
            };
            context.PageItems.AddOrUpdate(page);

            page = new PageItem()
            {
                ID = 10,
                Title = StringTable.ProviderAdminPageTitle,
                Description = StringTable.ProviderAdminPageDescription,
                FriendlyUrl = "kich-hoat-bai-dau-xe",
                ControllerName = "Home",
                ActionName = "ReviewProvider",
                ObjectState = ObjectState.Added
            };
            context.PageItems.AddOrUpdate(page);

            page = new PageItem()
            {
                ID = 11,
                Title = StringTable.ProviderAdminPageTitle,
                Description = StringTable.HomePageDescription,
                FriendlyUrl = "bai-dau-xe",
                ControllerName = "Provider",
                ActionName = "SlotProviders",
                ObjectState = ObjectState.Added
            };
            context.PageItems.AddOrUpdate(page);

            page = new PageItem()
            {
                ID = 12,
                Title = StringTable.ProviderAdminPageTitle,
                Description = StringTable.HomePageDescription,
                FriendlyUrl = "bai-dau-xe/thong-tin",
                ControllerName = "Provider",
                ActionName = "SlotProviderDetail",
                ObjectState = ObjectState.Added
            };
            context.PageItems.AddOrUpdate(page);

            context.SaveChanges();
        }

    }
}

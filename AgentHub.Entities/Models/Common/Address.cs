using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AgentHub.Entities.Models.Common
{
    public partial class Address: Entity
    {
        [MaxLength(20)]
        public string StreetNumber { get; set; }

        [Required]
        [MaxLength(80)]
        public string Street { get; set; }

        [MaxLength(10)]
        public string ZipCode { get; set; }

        [MaxLength(250)]
        [DefaultValue("")]
        public string AddressLine { get; set; }

        public int? DistrictId { get; set; }

        public virtual District District { get; set; }

        public int? CityId { get; set; }

        public virtual City City { get; set; }

        public int? StateId { get; set; }

        public virtual State State { get; set; }

        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        public override string ToString()
        {
            var addressInString = "";
            if (!string.IsNullOrEmpty(StreetNumber))
                addressInString = string.Format("{0} {1}", addressInString, StreetNumber); // Lo 38 - Tan An 4, Hoa Cuong Bac, Hai Chau, Da Nang, Vietnam
            if (!string.IsNullOrEmpty(Street))
                addressInString = string.Format("{0} - {1}", addressInString, Street);
            if (District != null)
                addressInString = string.Format("{0}, {1}", addressInString, District.Name);
            if (City != null)
                addressInString = string.Format("{0}, {1}", addressInString, City.Name);
            if (State != null)
                addressInString = string.Format("{0}, {1}", addressInString, State.Name);
            //if (Country != null)
            //    addressInString = string.Format("{0}, {1}", addressInString, Country.Name);

            addressInString = addressInString.Trim();
            if (addressInString.StartsWith("- "))
                addressInString = addressInString.Substring("- ".Length);

            return addressInString;
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using Newtonsoft.Json;

namespace AgentHub.Entities.Utilities
{
    public class WebLocation
    {
        public string As { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string ISP { get; set; }

        /// <summary>
        /// Gets or sets the latitude
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// Gets or sets the longitude
        /// </summary>
        public double Lon { get; set; }

        public string Org { get; set; }
        public string Query { get; set; }
        public string Region { get; set; }
        public string RegionName { get; set; }
        public string Status { get; set; }
        public string TimeZone { get; set; }
        public string Zip { get; set; }
    }

    public class Geolocation
    {
        public string Address { get; set; }
        public string StreetNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public Geolocation()
        {
            Address = string.Empty;
            StreetNumber = string.Empty;
            Street = string.Empty;
            City = string.Empty;
            State = string.Empty;
            Country = string.Empty;
        }

    }

    public static class LocationHelper
    {
        public static WebLocation GetCityByIP(string ip)
        {
            try
            {
                var url = AppSettings.IPAPIService;
                if (string.IsNullOrEmpty(url))
                    return null;

                url = string.Format("{0}/{1}", url, ip);
                var objRequest = (HttpWebRequest)WebRequest.Create(url);
                using (var objResponse = (HttpWebResponse)objRequest.GetResponse())
                {
                    using (var responseStream = new StreamReader(objResponse.GetResponseStream()))
                    {
                        var responseRead = responseStream.ReadToEnd();

                        var location = JsonConvert.DeserializeObject<WebLocation>(responseRead);

                        responseStream.Close();

                        return location;
                    }
                }
            }
            catch (Exception exception)
            {
                // TODO: Log the exception
            }

            return new WebLocation();
        }

        public static Geolocation GetLocation(double latitude, double longitude)
        {
            var location = new Geolocation();

            try
            {
                var doc = new XmlDocument();
                var geocodeServiceUrl = AppSettings.GoogleMapGeocodeAPIService;
                
                if (string.IsNullOrEmpty(geocodeServiceUrl))
                    return location;

                geocodeServiceUrl = string.Format(geocodeServiceUrl, latitude, longitude);
                doc.Load(geocodeServiceUrl);
                var element = doc.SelectSingleNode("//GeocodeResponse/status");
                if (element != null && element.InnerText != "OK")
                {
                    return location;
                }

                // Get Address
                var singleNode = doc.SelectSingleNode("//GeocodeResponse/result/formatted_address");
                if (singleNode != null)
                    location.Address = singleNode.InnerText;

                var resultListNodes = doc.SelectNodes("//GeocodeResponse/result");
                if (resultListNodes == null || resultListNodes.Count <= 0)
                    return location;

                var resultNode = resultListNodes.Item(0);
                if (resultNode == null || resultNode.ChildNodes.Count <= 0)
                    return location;

                var addressComponentNodes = resultNode.SelectNodes("address_component");
                if (addressComponentNodes == null || addressComponentNodes.Count <= 0)
                    return location;

                var supportedTypes = new []{"street_number", "route", "sublocality_level_1", "administrative_area_level_1", "country"};
                foreach (XmlNode addressComponent in addressComponentNodes)
                {
                    if (addressComponent != null)
                    {
                        var types = addressComponent.SelectNodes("type").Cast<XmlNode>();

                        var typeNode = types.FirstOrDefault(_ => supportedTypes.Contains(_.InnerText));
                        if (typeNode != null)
                        {
                            var valueNode = addressComponent.SelectNodes("long_name").Cast<XmlNode>().FirstOrDefault();
                            if (valueNode != null)
                            {
                                switch (typeNode.InnerText.ToLower())
                                {
                                    case "street_number":
                                        location.StreetNumber = valueNode.InnerText;
                                        break;
                                    case "route":
                                        location.Street = valueNode.InnerText;
                                        break;
                                    case "sublocality_level_1":
                                        location.City = valueNode.InnerText;
                                        break;
                                    case "administrative_area_level_1":
                                        location.State = valueNode.InnerText;
                                        break;
                                    case "country":
                                        location.Country = valueNode.InnerText;
                                        break;
                                }
                            }
                        }
                    }
                }
                //
                // Get geolocation (latitude & longitude)
                var geolocationNodes = resultNode.SelectNodes("address_component/geometry/location").Cast<XmlNode>();
                foreach (var geolocationNode in geolocationNodes)
                {
                    if (geolocationNode.Name == "lat")
                        location.Latitude = geolocationNode.InnerText.ToDouble();
                    else if (geolocationNode.Name == "lng")
                        location.Longitude = geolocationNode.InnerText.ToDouble();
                }

                return location;
            }
            catch (Exception exception)
            {
                // TODO: Log the exception
            }

            return location;
        }
    }
}

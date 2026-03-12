using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net;

namespace LocationTrackingAPI.Models
{
    public class DALDistanceMatrix
    {
        string _strCon = string.Empty;
        LogHelper _lLog = null;
        public DALDistanceMatrix()
        {
            _lLog = new LogHelper("");
            _strCon = ConfigurationManager.AppSettings["ConnStr"].ToString().Trim();
        }

        

        public async Task<DistanceResponse> CalculateMatrix(DistanceRequest model)
        {
            DistanceResponse _data = new DistanceResponse();
            DBHelper _lhelp = new DBHelper(_strCon);

            try
            {
                Matrix S= await GetLatLongFromPincode(model.FromPostalCode);
                Matrix E = await GetLatLongFromPincode(model.ToPostalCode);
                double dis = CalculateKm(Convert.ToDouble(S.Latitude), Convert.ToDouble(S.Longitude), Convert.ToDouble(E.Latitude), Convert.ToDouble(E.Longitude));

                _data.FromPostalCode = model.FromPostalCode;
                _data.ToPostalCode = model.ToPostalCode;
                _data.FromLatitude = S.Latitude;
                _data.FromLongitude = S.Longitude;
                _data.ToLatitude = E.Latitude;
                _data.ToLongitude = E.Longitude;
                _data.TotalDistance = dis;
            }
            catch (Exception ex)
            {
                _lLog.WriteFileToLocal(ex.ToString());
                return _data;   // return empty list on error
            }

            return _data;
        }


        public async Task<Matrix> GetLatLongFromPincode(string pincode)
        {
            Matrix _data = new Matrix();
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls;
            string url = $"https://nominatim.openstreetmap.org/search?format=json&country=India&postalcode={pincode}";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");

                string response = await client.GetStringAsync(url);

                JArray json = JArray.Parse(response);

                if (json.Count == 0)
                {
                    throw new Exception("Invalid pincode or no data found");
                }

                double lat = Convert.ToDouble(json[0]["lat"]);
                double lon = Convert.ToDouble(json[0]["lon"]);

                // Use values here
                //Console.WriteLine("Latitude: " + lat);
                //Console.WriteLine("Longitude: " + lon);

                _data.Latitude = lat.ToString();
                _data.Longitude = lon.ToString();
                

                

            }

            return _data;
        }
        public static double CalculateKm(
        double lat1, double lon1,
        double lat2, double lon2)
        {
            const double R = 6371; // Earth radius in KM

            double dLat = ToRad(lat2 - lat1);
            double dLon = ToRad(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRad(double val)
        {
            return val * Math.PI / 180;
        }
    }
}
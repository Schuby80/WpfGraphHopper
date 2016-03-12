using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WpfGraphHopper.Api
{
    class GraphHopper
    {
        //https://graphhopper.com/api/1/geocode?q=Brandenburger+Tor&locale=de&debug=true&key=
        //https://graphhopper.com/api/1/matrix?point=49.932707%2C11.588051&point=50.241935%2C10.747375&point=50.118817%2C11.983337&point=50.3404%2C11.64705&type=json&vehicle=car&debug=true&out_array=weights&out_array=times&out_array=distances&key=
        private string m_Key = ""; 
        private string m_lastResponse = "";

        private UInt32 m_timeoutSec = 10;

        public string LastResponse
        {
            get { return m_lastResponse; }
        }

        public UInt32 TimeoutSec
        {
            get { return m_timeoutSec; }
            set { m_timeoutSec = value; }
        }

        public GraphHopper(string key)
        {
            m_Key = key;
        }

        public HitsResponse Geolocate(string location)
        {
            HitsResponse hits;
            string url = "https://graphhopper.com/api/1/geocode?locale=de&debug=true&key=";
            url += m_Key;
            url += "&q=" + location.Replace(' ', '+');

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Proxy = null; // Performance hack!
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format(
                    "Server error (HTTP {0}: {1}).",
                    response.StatusCode,
                    response.StatusDescription));

                Stream stream = response.GetResponseStream();
                MemoryStream memStream = new MemoryStream();
                stream.CopyTo(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(HitsResponse));
                object objResponse = jsonSerializer.ReadObject(memStream);
                hits = objResponse as HitsResponse;
                CreateLastResponse(memStream);

                memStream.Close();
                stream.Close();
            }

            return hits;
        }

        public List<List<Tuple<double, double>>> GetDistanceTimeMatrix(List<Tuple<string, string>> latLngs)
        {
            List<List<Tuple<double, double>>> distTimes = new List<List<Tuple<double, double>>>();
            MatrixResponse matrix;
            string url = "https://graphhopper.com/api/1/matrix?type=json&vehicle=car&debug=true&out_array=times&out_array=distances&key=";
            url += m_Key;

            foreach (Tuple<string, string> latLng in latLngs)
            {
                url += "&point=" + latLng.Item1 + ","+ latLng.Item2;
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Proxy = null; // Performance hack!
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format(
                    "Server error (HTTP {0}: {1}).",
                    response.StatusCode,
                    response.StatusDescription));

                Stream stream = response.GetResponseStream();
                MemoryStream memStream = new MemoryStream();
                stream.CopyTo(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(MatrixResponse));
                object objResponse = jsonSerializer.ReadObject(memStream);
                matrix = objResponse as MatrixResponse;
                CreateLastResponse(memStream);

                memStream.Close();
                stream.Close();
            }

            // time and distance arrays have all the same length
            for (int i = 0; i < matrix.Distances.Count(); i++)
            {
                List<Tuple<double, double>> row = new List<Tuple<double, double>>();
                for (int k = 0; k < matrix.Distances[i].Count(); k++)
                {
                    double dist = matrix.Distances[i][k];
                    double time = matrix.Times[i][k];
                    Tuple<double, double> distTime = new Tuple<double, double>(dist,time);

                    row.Add(distTime);
                }

                distTimes.Add(row);
           }

            return distTimes;
        }

        public List<int> OptimizeRoute(List<Tuple<string, string>> latLngs, Tuple<string, string> start)
        {
            List<int> optimizedOrder = new List<int>();
            RouteOptimizationRequest routeRequest = new RouteOptimizationRequest();
            RouteOptimizationResponse routeResponse;
            string url = "https://graphhopper.com/api/1/vrp/optimize?key=";
            url += m_Key;

            RouteOptimizationVehicle vehicle = new RouteOptimizationVehicle();
            vehicle.StartAddress = new RouteOptimizationAddress();
            vehicle.StartAddress.LocationId = "start";
            vehicle.StartAddress.Lat = double.Parse(start.Item1.Replace('.', ','));
            vehicle.StartAddress.Lon = double.Parse(start.Item2.Replace('.', ','));
            vehicle.VehicleId = "car";

            routeRequest.Vehicles = new RouteOptimizationVehicle[] { vehicle };

            RouteOptimizationService[] services = new RouteOptimizationService[latLngs.Count];
            for (int i = 0; i < latLngs.Count; i++)
            {
                RouteOptimizationService service = new RouteOptimizationService();
                service.Id = i.ToString();
                service.Name = i.ToString();
                service.Address = new RouteOptimizationAddress();
                service.Address.LocationId = i.ToString();
                service.Address.Lat = double.Parse(latLngs[i].Item1.Replace('.', ','));
                service.Address.Lon = double.Parse(latLngs[i].Item2.Replace('.', ','));

                services[i] = service;
            }
            routeRequest.Services = services;


            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Proxy = null; // Performance hack!

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(RouteOptimizationRequest));
            MemoryStream outStream = new MemoryStream();
            jsonSerializer.WriteObject(outStream, routeRequest);
            outStream.Seek(0, SeekOrigin.Begin);
            outStream.CopyTo(request.GetRequestStream());

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format(
                    "Server error (HTTP {0}: {1}).",
                    response.StatusCode,
                    response.StatusDescription));

                Stream stream = response.GetResponseStream();
                MemoryStream memStream = new MemoryStream();
                stream.CopyTo(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                DataContractJsonSerializer jsonDeserializer = new DataContractJsonSerializer(typeof(RouteOptimizationResponse));
                object objResponse = jsonDeserializer.ReadObject(memStream);
                routeResponse = objResponse as RouteOptimizationResponse;
                CreateLastResponse(memStream);

                memStream.Close();
                stream.Close();
            }

            url = "https://graphhopper.com/api/1/vrp/solution/";
            url += routeResponse.JobId;
            url += "?key=" + m_Key;

            for (UInt32 i = 0; i < (m_timeoutSec * 2); i++)
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.Proxy = null; // Performance hack!
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));

                    Stream stream = response.GetResponseStream();
                    MemoryStream memStream = new MemoryStream();
                    stream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    DataContractJsonSerializer jsonDeserializer = new DataContractJsonSerializer(typeof(RouteOptimizationResponse));
                    object objResponse = jsonDeserializer.ReadObject(memStream);
                    routeResponse = objResponse as RouteOptimizationResponse;
                    CreateLastResponse(memStream);

                    memStream.Close();
                    stream.Close();
                }

                if ("finished" != routeResponse.Status)
                {
                    System.Threading.Thread.Sleep(500);
                }
                else
                {
                    if (routeResponse.Solution.Routes.Count() > 0)
                    {
                        foreach (RouteOptimizationActivity activity in routeResponse.Solution.Routes[0].Activities)
                        {
                            if ("start" == activity.LocationId)
                            {
                                continue;
                            }
                            else
                            {
                                int id = 0;
                                if (int.TryParse(activity.LocationId, out id))
                                {
                                    optimizedOrder.Add(id);
                                }
                            }
                        }
                    }

                    break;
                }
            }

            return optimizedOrder;
        }

        private void CreateLastResponse(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            StringBuilder sb = new StringBuilder();
            Byte[] buf = new byte[8192];
            int count = 0;
            do
            {
                count = stream.Read(buf, 0, buf.Length);
                if (count != 0)
                {
                    sb.Append(Encoding.UTF8.GetString(buf, 0, count)); // just hardcoding UTF8 here
                }
            } while (count > 0);

            m_lastResponse = sb.ToString();
        }
    }
}

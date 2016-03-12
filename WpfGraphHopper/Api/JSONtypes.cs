using System.Runtime.Serialization;

namespace WpfGraphHopper.Api
{
    [DataContract]
    public class InfoCopyrights
    {
        [DataMember(Name = "copyrights")]
        public string[] Copyrights { get; set; }
    }

    [DataContract]
    public class LngLatPoint
    {
        [DataMember(Name = "lng")]
        public string Lng { get; set; }
        [DataMember(Name = "lat")]
        public string Lat { get; set; }
    }

    [DataContract]
    public class Hit
    {
        [DataMember(Name = "osm_id")]
        public string OsmId { get; set; }
        [DataMember(Name = "extent")]
        public string[] Extent { get; set; }
        [DataMember(Name = "country")]
        public string Country { get; set; }
        [DataMember(Name = "city")]
        public string City { get; set; }
        [DataMember(Name = "postcode")]
        public string Postcode { get; set; }
        [DataMember(Name = "point")]
        public LngLatPoint Point { get; set; }
        [DataMember(Name = "osm_type")]
        public string OsmType { get; set; }
        [DataMember(Name = "osm_key")]
        public string OsmKey { get; set; }
        [DataMember(Name = "housenumber")]
        public string Housenumber { get; set; }
        [DataMember(Name = "street")]
        public string Street { get; set; }
        [DataMember(Name = "osm_value")]
        public string OsmValue { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "state")]
        public string State { get; set; }
    }

    [DataContract]
    public class HitsResponse
    {
        [DataMember(Name = "hits")]
        public Hit[] Hits { get; set; }
    }

    [DataContract]
    public class MatrixResponse
    {
        [DataMember(Name = "distances")]
        public double[][] Distances { get; set; }
        [DataMember(Name = "times")]
        public double[][] Times { get; set; }
        [DataMember(Name = "info")]
        public InfoCopyrights Info { get; set; }
    }


    [DataContract]
    public class RouteOptimizationAddress
    {
        [DataMember(Name = "location_id")]
        public string LocationId { get; set; }
        [DataMember(Name = "lon")]
        public double Lon { get; set; }
        [DataMember(Name = "lat")]
        public double Lat { get; set; }
    }

    [DataContract]
    public class RouteOptimizationService
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "address")]
        public RouteOptimizationAddress Address { get; set; }
    }

    [DataContract]
    public class RouteOptimizationVehicle
    {
        [DataMember(Name = "vehicle_id")]
        public string VehicleId { get; set; }
        [DataMember(Name = "start_address")]
        public RouteOptimizationAddress StartAddress { get; set; }
    }

    [DataContract]
    public class RouteOptimizationRequest
    {
        [DataMember(Name = "vehicles")]
        public RouteOptimizationVehicle[] Vehicles { get; set; }
        [DataMember(Name = "services")]
        public RouteOptimizationService[] Services { get; set; }
    }

    [DataContract]
    public class RouteOptimizationActivity
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "location_id")]
        public string LocationId { get; set; }
        [DataMember(Name = "end_time")]
        public string EndTime { get; set; }
        [DataMember(Name = "distance")]
        public string Distance { get; set; }
    }

    [DataContract]
    public class RouteOptimizationRoute
    {
        [DataMember(Name = "vehicle_id")]
        public string VehicleId { get; set; }
        [DataMember(Name = "activities")]
        public RouteOptimizationActivity[] Activities { get; set; }
    }

    [DataContract]
    public class RouteOptimizationSolution
    {
        [DataMember(Name = "costs")]
        public string Costs { get; set; }
        [DataMember(Name = "distance")]
        public string Distance { get; set; }
        [DataMember(Name = "time")]
        public string Time { get; set; }
        [DataMember(Name = "no_unassigned")]
        public string CntUnassigned { get; set; }
        [DataMember(Name = "routes")]
        public RouteOptimizationRoute[] Routes { get; set; }
    }

        [DataContract]
    public class RouteOptimizationResponse
    {
        [DataMember(Name = "job_id")]
        public string JobId { get; set; }
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "waiting_time_in_queue")]
        public string WaitingTimeInQueue { get; set; }
        [DataMember(Name = "processing_time")]
        public string ProcessingTime { get; set; }
        [DataMember(Name = "solution")]
        public RouteOptimizationSolution Solution { get; set; }
    }
}

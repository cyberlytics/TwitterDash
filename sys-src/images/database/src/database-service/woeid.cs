using System.Text.Json;

namespace places
{
    public class woeid
    {
        WOEID_places woeid_places;
        public woeid()
        {

            string fileName = "WOEID.json";
            string jsonString = File.ReadAllText(fileName);
            this.woeid_places = JsonSerializer.Deserialize<WOEID_places>(jsonString)!;
        }
        
        public int getWOEID(string coutry)
        {
            return this.woeid_places.places.Find(x => x.country == coutry).woeid;
        }

        public string getCountry(int woeid)
        {
            return this.woeid_places.places.Find(x => x.woeid == woeid).country;
        }
    }

  //   {
  //  "name": "Winnipeg",
  //  "placeType": {
  //    "code": 7,
  //    "name": "Town"
  //  },
  //  "url": "http://where.yahooapis.com/v1/place/2972",
  //  "parentid": 23424775,
  //  "country": "Canada",
  //  "woeid": 2972,
  //  "countryCode": "CA"
  //},
  
    public class WOEID_places
    {
        public List<place> places { get; set; }
    }
    public class place
    {
        public string name { get; set; }
        public placeType placeType { get; set; }
        public string url { get; set; }
        public int parentid { get; set; }
        public string country { get; set; }
        public int woeid { get; set; }
        public string countryCode { get; set; }

    }

    public class placeType
    {
        public int code { get; set; }
        public string name { get; set; }
    }
}
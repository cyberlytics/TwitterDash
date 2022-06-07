using System.Text.Json;

namespace places
{
    public class woeid
    {
        List<place> woeid_places;
        public woeid()
        {
            string fileName = "WOEID.json";
            string jsonString = File.ReadAllText(fileName);
            this.woeid_places = JsonSerializer.Deserialize<List<place>>(jsonString)!;
        }
        
        public int getWOEID(string country)
        {
            return this.woeid_places.Find(x => x.country == country).parentid;
        }

        public string getCountry(int woeid)
        {
            return this.woeid_places.Find(x => x.parentid == woeid).country;
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
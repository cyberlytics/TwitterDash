using places;

namespace places.test
{
    public class woeidInitTests
    {

        [Test]
        public void WoeidToCountry()
        {
            Assert.DoesNotThrow(() => { var WOEID = new woeid(); });
            
        }
    }
    
    public class woeidFunctionsTests
    {
        woeid Woeid;
        [SetUp]
        public void Setup()
        {
            this.Woeid = new woeid();
        }

        [Test]
        public void WoeidToCountry()
        {
            Assert.That("Germany" == Woeid.getCountry(23424829));
        }

        [Test]
        public void CountryToWoeid()
        {
            Assert.That(23424829 == Woeid.getWOEID("Germany"));
        }
    }
}
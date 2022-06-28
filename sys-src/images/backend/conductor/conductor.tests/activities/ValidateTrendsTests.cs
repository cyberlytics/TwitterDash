using conductor.activities;
using conductor.Nameservice;
using Elsa.ActivityResults;
using NUnit.Framework;
using System.Linq;
using Twitterdash;

namespace conductor.tests.activities
{
    public class ValidateTrendsTests : ActivityTestBase
    {
        [Test]
        public void ValidateTrends_Is_Constructable()
        {
            Assert.DoesNotThrow(() =>
            {
                var test = new ValidateTrends();
            });
        }

        [Test]
        public void ValidateTrends_Succeds_With_Valid_Trends()
        {

            context.WorkflowInstance.Variables.Set(VariableNames.Trends, new TrendProviderReply());

            var activity = new ValidateTrends();

            OutcomeResult result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                result = (OutcomeResult)await activity.ExecuteAsync(context);
            });

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.IsNotEmpty(result.Outcomes);
                Assert.AreEqual(Nameservice.Outcomes.Done , result.Outcomes.First());
            });
        }

        [Test]
        public void ValidateTrends_Fails_With_Invalid_Trends()
        {
            context.WorkflowInstance.Variables.Set(VariableNames.Trends, "notTrends");

            var activity = new ValidateTrends();

            OutcomeResult result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                result = (OutcomeResult)await activity.ExecuteAsync(context);
            });

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.IsNotEmpty(result.Outcomes);
                Assert.AreEqual(Nameservice.Outcomes.Failure, result.Outcomes.First());
            });
        }
    }
}

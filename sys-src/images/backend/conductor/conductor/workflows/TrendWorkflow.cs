using conductor.activities;
using Elsa.Activities.ControlFlow;
using Elsa.Builders;

namespace conductor.workflows
{
    public class TrendWorkflow : IWorkflow
    {
        public TrendWorkflow()
        {

        }

        public void Build(IWorkflowBuilder builder)
        {
            builder
                .WithDisplayName("Trend Workflow")
                .Then<ValidateTrends>()
                .WithDescription("Validate the Trends recieved from the Trend-Service")
                .WithId(nameof(ValidateTrends))
                .WithDisplayName("Validate Trends")
                .Then<Fork>(fork => fork.WithBranches("Persist", "Gather Tweets"), fork =>
                {
                    fork
                    .When("Persist")
                        .Then<PersistTrends>()
                        .WithDescription("Persist the trends to a Database")
                        .WithId(nameof(PersistTrends))
                        .WithDisplayName("Save Trends")
                    .ThenNamed("JoinTasks");

                    fork
                    .When("Gather Tweets")
                        .Then<CollectTweets>()
                        .WithDescription("Collect Tweets via Web-Scrapers")
                        .WithId(nameof(CollectTweets))
                        .WithDisplayName("Collect Tweets")
                    .Then<MakeTweetsUnique>()
                        .WithDescription("Compare the Tweet-IDs with the IDs already in the Database")
                        .WithId(nameof(MakeTweetsUnique))
                        .WithDisplayName("Filter Tweets")

                    .Then<Fork>(analysisfork => analysisfork.WithBranches("Sentiment", "TopicModeling"), analysisfork =>
                    {
                        analysisfork
                        .When("Sentiment")
                            .Then<PerformSentimentAnalysis>()
                            .Then<PersistSentiment>()
                            .ThenNamed("JoinAnalysisTasks");

                        analysisfork
                        .When("TopicModeling")
                            .Then<PerformTopicModeling>()
                            .Then<PersistTopicModels>()
                            .ThenNamed("JoinAnalysisTasks");

                    });
                    

                    fork.Add<Join>(x => x.WithMode(Join.JoinMode.WaitAll))
                   .WithDisplayName("Join Tasks")
                   .WithName("JoinAnalysisTasks")
                   .ThenNamed("JoinTasks");
                })
                .Add<Join>(x => x.WithMode(Join.JoinMode.WaitAll))
                .WithDisplayName("Join Tasks")
                .WithName("JoinTasks");
        }
    }
}

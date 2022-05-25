﻿using conductor.workflows;
using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using Twitterdash;

namespace conductor.activities
{
    public class PersistTrendsActivity : Activity
    {

        public PersistTrendsActivity()
        {
            this.DisplayName = "Persist Trends in Database";
        }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var trends = (TrendProviderReply)context.WorkflowInstance.Variables.Get("Trends")!;
            var success = true;
            try
            {
                await Task.Delay(5000);
            }catch (Exception ex)
            {
                success = false;
            }

            return Outcome(success ? "Success" : "Failed");
        }
    }
}

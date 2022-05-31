using conductor.activities;
using conductor.Nameservice;
using conductor.workflows;
using Elsa;
using Elsa.Builders;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Twitterdash;

namespace conductor.tests.workflows
{
    public class WorkflowTestBase : TestBase
    {
        protected IServiceProvider ServiceProvider;
        protected WebApplicationBuilder builder;
        public override void Setup()
        {
            base.Setup();
            builder = WebApplication.CreateBuilder();
        }

        protected IServiceProvider CreateServiceProvider()
        {
            builder.Services.AddElsa(elsa => elsa
             .AddActivity<ValidateTrends>()
                .AddActivity<PersistTrends>()
                .AddActivity<CollectTweets>()
                .AddWorkflow<TrendWorkflow>()
            );
            var app = builder.Build();
            ServiceProvider =  app.Services;
            return ServiceProvider;
        }

        protected async Task<(IWorkflowBlueprint, WorkflowInstance)> BuildWorkflow<TWorkflow>(IServiceScope scope) where TWorkflow : IWorkflow
        {
            var workflowRegistry = scope.ServiceProvider.GetRequiredService<IWorkflowRegistry>();
            var workflowFactory = scope.ServiceProvider.GetRequiredService<IWorkflowFactory>();

            var workflowBlueprint = (await workflowRegistry.GetWorkflowAsync<TWorkflow>())!;

            return (workflowBlueprint, await workflowFactory.InstantiateAsync(workflowBlueprint));
        }
    }
}

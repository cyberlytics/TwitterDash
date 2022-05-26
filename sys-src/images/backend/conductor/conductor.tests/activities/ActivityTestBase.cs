using Elsa.Models;
using Elsa.Services.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Threading;

namespace conductor.tests.activities
{
    [TestFixture]
    public class ActivityTestBase : TestBase
    {
        protected IServiceProvider ServiceProvider;
        public override void Setup()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddElsa();
            var app = builder.Build();
            ServiceProvider =  app.Services;
        }

        protected ActivityExecutionContext BuildExecutionContext()
        {
            //Build WorkflowExecutionContext
            var cancelToken = new CancellationToken(false);
            var workflowInstance = new WorkflowInstance();
            workflowInstance.CorrelationId = "UNITTEST_ID";

            var workflowBlueprint = new WorkflowBlueprint();
            var workflowExecutionContext = new WorkflowExecutionContext(ServiceProvider, workflowBlueprint, workflowInstance);

            // Build context
            return new ActivityExecutionContext(null, workflowExecutionContext, null, null, false, cancelToken);
        }

    }
}

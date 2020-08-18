using System;
using Alba;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MembershipSystem.Test.Setup
{
    public class ApplicationFixture : IDisposable
    {
        public SystemUnderTest SystemUnderTest { get; }

        public ApplicationFixture()
        {
            SystemUnderTest = SystemUnderTest.ForStartup<Startup>(Configure);
        }

        private static IWebHostBuilder Configure(IWebHostBuilder builder)
        {
            return builder
                 .UseEnvironment(Environments.Development)
                 .ConfigureAppConfiguration(configuration =>
                 {
                     configuration.AddEnvironmentVariables();
                 });
        }

        public void Dispose()
        {
            SystemUnderTest?.Dispose();
        }
    }
}

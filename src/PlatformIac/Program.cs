using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;
using PlatformIac.Stacks.Common;


namespace PlatformIac
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            // Sonar Qube Stack in dev environment 
            PFEnvironment sonarQubeEnvironment = new PFEnvironment("common", Constants.AWS_ACCOUNT_ID, Constants.AWS_ACCOUNT_REGION);
            PFStackManager sonarQubeStack =
                    new PFStackManager(app,
                        new PFStackProps(sonarQubeEnvironment, "SonarQube"));
            sonarQubeStack.CreateSonarQubeStack();

            // Dev Environment Stack
            PFEnvironment devEnvEnvironment = new PFEnvironment("dev", Constants.AWS_ACCOUNT_ID, Constants.AWS_ACCOUNT_REGION);
            PFStackManager devEnvStack =
                    new PFStackManager(app,
                        new PFStackProps(devEnvEnvironment, "DevEnv"));
            devEnvStack.CreateDevEnvStack();

            // Test Environment Stack
            PFEnvironment testEnvEnvironment = new PFEnvironment("test", Constants.AWS_ACCOUNT_ID, Constants.AWS_ACCOUNT_REGION);
            PFStackManager testEnvStack =
                    new PFStackManager(app,
                        new PFStackProps(testEnvEnvironment, "TestEnv"));
            testEnvStack.CreateTestEnvStack();

            // QA Environment Stack
            PFEnvironment qaEnvEnvironment = new PFEnvironment("QA", Constants.AWS_ACCOUNT_ID, Constants.AWS_ACCOUNT_REGION);
            PFStackManager qaEnvStack =
                    new PFStackManager(app,
                        new PFStackProps(qaEnvEnvironment, "QAEnv"));
            qaEnvStack.CreateQAEnvStack();

            // Hosted Zones Stack
            PFEnvironment hostedZonesEnvEnvironment = new PFEnvironment("common", Constants.AWS_ACCOUNT_ID, Constants.AWS_ACCOUNT_REGION);
            PFStackManager hostedZonesStack =
                    new PFStackManager(app,
                        new PFStackProps(hostedZonesEnvEnvironment, "HostedZones"));
            hostedZonesStack.CreateHostedZonesStack();

            // JMeter Stack
            PFEnvironment jMeterEnvironment = new PFEnvironment("common", Constants.AWS_ACCOUNT_ID, Constants.AWS_ACCOUNT_REGION);
            PFStackManager jMeterStack =
                    new PFStackManager(app,
                        new PFStackProps(jMeterEnvironment, "JMeter"));
            jMeterStack.CreateJMeterStack();
            app.Synth();
        }
    }
}

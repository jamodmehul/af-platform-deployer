
using System;
using Amazon.CDK;
using PlatformIac.Stacks;
using PlatformIac.Stacks.Common;

namespace PlatformIac
{
	public class PFStackManager
	{
		private readonly PFStackProps _baseStackProps;
		private readonly App _app;

		public PFStackManager(App app, PFStackProps props)
		{
			_app = app;
			_baseStackProps = props;
		}

		public void CreateSonarQubeStack()
		{
			
			new SonarQubeStack(_app, _baseStackProps);
		}

        public void CreateDevEnvStack()
        {
            new DevEnvStack(_app, _baseStackProps);
        }


        public void CreateTestEnvStack()
		{
			new TestEnvStack(_app, _baseStackProps);
		}

        public void CreateQAEnvStack()
        {
            new QAEnvStack(_app, _baseStackProps);
        }

        public void CreateHostedZonesStack()
        {
            new HostedZonesStack(_app, _baseStackProps);
        }

        public void CreateJMeterStack()
        {
            new JMeterStack(_app, _baseStackProps);
        }
    }
}


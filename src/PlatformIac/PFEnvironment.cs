using System;
using Amazon.CDK;

namespace PlatformIac
{
	public sealed class PFEnvironment
	{
		public string Name { get; }
        public string AccountId { get; }
        public string Region { get; }

        public PFEnvironment(string name, string accountId, string region)
		{
			Name = name;
			AccountId = accountId;
			Region = region;
		}
	}
}


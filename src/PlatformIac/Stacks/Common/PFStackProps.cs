using System;
using Amazon.CDK;


namespace PlatformIac.Stacks.Common
{
	public class PFStackProps : StackProps
	{
		internal PFEnvironment Environment { get; }

		public PFStackProps(PFEnvironment pFEnvironment, string stackName) : this(null, pFEnvironment, $"{Constants.APPLICATION_SUFFIX}-{pFEnvironment.Name}-{stackName}")
		{
			Env = new Amazon.CDK.Environment
			{
				Account = pFEnvironment.AccountId,
				Region = pFEnvironment.Region
			};
		}

		internal PFStackProps(PFStackProps source) : this(source.Env, source.Environment, source.StackName)
		{ }

		public PFStackProps(IEnvironment cdkEnvironment, PFEnvironment pFEnvironment, string stackName)
		{
			Env = cdkEnvironment;
			Environment = pFEnvironment;
			StackName = stackName;
		}
    }
}


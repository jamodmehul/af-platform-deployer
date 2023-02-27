using System;
using Amazon.CDK;
using Constructs;

namespace PlatformIac.Stacks.Common
{
	public abstract class PFStackBase : Stack
	{
		public PFEnvironment pFEnvironment { get; }
		internal string EnvironmentName => pFEnvironment.Name;

		internal string Platform { get; }
		internal string ResourcePrefix => Constants.APPLICATION_SUFFIX + "-" + EnvironmentName;

        protected PFStackBase(Construct scope, string stackName, PFStackProps stackProps )
			: base (scope, stackName, InitialiseProps(stackProps, stackName))
		{
			pFEnvironment = stackProps.Environment;
			Platform = $"{Constants.APPLICATION_SUFFIX}-{EnvironmentName}";

			//Apply default tag
            Amazon.CDK.Tags.Of(this).Add(Constants.GLOBAL_TAG_NAME_ENVIRONMENT, stackProps.Environment.Name);
        }

		public static StackProps InitialiseProps(PFStackProps sp, string stackName)
		{
			sp.StackName = stackName;
			sp.Description = $"Resources for  {stackName} stack";
			return sp;
		}
	}
}


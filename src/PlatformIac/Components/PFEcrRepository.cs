using System;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Constructs;
using PlatformIac.Stacks.Common;

namespace PlatformIac.Components
{
	public static class PFEcrRepository
	{
        // Function to create ECR Repository
        public static Repository CreateEcrRepository(this PFStackBase stack, string ecrRepositoryId, string ecrRepositoryName)
        {
            return new Repository(stack, ecrRepositoryId, new RepositoryProps
            {
                RepositoryName = ecrRepositoryName
            });
        }
    }
}


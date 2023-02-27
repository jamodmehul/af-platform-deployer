using System;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Logs;
using PlatformIac.Stacks.Common;

namespace PlatformIac.Components
{
	public static class PFVpc
    {
		public static Vpc CreateVpc(this PFStackBase stack, string vpcCidr)
		{
            var vpcId = $"{stack.Platform}-VPC";
            var resourcePrefix = $"{stack.Platform}";

            LogGroup flowLogGroup = new LogGroup(stack, "vpcFlowLogCustomLogGroup");

            Role flowLogRole = new Role(stack, "flowLogRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("vpc-flow-logs.amazonaws.com")
            });

            Vpc vpc = new Vpc(stack, vpcId, new VpcProps
            {
                IpAddresses = IpAddresses.Cidr(vpcCidr),
                MaxAzs = 2,
                SubnetConfiguration = new ISubnetConfiguration[]
                    {
                        new SubnetConfiguration
                        {
                            CidrMask = 24,
                            SubnetType = SubnetType.PUBLIC,
                            Name = resourcePrefix + "-public-sn"
                        },
                        new SubnetConfiguration
                        {
                            CidrMask = 24,
                            SubnetType = SubnetType.PRIVATE_ISOLATED,
                            Name = resourcePrefix + "-private-sn",
                        }
                    },
                EnableDnsSupport = true,
                EnableDnsHostnames = true,

            });

            new FlowLog(stack, "FlowLog", new FlowLogProps
            {
                ResourceType = FlowLogResourceType.FromVpc(vpc),
                Destination = FlowLogDestination.ToCloudWatchLogs(flowLogGroup, flowLogRole),
            });

            return vpc;
        }
		
	}
}


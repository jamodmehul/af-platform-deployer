using System;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Logs;
using Constructs;
using PlatformIac.Stacks.Common;

namespace PlatformIac.Components
{
	public static class PFSecurityGroup 
	{
        // Function to create Security group
        public static SecurityGroup CreateSecurityGroup(this PFStackBase stack, Vpc vpc, string groupId, string groupName, string groupDesc)
        {
            return new SecurityGroup(stack, groupId, new SecurityGroupProps
            {
                SecurityGroupName = groupName,
                Description = groupDesc,
                Vpc = vpc,
                AllowAllOutbound = true
            });
        }
    }
}


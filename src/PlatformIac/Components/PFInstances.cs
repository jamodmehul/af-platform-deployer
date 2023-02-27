using System;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Logs;
using Constructs;
using PlatformIac.Stacks.Common;

namespace PlatformIac.Components
{
	public static class PFInstances
	{
        public static Instance_ CreateInstance(this PFStackBase stack, string instanceId, string instanceName, string instanceType, string instanceKeyName, SecurityGroup jwtSonarcubeWebSG, UserData userdata, Vpc vpc, IMachineImage mi)
        {
            var sonarCubeInstance = new Amazon.CDK.AWS.EC2.Instance_(stack, instanceId, new InstanceProps
            {
                Vpc = vpc,
                InstanceName = instanceName,
                InstanceType = new InstanceType(instanceType),
                MachineImage = mi,
                SecurityGroup = jwtSonarcubeWebSG,
                KeyName = instanceKeyName,
                UserData = userdata,
                VpcSubnets = new SubnetSelection
                {
                    SubnetType = SubnetType.PUBLIC
                }
            });

            return sonarCubeInstance;
        }

        public static CfnEIP CreateEIP(this PFStackBase stack, string eipId, string eipName)
        {
            var eip = new CfnEIP(stack, eipId, new CfnEIPProps
            {
                Domain = "vpc",
                Tags = new[]
                {
                    new CfnTag
                    {
                        Key = "Name",
                        Value = eipName
                    }
                }
            });

            
            return eip;
        }
    }
}


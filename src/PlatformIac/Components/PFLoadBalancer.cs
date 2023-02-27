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
	public static class PFLoadBalancer
	{
        // Function to create Load balncer
        public static ApplicationLoadBalancer CreateALB(this PFStackBase stack, string ablId, string albName, Vpc vpc, SecurityGroup sg)
        {
            return new ApplicationLoadBalancer(stack, ablId, new ApplicationLoadBalancerProps
            {
                LoadBalancerName = albName,
                Vpc = vpc,
                InternetFacing = true,
                SecurityGroup = sg,
                VpcSubnets = new SubnetSelection
                {
                    SubnetType = SubnetType.PUBLIC
                }
            });
        }

        public static ApplicationListener AddListnerToAlb(this PFStackBase stack, ApplicationLoadBalancer ecsALB,
            string listnerId, double port, ListenerAction defaultAction, IListenerCertificate[] listenerCertificate, IApplicationTargetGroup[] defaultTargetGroups)
        {
            ApplicationListener listener = ecsALB.AddListener(listnerId, new BaseApplicationListenerProps
            {
                Port = port,
                DefaultAction = defaultAction,
                Certificates = listenerCertificate,
                DefaultTargetGroups = defaultTargetGroups
            });

            return listener;
        }
    }
}


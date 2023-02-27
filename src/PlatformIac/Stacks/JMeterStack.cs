using System;
using Amazon.CDK;
using Constructs;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using PlatformIac.Components;
using System.Collections.Generic;
using Amazon.CDK.AWS.ElasticLoadBalancingV2.Targets;
using PlatformIac.Stacks.Common;

namespace PlatformIac.Stacks
{
    public class JMeterStack : PFStackBase
    {
        internal JMeterStack(Construct scope, PFStackProps stackProps)
            : base(scope, stackProps.StackName, stackProps)
        {
        }
    }
}


using System;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.IAM;
using Constructs;
using PlatformIac.Components;
using PlatformIac.Stacks.Common;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK;
using Amazon.CDK.AWS.CloudWatch;
using Amazon.CDK.AWS.CloudWatch.Actions;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.Lightsail;
using System.ComponentModel;
using Amazon.CDK.AWS.CloudFront;

namespace PlatformIac.Stacks
{
	public class DevEnvStack : PFStackBase
    {
        //const string ecsLBCertArn = "arn:aws:acm:eu-west-2:316656599330:certificate/c71144f2-cac0-4468-98bb-514c67932cdc";
        //const string beLBCertArn = "arn:aws:acm:eu-west-2:316656599330:certificate/b732fe8f-ec50-4420-9d85-5001ff42bbf4";

        public DevEnvStack(Construct scope, PFStackProps stackProps)
            : base(scope, stackProps.StackName, stackProps)
        {
            Amazon.CDK.Tags.Of(this).Add(Constants.GLOBAL_TAG_NAME_ENVIRONMENT, stackProps.Environment.Name);
            string resourcePrefix = Constants.APPLICATION_SUFFIX + "-" + stackProps.Environment.Name;

            Vpc vpc = this.CreateVpc(Constants.DEVENV_VPC_CIDR);
        }
	}
}


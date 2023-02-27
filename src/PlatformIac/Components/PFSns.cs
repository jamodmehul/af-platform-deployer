using System;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECR;
using Constructs;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Logs;
using System.ComponentModel;
using Amazon.CDK.AWS.CloudWatch;
using Amazon.CDK.AWS.CloudWatch.Actions;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SNS.Subscriptions;
using System.Collections.Generic;
using System.Security.Principal;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using PlatformIac.Stacks.Common;


namespace PlatformIac.Components
{
	public static class PFSns
	{
        // Function to create SNS Topic
        public static Topic CreateSnsTopic(this PFStackBase stack, string topicId, string topicName, string emailSubscriber)
        {
            // SNS topic creatation
            Topic topic = new Topic(stack,
                                    topicId,
                                    new TopicProps
                                    {
                                        TopicName = topicName
                                    });
            topic.AddSubscription(new EmailSubscription(emailSubscriber));
            topic.GrantPublish(new PrincipalWithConditions(new AnyPrincipal(),
            new Dictionary<string, object>
            {
                { "StringEquals", new Dictionary<string, string> {{ "AWS:SourceOwner", Constants.AWS_ACCOUNT_ID } } }
            }));

            return topic;
        }
    }
}


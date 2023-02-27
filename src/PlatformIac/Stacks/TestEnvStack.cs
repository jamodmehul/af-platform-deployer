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
	public class TestEnvStack : PFStackBase
    {
        //const string ecsLBCertArn = "arn:aws:acm:eu-west-2:316656599330:certificate/c71144f2-cac0-4468-98bb-514c67932cdc";
        const string ecsLBCertArn = "arn:aws:acm:eu-west-2:316656599330:certificate/453be658-eec5-4028-8a7b-56677330c11c";
        const string ercRepo = "316656599330.dkr.ecr.eu-west-2.amazonaws.com/jwt-test-fe-repo:latest";

        public TestEnvStack(Construct scope, PFStackProps stackProps)
            : base(scope, stackProps.StackName, stackProps)
        {
            //Amazon.CDK.Tags.Of(this).Add(Constants.GLOBAL_TAG_NAME_ENVIRONMENT, stackProps.Environment.Name);

            string resourcePrefix = Constants.APPLICATION_SUFFIX + "-" + stackProps.Environment.Name;

            // Creating VPC
            Vpc vpc = this.CreateVpc(Constants.TESTENV_VPC_CIDR);

            // Creating ECR Repository
            string feEcrRepoId = (resourcePrefix + "-fe-repo").ToLower();
            string feEcrRepoName = feEcrRepoId;
            Repository ecrRepository = this.CreateEcrRepository(feEcrRepoId, feEcrRepoName);

            // Creating ECS Cluster
            string ecsClusterId = (resourcePrefix + "-fe-cluster").ToLower(); ;
            string ecsClusterName = ecsClusterId;
            Cluster ecsCluster = this.CreateEcsCluster(ecsClusterId, ecsClusterName, vpc);

            var ecsExecutionRoleId = resourcePrefix + "-ecsExecutionRole" ;
            var ecsExecutionRoleName = resourcePrefix + "-ecs-execution-role" ;
            Role ecsExecutionRole = this.CreateEcsExecutionRole(ecsExecutionRoleId, ecsExecutionRoleName);

            var ecsTaskRoleId = resourcePrefix + "-ecstaskroleId" ;
            var ecsTaskRoleName = resourcePrefix + "-ecs-task-role" ;
            Role ecsTaskRole = this.CreateEcsTaskRole(ecsTaskRoleId, ecsTaskRoleName);

            string taskDefinitionId = resourcePrefix + "-ecsFETaskDef";
            var logGroupName = resourcePrefix + "-jwtFELogGroup";
            var containerId = resourcePrefix + "-FEContainer";
            TaskDefinition ecsTaskDefinition = this.CreateEcsTaskDefinition(taskDefinitionId, ecsExecutionRole, ecsTaskRole, "512", "256", ercRepo, logGroupName, containerId);

            // Create security group for load balancer
            string lbsgId = resourcePrefix + "-ecsLBSG";
            string ldsgName = resourcePrefix + "-fe-ecs-loadbalancer-sg";
            string ldsgDesc = resourcePrefix + "Security group for ecs load balancer";
            SecurityGroup loadBalanberSG = this.CreateSecurityGroup(vpc, lbsgId, ldsgName, ldsgDesc);
            loadBalanberSG.AddIngressRule(Peer.Ipv4("0.0.0.0/0"), Port.Tcp(80), "all traffic");
            loadBalanberSG.AddIngressRule(Peer.Ipv4("0.0.0.0/0"), Port.Tcp(443), "all traffic");

            // Create Security group for container
            string consgId = resourcePrefix + "-containerSG";
            string consgName = resourcePrefix + "-fe-ecs-container-sg";
            string consgDesc = resourcePrefix + "Security group for ecs containers";
            SecurityGroup containerSG = this.CreateSecurityGroup(vpc, consgId, consgName, consgDesc);
            containerSG.AddIngressRule(Peer.SecurityGroupId(loadBalanberSG.SecurityGroupId), Port.TcpRange(80, 80), "all traffic");

            // Creating ECS Service
            string fargetServiceId = resourcePrefix + "-Service";
            string fargetServiceName = resourcePrefix + "-frontend-service";
            int desiredTaskCount = 2;
            FargateService feECSService = this.CreateEcsFargateService(resourcePrefix, fargetServiceId, ecsCluster, ecsTaskDefinition, fargetServiceName, containerSG, desiredTaskCount);

            // SNS topic creation
            string topicId = "DevOpsNotificationTopic";
            string topicName = $"{resourcePrefix}-{topicId}";
            Topic topic = this.CreateSnsTopic(topicId, topicName, "mehul.jamod@analogfolk.com");

            // Create Alarams for ECS
            Alarm cupAlarm = this.CreateAlarm($"{fargetServiceId}-CpuUtilization-Alarm",
                                                $"{resourcePrefix}-{fargetServiceName}-CpuUtilization",
                                                feECSService.MetricCpuUtilization(new MetricOptions
                                                {
                                                    Statistic = "max"
                                                }),
                                                80,
                                                1,
                                                ComparisonOperator.GREATER_THAN_OR_EQUAL_TO_THRESHOLD,
                                                TreatMissingData.IGNORE);

            Alarm memoryAlarm = this.CreateAlarm($"{fargetServiceId}-MemoryUtilization-Alarm",
                                                $"{resourcePrefix}-{fargetServiceName}-MemoryUtilization",
                                                feECSService.MetricMemoryUtilization(new MetricOptions
                                                {
                                                    Statistic = "max"
                                                }),
                                                70,
                                                1,
                                                ComparisonOperator.GREATER_THAN_OR_EQUAL_TO_THRESHOLD,
                                                TreatMissingData.IGNORE);

            Alarm runningTaskCountAlarm = this.CreateAlarm($"{fargetServiceId}-RunningTaskCount-Alarm",
                                                $"{resourcePrefix}-{fargetServiceName}-RunningTaskCount",
                                                feECSService.MetricMemoryUtilization(new MetricOptions
                                                {
                                                    Period = Duration.Seconds(60),
                                                    Statistic = "SampleCount"
                                                }),
                                                2,
                                                1,
                                                ComparisonOperator.LESS_THAN_THRESHOLD,
                                                TreatMissingData.BREACHING);

            // add ecs alarms to sns action
            var snsAction = new SnsAction(topic);
            cupAlarm.AddAlarmAction(snsAction);
            memoryAlarm.AddAlarmAction(snsAction);
            runningTaskCountAlarm.AddAlarmAction(snsAction);

            // Create Load Balancer for ECS cluster
            string ecsAlbId = resourcePrefix + "-fe-ecs-lb";
            string ecsAlbName = resourcePrefix + "-frontend-ecs-lb";
            ApplicationLoadBalancer ecsALB = this.CreateALB(ecsAlbId, ecsAlbName, vpc, loadBalanberSG);

            // Create Alarams for ApplicationLoadBalancer
            Alarm Alb5xxErrorCountAlarm = this.CreateAlarm(
                                                        $"{ecsAlbId}-Alb-5xxErrorCount",
                                                        $"{ecsAlbName}-Alb-5xxErrorCount",
                                                        ecsALB.Metrics.HttpCodeElb(HttpCodeElb.ELB_5XX_COUNT,
                                                            new MetricOptions
                                                            {
                                                                Statistic = "sum",
                                                                Period = Duration.Seconds(60)
                                                            }),
                                                        0,
                                                        1,
                                                        ComparisonOperator.GREATER_THAN_THRESHOLD,
                                                        TreatMissingData.NOT_BREACHING );

            Alarm AlbRejectedConnectionCountAlarm = this.CreateAlarm(
                                                        $"{ecsAlbId}-Alb-RejectedConnectionCount",
                                                        $"{ecsAlbName}-Alb-RejectedConnectionCount",
                                                        ecsALB.Metrics.RejectedConnectionCount(new MetricOptions
                                                        {
                                                            Statistic = "sum",
                                                            Period = Duration.Seconds(60)
                                                        }),
                                                        0,
                                                        1,
                                                        ComparisonOperator.GREATER_THAN_THRESHOLD,
                                                        TreatMissingData.NOT_BREACHING);

            // add Alb alarms to sns action
            Alb5xxErrorCountAlarm.AddAlarmAction(snsAction);
            AlbRejectedConnectionCountAlarm.AddAlarmAction(snsAction);

            // Adding listeners to ecs load balancer
            string feLbHttpListnerId = resourcePrefix + "-listner80";
            ApplicationListener listener80 = this.AddListnerToAlb(ecsALB,
                                feLbHttpListnerId,
                                80,
                                ListenerAction.Redirect(new RedirectOptions
                                {
                                    Host = "#{host}",
                                    Path = "/#{path}",
                                    Permanent = true,
                                    Protocol = "HTTPS",
                                    Port = "443",
                                    Query = "#{query}"
                                }),
                                null,
                                null);

            string feLbHttpsListnerId = resourcePrefix + "-listner443";
            ApplicationListener listener443 = this.AddListnerToAlb(ecsALB,
                                feLbHttpsListnerId,
                                443,
                                null,
                                new IListenerCertificate[] { ListenerCertificate.FromArn(certificateArn: ecsLBCertArn) },
                                null);

            // Assign service to load balancer target group
            string containerTargetGroupName = resourcePrefix + "FeTg";
            feECSService.RegisterLoadBalancerTargets(new EcsTarget
            {
                ContainerPort = 3000,
                ContainerName = ecsTaskDefinition.DefaultContainer.ContainerName,
                NewTargetGroupId = containerTargetGroupName,
                Listener = ListenerConfig.ApplicationListener(listener443, new AddApplicationTargetsProps
                {
                    Protocol = ApplicationProtocol.HTTP,
                    HealthCheck = new Amazon.CDK.AWS.ElasticLoadBalancingV2.HealthCheck
                    {
                        Path = "/",
                        HealthyThresholdCount = 2,
                        UnhealthyThresholdCount = 5,
                        Timeout = Duration.Seconds(60),
                        Interval = Duration.Seconds(70),
                        HealthyHttpCodes = "200-400"
                    },
                })
            });


            // Create CloudFront Web Distribution group
            CloudFrontWebDistribution cdn = this.CreateCloudFrontWebDistribution("jwtTestCdn", ecsALB.LoadBalancerDnsName);
        }
    }
}


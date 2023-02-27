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
using PlatformIac.Stacks.Common;

namespace PlatformIac.Components
{
    /// <summary>
    /// This class provides extention methods for PFStackBase class
    /// </summary>
    public static class PFEcs
    {
        // Function to create ECS Cluster
        /// <summary>
        /// Method to create ECS Cluster
        /// </summary>
        /// <param name="stack">Cloudformation stack</param>
        /// <param name="ecrClusterId">Cluster Id</param>
        /// <param name="ecrClusterName">Cluster Name</param>
        /// <param name="vpc">VPC to create cluster in</param>
        /// <returns></returns>
        public static Cluster CreateEcsCluster(this PFStackBase stack, string ecrClusterId, string ecrClusterName, Vpc vpc)
        {
            return new Cluster(stack, ecrClusterId, new ClusterProps
            {
                Vpc = vpc,
                ClusterName = ecrClusterName
            });
        }

        /// <summary>
        /// Method to create ECS execution role within ECS Cluster 
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="ecsExecutionRoleId"></param>
        /// <param name="ecsExecutionRoleName"></param>
        /// <returns></returns>
        public static Role CreateEcsExecutionRole(this PFStackBase stack, string ecsExecutionRoleId, string ecsExecutionRoleName)
        {
            // Create Execution role for the task definition
            var ecsExecutionRole = new Role(stack, ecsExecutionRoleId, new RoleProps
            {
                RoleName = ecsExecutionRoleName,
                ManagedPolicies = new[]
                {
                    ManagedPolicy.FromAwsManagedPolicyName("service-role/AmazonECSTaskExecutionRolePolicy")
                },
                AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com")
            });
            ecsExecutionRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new[] { "sts:AssumeRole" },
                Resources = new[] { "*" }
            }));

            return ecsExecutionRole;
        }

        public static Role CreateEcsTaskRole(this PFStackBase stack, string ecsTaskRoleId, string ecsTaskRoleName)
        {
            // Create task role for the task definition
            var ecsTaskRole = new Role(stack, ecsTaskRoleId, new RoleProps
            {
                RoleName = ecsTaskRoleName, 
                AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com")
            }); ;
            ecsTaskRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new[] { "sts:AssumeRole" },
                Resources = new[] { "*" }
            }));

            return ecsTaskRole;
        }

        public static TaskDefinition CreateEcsTaskDefinition(this PFStackBase stack, string taskDefinitionId, Role ecsExecutionRole,
            Role ecsTaskRole, string memoryAmount, string cpuAmount, string ercRepo, string logGroupName, string containerId)
        {
            // Create task definitation
            //string taskDefinitionId = resourcePrefix + "-ecsFETaskDef";
            TaskDefinition taskDefinition = new TaskDefinition(stack, "ecsFETaskDef", new TaskDefinitionProps
            {
                MemoryMiB = memoryAmount,
                Cpu = cpuAmount,
                NetworkMode = NetworkMode.AWS_VPC,
                Compatibility = Compatibility.FARGATE,
                ExecutionRole = ecsExecutionRole,
                TaskRole = ecsTaskRole

            });

            // add logs for ECS
            var logGroup = new LogGroup(
                                        stack,
                                        "ecsLogGroup",
                                        new LogGroupProps
                                        {
                                            LogGroupName = logGroupName,
                                            Retention = RetentionDays.TWO_WEEKS,
                                            RemovalPolicy = RemovalPolicy.DESTROY
                                        });
            var logConfiguration = new AwsLogDriver(new AwsLogDriverProps { LogGroup = logGroup, StreamPrefix = "jwtFELogs" });

            // Create container definition for the task definition
            ContainerDefinition container = taskDefinition.AddContainer(containerId, new ContainerDefinitionOptions
            {
                Image = ContainerImage.FromRegistry(ercRepo),
                Logging = logConfiguration,
                PortMappings = new[]
                {
                    new PortMapping
                    {
                        ContainerPort = 3000
                    },
                    new PortMapping
                    {
                        ContainerPort = 80
                    }
                }
            });

            return taskDefinition;
        }

        public static FargateService CreateEcsFargateService(this PFStackBase stack, string resourcePrefix, string fargetServiceId, Cluster jwtFrontendCluster, TaskDefinition taskDefinition, string fargetServiceName, SecurityGroup containerSG, int desiredTaskCount)
        {
            // Create ECS Fargate Service
            FargateService feECSService = new FargateService(stack, fargetServiceId, new FargateServiceProps
            {
                Cluster = jwtFrontendCluster,
                TaskDefinition = taskDefinition,
                DesiredCount = desiredTaskCount,
                ServiceName = fargetServiceName,
                //HealthCheckGracePeriod = Duration.Seconds(30),
                VpcSubnets = new SubnetSelection
                {
                    SubnetType = SubnetType.PUBLIC
                },
                AssignPublicIp = true,
                SecurityGroups = new[] { containerSG },
            });

            //// Create Alarams for ECS
            //Alarm cupAlarm = stack.CreateAlarm($"{fargetServiceId}-CpuUtilization-Alarm",
            //                                    $"{resourcePrefix}-{fargetServiceName}-CpuUtilization",
            //                                    feECSService.MetricCpuUtilization(new MetricOptions
            //                                    {
            //                                        Statistic = "max"
            //                                    }),
            //                                    80,
            //                                    1,
            //                                    ComparisonOperator.GREATER_THAN_OR_EQUAL_TO_THRESHOLD,
            //                                    TreatMissingData.IGNORE);

            //Alarm memoryAlarm = stack.CreateAlarm($"{fargetServiceId}-MemoryUtilization-Alarm",
            //                                    $"{resourcePrefix}-{fargetServiceName}-MemoryUtilization",
            //                                    feECSService.MetricMemoryUtilization(new MetricOptions
            //                                    {
            //                                        Statistic = "max"
            //                                    }),
            //                                    70,
            //                                    1,
            //                                    ComparisonOperator.GREATER_THAN_OR_EQUAL_TO_THRESHOLD,
            //                                    TreatMissingData.IGNORE);

            //Alarm runningTaskCountAlarm = stack.CreateAlarm($"{fargetServiceId}-RunningTaskCount-Alarm",
            //                                    $"{resourcePrefix}-{fargetServiceName}-RunningTaskCount",
            //                                    feECSService.MetricMemoryUtilization(new MetricOptions
            //                                    {
            //                                        Period = Duration.Seconds(60),
            //                                        Statistic = "SampleCount"
            //                                    }),
            //                                    2,
            //                                    1,
            //                                    ComparisonOperator.LESS_THAN_THRESHOLD,
            //                                    TreatMissingData.BREACHING);

            //// SNS topic creation
            //Topic topic = stack.CreateSnsTopic(resourcePrefix, "mehul.jamod@analogfolk.com");

            //// add ecs alarms to sns action
            //var snsAction = new SnsAction(topic);
            //cupAlarm.AddAlarmAction(snsAction);
            //memoryAlarm.AddAlarmAction(snsAction);
            //runningTaskCountAlarm.AddAlarmAction(snsAction);

            return feECSService;
        }

    }
       
}


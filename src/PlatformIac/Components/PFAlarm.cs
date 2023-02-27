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
	public static class PFAlarm
	{
        // Function to Alarm
        public static Alarm CreateAlarm(this PFStackBase stack, string alarmId, string alarmName, Metric metric,
                                    double threshold, double evaluationPeriods, ComparisonOperator? comparisonOperator, TreatMissingData? treatMissingData)
        {
            Alarm alarm = new Alarm(stack, 
                                      alarmId,
                                      new AlarmProps
                                      {
                                          AlarmName = alarmName,
                                          Metric = metric,
                                          Threshold = threshold,
                                          EvaluationPeriods = evaluationPeriods,
                                          ComparisonOperator = comparisonOperator,
                                          TreatMissingData = treatMissingData
                                      });

            return alarm;
        }
    }
}


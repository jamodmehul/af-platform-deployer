using System;
using Constructs;
using PlatformIac.Components;
using PlatformIac.Stacks.Common;
using Amazon.CDK;
using Amazon.CDK.AWS.CloudWatch;
using System.ComponentModel;
using Amazon.CDK.AWS.Route53;
using System.Collections.Generic;

namespace PlatformIac.Stacks
{
    public class HostedZonesStack : PFStackBase
    {
        public HostedZonesStack(Construct scope, PFStackProps stackProps)
            : base(scope, stackProps.StackName, stackProps)
        {
            HostedZone gwlHostedZone = new HostedZone(this, "Z0090668220UDDUR0VCCG", new HostedZoneProps { ZoneName = "growthwise.uk" });
            Metric gwlMetric = new Metric(new MetricProps
            {
                Namespace = "AWS/Route53",
                MetricName = "DNSQueries",
                DimensionsMap = new Dictionary<string, string> {
                    { "gwl-HostedZoneId", gwlHostedZone.HostedZoneId }
                }
            });

            new CnameRecord(this, "jwtqaenvid", new CnameRecordProps
            {
                RecordName = "jwtqaenv",
                Zone = gwlHostedZone,
                DomainName = "dyurqcutc1kmt.cloudfront.net"
            });

            new CnameRecord(this, "certid", new CnameRecordProps
            {
                RecordName = "_0690d184d36024c544dd8b8e9e5fc5f6",
                Zone = gwlHostedZone,
                DomainName = "_418fbf49db17d02ab1b50534b5711342.htgdxnmnnj.acm-validations.aws."
            });

            new CnameRecord(this, "jwtsonarqubeid", new CnameRecordProps
            {
                RecordName = "jwtsonarqube",
                Zone = gwlHostedZone,
                DomainName = "sonarqube-lb-299271916.eu-west-2.elb.amazonaws.com"
            });

            new CnameRecord(this, "jwttestenvid", new CnameRecordProps
            {
                RecordName = "jwttestenv",
                Zone = gwlHostedZone,
                DomainName = "dnpsn1lju50qr.cloudfront.net"
            });

            new CnameRecord(this, "jwttestenvlbid", new CnameRecordProps
            {
                RecordName = "jwt-test",
                Zone = gwlHostedZone,
                DomainName = "jwt-test-frontend-ecs-lb-734103629.eu-west-2.elb.amazonaws.com"
            });


        }
    }
}


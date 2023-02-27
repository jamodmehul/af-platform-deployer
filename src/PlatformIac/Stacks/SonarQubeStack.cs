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
	public sealed class SonarQubeStack : PFStackBase
	{
        //internal Vpc Vpc { get; }
        const string sqLBCertArn = "arn:aws:acm:eu-west-2:316656599330:certificate/453be658-eec5-4028-8a7b-56677330c11c";

        internal SonarQubeStack(Construct scope, PFStackProps stackProps)
			: base(scope, stackProps.StackName, stackProps)
		{
            Amazon.CDK.Tags.Of(this).Add(Constants.GLOBAL_TAG_NAME_ENVIRONMENT, stackProps.Environment.Name);

            string resourcePrefix = Constants.APPLICATION_SUFFIX + "-" + stackProps.Environment.Name;

            Vpc vpc = this.CreateVpc(Constants.SONARQUBE_VPC_CIDR);

            // Create security group sonar cube server
            SecurityGroup jwtSonarcubeWebSG = this.CreateSecurityGroup(vpc, "jwtSonarCubeWebSG", "jwt-sonarcube-web-sg", "Security group for sonar cube web access");
            jwtSonarcubeWebSG.AddIngressRule(Peer.Ipv4("0.0.0.0/0"), Port.Tcp(9000), "all traffic");
            jwtSonarcubeWebSG.AddIngressRule(Peer.Ipv4("0.0.0.0/0"), Port.Tcp(443), "all traffic");
            jwtSonarcubeWebSG.AddIngressRule(Peer.Ipv4("0.0.0.0/0"), Port.Tcp(22), "SSH");
            //jwtUmbracoDevWebSG.AddIngressRule(Peer.Ipv4("164.39.178.139/32"), Port.TcpRange(49152, 65534), "FTPSERVER - PC - OFFICE");


            // Creating Instance
            IDictionary<string, string> d = new Dictionary<string, string>();
            d.Add(new KeyValuePair<string, string>(Region, "ami-038d76c4d28805c09")); // Ubuntu 20.02

            IMachineImage mi = MachineImage.GenericLinux(d);

            // Comments for user data script
            UserData userdata = UserData.ForLinux();
            userdata.AddCommands("sudo snap install docker", "sudo addgroup --system docker", "sudo adduser ubuntu docker", "newgrp docker", "sudo snap disable docker", "sudo snap enable docker");

            var instanceId = "sonarQubeInstance";
            var instanceName = "sonarqube-web";
            var instanceType = "t3.medium";
            var instanceKey = "jwt-dev-key";
            var sonarCubeInstance = this.CreateInstance(instanceId, instanceName, instanceType, instanceKey, jwtSonarcubeWebSG, userdata, vpc, mi);


            // Create EIP for Sonar Qube EC2 Instance
            CfnEIP sqEIP = this.CreateEIP("sonarqubeEIP", "SonarQubeWebEIP");

            // Associate EIP to instance 
            var instanceAssociation = new CfnEIPAssociation(this, "sonarQubeEIPAcc", new CfnEIPAssociationProps
            {
                Eip = sqEIP.Ref,
                InstanceId = sonarCubeInstance.InstanceId
            });

            // Create security group for sonar qube load balancer
            SecurityGroup sonarQubeLoadBalanberSG = this.CreateSecurityGroup(vpc, "sqLBSG", "sonarqube-loadbalancer-sg", "Security group for sonar qube load balancer");
            sonarQubeLoadBalanberSG.AddIngressRule(Peer.Ipv4("0.0.0.0/0"), Port.Tcp(80), "all traffic");
            sonarQubeLoadBalanberSG.AddIngressRule(Peer.Ipv4("0.0.0.0/0"), Port.Tcp(443), "all secure traffic");

            // Create Load Balancer for sonar qube instance
            ApplicationLoadBalancer sonarQubeALB = this.CreateALB( "sq-lb", "sonarqube-lb", vpc, sonarQubeLoadBalanberSG);


            // Adding listeners to ecs load balancer
            string sqListener80Id = resourcePrefix + "-sqListener80";
            ApplicationListener sqListener80 = this.AddListnerToAlb(sonarQubeALB,
                                sqListener80Id,
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

            // Create Target group
            var sqAlbTg = new ApplicationTargetGroup(this, "sqListener443Target", new ApplicationTargetGroupProps
            {
                Port = 9000,
                Protocol = ApplicationProtocol.HTTP,
                TargetGroupName = "SQAlbTg",
                HealthCheck = new Amazon.CDK.AWS.ElasticLoadBalancingV2.HealthCheck
                {
                    Path = "/"
                },
                TargetType = TargetType.INSTANCE,
                Targets = new IApplicationLoadBalancerTarget[]
                {
                    new InstanceIdTarget(sonarCubeInstance.InstanceId),
                },
                Vpc = vpc
            });

            // Create listner for SSL connection
            string sqListener443Id = resourcePrefix + "-listner443";
            ApplicationListener listener443 = this.AddListnerToAlb(sonarQubeALB,
                                sqListener443Id,
                                443,
                                null,
                                new IListenerCertificate[] { ListenerCertificate.FromArn(certificateArn: sqLBCertArn) },
                                new IApplicationTargetGroup[] { sqAlbTg });

        }

    }
}


using System;
using Amazon.CDK;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.WAFv2;
using static Amazon.CDK.AWS.WAFv2.CfnWebACL;
using PlatformIac.Stacks.Common;

namespace PlatformIac.Components
{
    public static class PFCdn
    {
        public static CloudFrontWebDistribution CreateCloudFrontWebDistribution(this PFStackBase stack, string cdnId, string domainName)
        {
            //ViewerCertificate cert = ViewerCertificate.FromAcmCertificate(globalCert, new ViewerCertificateOptions
            //{
            //    Aliases = new[] { "afqaenv.growthwise.uk" },
            //    SecurityPolicy = SecurityPolicyProtocol.SSL_V3,  // default
            //    SslMethod = SSLMethod.SNI
            //});

            CloudFrontWebDistribution cdn = new CloudFrontWebDistribution(stack, cdnId, new CloudFrontWebDistributionProps
            {
                
                OriginConfigs = new[]
                {
                    //ViewerCertificate.FromAcmCertificate(globalCert, new ViewerCertificateOptions
                    //{
                    //    Aliases = new[] { "af-testenv.growthwise.uk" },
                    //    SecurityPolicy = SecurityPolicyProtocol.SSL_V3,  // default
                    //    SslMethod = SSLMethod.SNI
                    //}),
                    new SourceConfiguration
                    {
                        Behaviors = new [] { new Behavior {
                            IsDefaultBehavior = true,

                        } },
                        
                        CustomOriginSource = new CustomOriginConfig
                        {
                             DomainName = domainName,

                             // the properties below are optional
                             AllowedOriginSSLVersions = new [] { OriginSslPolicy.SSL_V3 },
                             HttpPort = 80,
                             HttpsPort = 443,
                             
                             //OriginHeaders = new Dictionary<string, string>
                             //{
                             //    { "originHeadersKey", "originHeaders" }
                             //},
                             //OriginKeepaliveTimeout = Duration.Minutes(30),
                             //OriginPath = "originPath",
                             OriginProtocolPolicy = OriginProtocolPolicy.HTTPS_ONLY,
                             //OriginReadTimeout = Duration.Minutes(30),
                             //OriginShieldRegion = "originShieldRegion"
                        }
                    }
                }
            });

            return cdn;
        }
    }
}


#!/bin/bash
echo Building the source code
dotnet build src

echo Executing synth
cdk synth

Deploying Test Environment stack
STACKNAME="Jwt-test-TestEnv"
echo Checking stack : $STACKNAME
cdk diff $stackame --fail && cdkDiff="N" || cdkDiff="Y"
if [ $cdkDiff == "N" ]
then
    echo No difference found in $STACKNAME stack
else
    echo Differences found in $STACKNAME stack, do you want to deploy ? 
    echo Enter Yes/yes/Y/y to proceed
    read goahed
    if [ $goahed == "Yes" ] || [ $goahed == "yes" ] || [ $goahed == "y" ] || [ $goahed == "Y" ]
    then
        echo deploying changes to stack : $STACKNAME
        cdk deploy $STACKNAME
    else
        echo Exiting .... Changes not deployed ....
    fi
fi

# Deploying QA Environment stack
# STACKNAME="Jwt-QA-QAEnv"
# echo Checking stack : $STACKNAME
# cdk diff $stackame --fail && cdkDiff="N" || cdkDiff="Y"
# if [ $cdkDiff == "N" ]
# then
#     echo No difference found in $STACKNAME stack
# else
#     echo Differences found in $STACKNAME stack, do you want to deploy ? 
#     echo Enter Yes/yes/Y/y to proceed
#     read goahed
#     if [ $goahed == "Yes" ] || [ $goahed == "yes" ] || [ $goahed == "y" ] || [ $goahed == "Y" ]
#     then
#         echo deploying changes to stack : $STACKNAME
#         cdk deploy $STACKNAME
#     else
#         echo Exiting .... Changes not deployed ....
#     fi
# fi


# Deploying Jwt-common-SonarQube
# STACKNAME="Jwt-common-SonarQube"
# echo Checking stack : $STACKNAME
# cdk diff $stackame --fail && cdkDiff="N" || cdkDiff="Y"
# if [ $cdkDiff == "N" ]
# then
#     echo No difference found in $STACKNAME stack
# else
#     echo Differences found in $STACKNAME stack, do you want to deploy ? 
#     echo Enter Yes/yes/Y/y to proceed
#     read goahed
#     if [ $goahed == "Yes" ] || [ $goahed == "yes" ] || [ $goahed == "y" ] || [ $goahed == "Y" ]
#     then
#         echo deploying changes to stack : $STACKNAME
#         cdk deploy $STACKNAME
#     else
#         echo Exiting .... Changes not deployed ....
#     fi
# fi

# # Deploying Jwt-dev-DevEnv
# STACKNAME="Jwt-dev-DevEnv"
# echo Checking stack : $STACKNAME
# cdk diff $stackame --fail && cdkDiff="N" || cdkDiff="Y"
# if [ $cdkDiff == "N" ]
# then
#     echo No difference found in $STACKNAME stack
# else
#     echo Differences found in $STACKNAME stack, do you want to deploy ? 
#     echo Enter Yes/yes/Y/y to proceed
#     read goahed
#     if [ $goahed == "Yes" ] || [ $goahed == "yes" ] || [ $goahed == "y" ] || [ $goahed == "Y" ]
#     then
#         echo deploying changes to stack : $STACKNAME
#         cdk deploy $STACKNAME
#     else
#         echo Exiting .... Changes not deployed ....
#     fi
# fi


# Deploying Hosted Zones Stack
STACKNAME="Jwt-common-HostedZones"
echo Checking stack : $STACKNAME
cdk diff $stackame --fail && cdkDiff="N" || cdkDiff="Y"
if [ $cdkDiff == "N" ]
then
    echo No difference found in $STACKNAME stack
else
    echo Differences found in $STACKNAME stack, do you want to deploy ? 
    echo Enter Yes/yes/Y/y to proceed
    read goahed
    if [ $goahed == "Yes" ] || [ $goahed == "yes" ] || [ $goahed == "y" ] || [ $goahed == "Y" ]
    then
        echo deploying changes to stack : $STACKNAME
        cdk deploy $STACKNAME
    else
        echo Exiting .... Changes not deployed ....
    fi
fi


# Deploying JMeter Stack
# STACKNAME="Jwt-common-JMeter"
# echo Checking stack : $STACKNAME
# cdk diff $stackame --fail && cdkDiff="N" || cdkDiff="Y"
# if [ $cdkDiff == "N" ]
# then
#     echo No difference found in $STACKNAME stack
# else
#     echo Differences found in $STACKNAME stack, do you want to deploy ? 
#     echo Enter Yes/yes/Y/y to proceed
#     read goahed
#     if [ $goahed == "Yes" ] || [ $goahed == "yes" ] || [ $goahed == "y" ] || [ $goahed == "Y" ]
#     then
#         echo deploying changes to stack : $STACKNAME
#         cdk deploy $STACKNAME
#     else
#         echo Exiting .... Changes not deployed ....
#     fi
# fi

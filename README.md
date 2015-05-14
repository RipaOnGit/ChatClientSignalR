# ChatClientSignalR
Azure Web Application that uses SignalR technology and Azure Table Storage.

Following script is for Azure Cloud Application deployment.

#Param([string]$publishsettings="..\FreeTrial-5-13-2015-credentials.publishsettings",
#      [string]$storageaccount="ripa******",
#      [string]$subscription="Free Trial",
#      [string]$cloudServiceName="Simple****",
#      [string]$containerName="vsd****",
#      [string]$cscfg="C:\SimpleChatDemoCloudService\SimpleChatDemoCloudService\bin\Debug\app.publish\ServiceConfiguration.Cloud.cscfg",
#      [string]$cspkg="C:\SimpleChatDemoCloudService\SimpleChatDemoCloudService\bin\Debug\app.publish\SimpleChatDemoCloudService.cspkg",
#      [string]$slot="Staging")
	  
#Modified and simplified version of https://www.windowsazure.com/en-us/develop/net/common-tasks/continuous-delivery/
#From: #https://gist.github.com/3694398
$subscriptionId = "bee93dc3-9eb****"
$subscription = "Free Trial" #this the name from your .publishsettings file
$service = "ripa****" #this is the name of the cloud service you created
$storageAccount = "ripasstorage1" #this is the name of the storage service you created
$slot = "staging" #staging or production
$package = "C:\SimpleChatDemoCloudService\SimpleChatDemoCloudService\bin\Debug\app.publish\SimpleChatDemoCloudService.cspkg"
$configuration = "C:\SimpleChatDemoCloudService\SimpleChatDemoCloudService\bin\Debug\app.publish\ServiceConfiguration.Cloud.cscfg"
$publishSettingsFile = "C:\SimpleChatDemoCloudService\SimpleChatDemoCloudService\bin\Debug\app.publish\Free Trial-5-13-2015-credentials.publishsettings"
$timeStampFormat = "g"
$deploymentLabel = "PowerShell Deploy to $service"
  
Write-Output "Slot: $slot"
Write-Output "Subscription: $subscription"
Write-Output "Service: $service"
Write-Output "Storage Account: $storageAccount"
Write-Output "Slot: $slot"
Write-Output "Package: $package"
Write-Output "Configuration: $configuration"
 
Write-Output "Running Azure Imports"
Import-Module "C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\*.psd1"
Import-AzurePublishSettingsFile $publishSettingsFile
Set-AzureSubscription -CurrentStorageAccount $storageAccount -SubscriptionName $subscription
Set-AzureService -ServiceName $service -Label $deploymentLabel
  
function Publish(){
 $deployment = Get-AzureDeployment -ServiceName $service -Slot $slot -ErrorVariable a -ErrorAction silentlycontinue
  
 if ($a[0] -ne $null) {
    Write-Output "$(Get-Date -f $timeStampFormat) - No deployment is detected. Creating a new deployment. "
 }
  
 if ($deployment.Name -ne $null) {
    #Update deployment inplace (usually faster, cheaper, won't destroy VIP)
    Write-Output "$(Get-Date -f $timeStampFormat) - Deployment exists in $servicename.  Upgrading deployment."
    UpgradeDeployment
 } else {
    CreateNewDeployment
 }
}
  
function CreateNewDeployment()
{
    write-progress -id 3 -activity "Creating New Deployment" -Status "In progress"
    Write-Output "$(Get-Date -f $timeStampFormat) - Creating New Deployment: In progress"
  
    $opstat = New-AzureDeployment -Slot $slot -Package $package -Configuration $configuration -label $deploymentLabel -ServiceName $service
  
    $completeDeployment = Get-AzureDeployment -ServiceName $service -Slot $slot
    $completeDeploymentID = $completeDeployment.deploymentid
  
    write-progress -id 3 -activity "Creating New Deployment" -completed -Status "Complete"
    Write-Output "$(Get-Date -f $timeStampFormat) - Creating New Deployment: Complete, Deployment ID:
 
$completeDeploymentID"
}
  
function UpgradeDeployment()
{
    write-progress -id 3 -activity "Upgrading Deployment" -Status "In progress"
    Write-Output "$(Get-Date -f $timeStampFormat) - Upgrading Deployment: In progress"
  
    # perform Update-Deployment
    $setdeployment = Set-AzureDeployment -Upgrade -Slot $slot -Package $package -Configuration $configuration -label $deploymentLabel -ServiceName $service -Force
  
    $completeDeployment = Get-AzureDeployment -ServiceName $service -Slot $slot
    $completeDeploymentID = $completeDeployment.deploymentid
  
    write-progress -id 3 -activity "Upgrading Deployment" -completed -Status "Complete"
    Write-Output "$(Get-Date -f $timeStampFormat) - Upgrading Deployment: Complete, Deployment ID: $completeDeploymentID"
}
  
Write-Output "Create Azure Deployment"
Publish

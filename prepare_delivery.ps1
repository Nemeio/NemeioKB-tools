#Set yout environment : develop, testing, master
$environment="master"
#Put yout  Azure SAS token into this string"
$azure_sas_token = "sv=***&ss=***&srt=***&sp=***&st=***&se=***&sig=***"


$request = 'https://nemeioupdateinquiry.azurewebsites.net/api/embedded/delivery?environment='+$environment

$result = Invoke-WebRequest $request |
ConvertFrom-Json |
select -expand components |
select component, url

$binarypath = "./Delivery/Binaires/"
$isopath = "./Delivery/installer/"

foreach($comp in $result)
{	
	$dlrequest = $comp.url+'?'+$azure_sas_token	
	
	if($comp.component -eq 'sfb')
	{
		$filename = "LDLC-Karmeliet.sfb"
		Invoke-WebRequest $dlrequest -OutFile $binarypath$filename
	}
	elseif($comp.component -eq 'sbsfu')
	{
		$filename = "LDLC-Karmeliet-SBSFU.bin"
		Invoke-WebRequest $dlrequest -OutFile $binarypath$filename
	}
	elseif($comp.component -eq 'scratch')
	{
		$filename = "Scratch-Install.bin"
		Invoke-WebRequest $dlrequest -OutFile $binarypath$filename
	}
	elseif($comp.component -eq 'cpu-no-bootloader')
	{
		$filename = "LDLC-Karmeliet.bin"
		Invoke-WebRequest $dlrequest -OutFile $binarypath$filename
	}
}

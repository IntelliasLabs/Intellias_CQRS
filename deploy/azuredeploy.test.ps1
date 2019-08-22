param ([Parameter(Mandatory=$true)][string]$armOutput)

$json = $armOutput | ConvertFrom-Json

Write-Host $json

Write-Host "##vso[task.setvariable variable=StorageAccount.ConnectionString;]${json.testsStorageAccountConnectionString.value}"

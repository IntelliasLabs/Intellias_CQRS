param ([Parameter(Mandatory=$true)][string]$armOutput)

$json = $armOutput | ConvertFrom-Json

Write-Host ("##vso[task.setvariable variable=StorageAccount__ConnectionString;]{0}" -f $json.testsStorageAccountConnectionString.value)

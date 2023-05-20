using namespace System
using namespace System.IO

[CmdletBinding()]
param(
  $CertificatePath = 'localhost.crt',
  $PrivateKeyPath = 'localhost.key',
  $Days = 365,
  $Country,
  $State,
  $Locality,
  $Organization,
  $OrganizationUnit,
  $CN,
  $Email
)

$CertificatePath = [Path]::GetFullPath($CertificatePath, $PSScriptRoot)
$PrivateKeyPath = [Path]::GetFullPath($PrivateKeyPath, $PSScriptRoot)

$cert_path_directory = [Path]::GetDirectoryName($CertificatePath)
$key_path_directory = [Path]::GetDirectoryName($PrivateKeyPath)

if (-not [Directory]::Exists($cert_path_directory)) {
  [Directory]::CreateDirectory($cert_path_directory)
}

if (-not [Directory]::Exists($key_path_directory)) {
  [Directory]::CreateDirectory($key_path_directory)
}

"$Country`n$State`n$Locality`n$Organization`n$OrganizationUnit`n$CN`n$Email" | openssl req -x509 -new -nodes -out "$CertificatePath" -keyout "$PrivateKeyPath" -days $Days

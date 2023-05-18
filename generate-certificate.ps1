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

"$Country`n$State`n$Locality`n$Organization`n$OrganizationUnit`n$CN`n$Email" | openssl req -x509 -new -nodes -out "$CertificatePath" -keyout "$PrivateKeyPath" -days $Days

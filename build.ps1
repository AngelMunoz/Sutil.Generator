[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string]
    $library = "shoelace"
)

$project = $library -eq "shoelace" ? "Sutil.Shoelace" : "Sutil.Fast"

$root = Get-Location


Remove-Item $library -R -Force -ErrorAction Ignore;
Remove-Item dist -R -Force -ErrorAction Ignore;
dotnet tool restore
set-location $root/src/Generator
dotnet run -C Release -- -cs $library
if ($LASTEXITCODE -gt 0) {
    Exit 1
}
dotnet build
if ($LASTEXITCODE -gt 0) {
    Exit 1
}
Set-Location $root
dotnet fable --cwd src/$project
if ($LASTEXITCODE -gt 0) {
    Exit 1
}
dotnet pack src/$project -o dist
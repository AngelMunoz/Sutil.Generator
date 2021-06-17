Remove-Item Sutil.Shoelace -R -Force -ErrorAction Ignore;
Remove-Item dist -R -Force -ErrorAction Ignore;
dotnet tool restore
dotnet run -C Release
if ($LASTEXITCODE -gt 0) {
    Exit 1
}
dotnet build src/Sutil.Shoelace/Sutil.Shoelace
if ($LASTEXITCODE -gt 0) {
    Exit 1
}
dotnet fable --cwd src/Sutil.Shoelace/Sutil.Shoelace
if ($LASTEXITCODE -gt 0) {
    Exit 1
}
dotnet pack src/Sutil.Shoelace/Sutil.Shoelace -o dist
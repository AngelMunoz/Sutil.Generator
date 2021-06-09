Remove-Item Sutil.Shoelace -R -Force -ErrorAction Ignore;
Remove-Item dist -R -Force -ErrorAction Ignore;
dotnet tool restore
dotnet run -C Release
if ($LASTEXITCODE -gt 0) {
    Exit 1
}
dotnet build Sutil.Shoelace 
if ($LASTEXITCODE -gt 0) {
    Exit 1
}
dotnet fable --cwd Sutil.Shoelace
if ($LASTEXITCODE -gt 0) {
    Exit 1
}
dotnet pack Sutil.Shoelace -o dist
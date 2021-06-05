Remove-Item Sutil.Shoelace -R -Force
Remove-Item dist -R -Force
dotnet tool restore
dotnet run -C Release
dotnet build Sutil.Shoelace 
dotnet fable --cwd Sutil.Shoelace
dotnet pack Sutil.Shoelace -o dist
$project = Join-Path $PSScriptRoot '\src\nxDumpFuse\nxDumpFuse.csproj'
$output = Join-Path $PSScriptRoot '\src\nxDumpFuse\bin\Release\net5.0\publish\'
$runtimes = @(
 "win-x64"
 "osx-x64"
 "linux-x64"
 )
    
 # clear previous releases
Remove-Item "$output\*" -Force -Recurse -Confirm:$true
    
 $runtimes | %{
     & dotnet publish $project -c release -r $_  -o ("{0}\{1}" -f $output,$_)
 }
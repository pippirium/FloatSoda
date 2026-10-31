[CmdletBinding()]
param(
    [string]$OutputDirectory = (Join-Path $env:LOCALAPPDATA 'FloatSoda/Samples/Settings')
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../samples/FloatSoda.Samples.Settings/FloatSoda.Samples.Settings.csproj'
$destination = [System.IO.Path]::GetFullPath($OutputDirectory)

dotnet publish $project --configuration Release --self-contained false --output $destination
if ($LASTEXITCODE -ne 0) {
    throw "Settingsサンプルの発行に失敗しました（終了コード: $LASTEXITCODE）。"
}

Write-Output (Join-Path $destination 'FloatSoda.Samples.Settings.exe')

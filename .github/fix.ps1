$ErrorActionPreference = "Stop"

$env:OLD_GITHUB_TOKEN = $env:GITHUB_TOKEN
$env:GITHUB_TOKEN = $null
$env:OLD_GH_TOKEN = $env:GH_TOKEN
$env:GH_TOKEN = $null

try
{
    gh auth login --web -h github.com -s repo -p https

    .\setup-branch-ruleset.ps1
    .\setup-production-environment.ps1
    .\setup-repo-settings.ps1
}
finally
{
    $env:GITHUB_TOKEN = $env:OLD_GITHUB_TOKEN
    $env:GH_TOKEN = $env:OLD_GH_TOKEN
}

# Run this from the codespace in the workspace directory.
# The script should automatically pick up the correct target repository.

$ErrorActionPreference = "Stop"

$repo = & gh repo view --json nameWithOwner --jq .nameWithOwner
$targetBranch = "main"

$rulesets = (& gh api /repos/$repo/rules/branches/$targetBranch) | ConvertFrom-Json | %{ $_.ruleset_id } | Select-Object -Unique

$rulesets | %{
    write-host "Deleting ruleset: $_"
    gh api --method DELETE /repos/$repo/rulesets/$_
}

Write-Host "Creating new ruleset named: Protect $targetBranch"


& gh api --method POST /repos/$repo/rulesets --input $PSScriptRoot/setup-branch-ruleset.json
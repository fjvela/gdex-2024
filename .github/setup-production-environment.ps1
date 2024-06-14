# Run this from the codespace in the workspace directory.
# The script should automatically pick up the correct target repository.

$ErrorActionPreference = "Stop"

$repo = & gh repo view --json nameWithOwner --jq .nameWithOwner
$org = & gh repo view --json owner --jq .owner.login
$teamName = & gh repo view --json name --jq .name
$teamId = & gh api https://api.github.com/orgs/$org/teams/$teamName --jq .id
$targetEnvironment = "production"

$environments = (& gh api https://api.github.com/repos/$repo/environments) | ConvertFrom-Json

if ($environments | Where-Object { $_.name -ieq 'targetEnvironment' }) {
  Write-Host "Environment already exists, deleting: $targetEnvironment"
  & gh api https://api.github.com/repos/$repo/environments/$targetEnvironment -X DELETE
} 

Write-Host "Creating new environment named: $targetEnvironment"
& gh api https://api.github.com/repos/$repo/environments/$targetEnvironment -X PUT

Write-Host "Setting desired policy"
gh api --method PUT /repos/$repo/environments/$targetEnvironment `
    -F "prevent_self_review=true" `
    -f "reviewers[][type]=Team" `
    -F "reviewers[][id]=$teamId" `
    -F "deployment_branch_policy[protected_branches]=true" `
    -F "deployment_branch_policy[custom_branch_policies]=false"
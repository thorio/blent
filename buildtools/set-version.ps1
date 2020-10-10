# extract package version
[xml]$csproj = Get-Content src/Blent/Blent.csproj
$version = "${csproj.Project.PropertyGroup.Version}".Trim()
$sha = "${env:APPVEYOR_REPO_COMMIT}".Substring(0, 8)

if ($env:APPVEYOR_REPO_BRANCH -ne "master") {
	$branch = $env:APPVEYOR_REPO_BRANCH -replace '[^\d\w]+','-'
	$version += "-${branch}+${sha}"
} elseif ($env:APPVEYOR_PULL_REQUEST_HEAD_COMMIT -eq "") {
	$pr_branch = $env:APPVEYOR_PULL_REQUEST_HEAD_REPO_BRANCH -replace '[^\d\w]+','-'
	$version += "-pr-${pr_branch}+${sha}"
}

try {
	Update-AppveyorBuild -Version "$version"
} catch {
	Exit-AppveyorBuild # can happen when the version is not unique.
}
echo "set version: $env:APPVEYOR_BUILD_VERSION"

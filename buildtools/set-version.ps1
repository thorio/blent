# extract package version
[xml]$csproj = Get-Content src/Blent/Blent.csproj
$version = $csproj.Project.PropertyGroup.Version
$version = "$version".Trim()

$sha = "${env:APPVEYOR_REPO_COMMIT}".Substring(0, 8)

if ($env:APPVEYOR_REPO_BRANCH -eq "master") {
	# release build (1.0.0)
} elseif ($env:APPVEYOR_PULL_REQUEST_HEAD_COMMIT -ne $null) {
	# pr build (1.0.0-pr-feature-version-verb+8e0376c2)
	$pr_branch = $env:APPVEYOR_PULL_REQUEST_HEAD_REPO_BRANCH -replace '[^\d\w]+','-'
	$version += "-pr-${pr_branch}+${sha}"
} else {
	# branch build (1.0.0-develop+8e0376c2)
	$branch = $env:APPVEYOR_REPO_BRANCH -replace '[^\d\w]+','-'
	$version += "-${branch}+${sha}"
}

try {
	Update-AppveyorBuild -Version "$version"
} catch {
	Exit-AppveyorBuild # can happen when the version is not unique.
}
echo "set version: $env:APPVEYOR_BUILD_VERSION"

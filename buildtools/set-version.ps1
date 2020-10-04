# extract package version
[xml]$csproj = Get-Content src/Blent/Blent.csproj
$version += $csproj.Project.PropertyGroup.Version
$version = "$version".Trim()

if ($env:APPVEYOR_REPO_BRANCH -ne "master") {
	$branch = $env:APPVEYOR_REPO_BRANCH -replace '[/]+','-'
	$version += "-indev+${branch}"
}

Update-AppveyorBuild -Version "$version"
echo "set version: $env:APPVEYOR_BUILD_VERSION"

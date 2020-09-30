# extract package version
[xml]$csproj = Get-Content src/Blent/Blent.csproj
$version += $csproj.Project.PropertyGroup.Version
$version = "$version".Trim()

if ($APPVEYOR_REPO_BRANCH -ne "master") {
   $version += "+b$env:APPVEYOR_BUILD_NUMBER-$env:APPVEYOR_REPO_BRANCH"
}

Update-AppveyorBuild -Version "$version"
echo "set version: $env:APPVEYOR_BUILD_VERSION"

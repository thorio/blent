# replace placeholders in File
$file = $env:APPVEYOR_BUILD_FOLDER + "/src/blent/Constants.cs"

function Replace-Placeholder {
	param (
		$placeholder,
		$replacement
	)

	(Get-Content $file).replace($placeholder, $replacement) | Set-Content $file
	echo "${file}: replaced $placeholder with $replacement"
}

Replace-Placeholder '$$APPVEYOR_REPO_NAME$$' $env:APPVEYOR_REPO_NAME
Replace-Placeholder '$$APPVEYOR_REPO_COMMIT$$' $env:APPVEYOR_REPO_COMMIT

$last_tag = git describe --tags --abbrev=0
$git_log = (git log ${last_tag}`..HEAD --format=**%s**%n%b) | Out-String
$env:GIT_LOG_SINCE_TAG = $git_log -replace '\n|\r\n','\n'

if ($env:configuration -eq 'Release' -and $env:appveyor_repo_tag -eq 'true')
{
    # Copy ScreenShots to temporary location
    md ../Tabs
    Get-ChildItem -Path 'src/UITests/bin/Release/Tabs/*' -Include *.png | Copy-Item -Destination '../Tabs/' -Force

    # Stash to prevent conflicts when checking out
    git stash

    # Switch to GitHub Pages branch
    git checkout gh-pages

    # Copy ScreenShots to Target location
    Get-ChildItem -Path '../Tabs/*' -Include *.png | Copy-Item -Destination 'img/ScreenShots/Tabs/' -Force

    # Stage
    git add img/ScreenShots/Tabs

    # Setup git credentials
    git config --global credential.helper store
    Add-Content "$env:USERPROFILE\.git-credentials" "https://$($env:access_token):x-oauth-basic@github.com`n"
    git config --global user.email "mathew.sachin.git@outlook.com"
    git config --global user.name "Mathew Sachin"

    # Commit
    git commit -m "Update ScreenShots to $env:APPVEYOR_REPO_TAG_NAME"

    # Push back changes
    git push
}
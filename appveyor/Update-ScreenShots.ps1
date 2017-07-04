if ($env:configuration -eq 'Release' -and $env:appveyor_repo_tag -eq 'true')
{
    # Copy ScreenShots to temporary location
    md ../Tabs
    Get-ChildItem -Path 'src/UITests/bin/Release/Tabs/*' -Include *.png | Copy-Item -Destination '../Tabs/' -Force

    # git needs to know who is stashing/commiting/pushing
    git config --global user.email "mathew.sachin.git@outlook.com"
    git config --global user.name "Mathew Sachin"

    # Stash to prevent conflicts when checking out
    git stash

    # Switch to GitHub Pages branch
    git checkout gh-pages

    # Copy ScreenShots to Target location
    Get-ChildItem -Path '../Tabs/*' -Include *.png | Copy-Item -Destination 'img/ScreenShots/Tabs/' -Force

    # Stage
    git add img/ScreenShots/Tabs
    
    # Commit
    git commit -m "Update ScreenShots to $env:APPVEYOR_REPO_TAG_NAME"

    # Setup git credentials for pushing
    git config --global credential.helper store
    Add-Content "$env:USERPROFILE\.git-credentials" "https://$($env:access_token):x-oauth-basic@github.com`n"
    
    # Push back changes
    git push
}
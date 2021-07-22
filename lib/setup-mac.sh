brew install docker
curl -O -sSL https://desktop.docker.com/mac/stable/amd64/Docker.dmg
open -W Docker.dmg
cp -r /Volumes/Docker/Docker.app /Applications
sudo /Applications/Docker.app/Contents/MacOS/Docker --quit-after-install --unattended
nohup /Applications/Docker.app/Contents/MacOS/Docker --unattended &

brew install docker
curl -O -sSL https://desktop.docker.com/mac/stable/amd64/Docker.dmg
hdiutil attach Docker.dmg
cp -R /Volumes/Docker/Docker.app /Applications
open /Applications/Docker.app
/Applications/Docker.app/Contents/MacOS/Docker --quit-after-install --unattended
nohup /Applications/Docker.app/Contents/MacOS/Docker --unattended &
hdiutil detach /Volumes/Docker

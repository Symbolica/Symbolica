brew install docker docker-compose
curl -O -sSL https://download.docker.com/mac/stable/31259/Docker.dmg
open -W Docker.dmg
cp -r /Volumes/Docker/Docker.app /Applications
sudo /Applications/Docker.app/Contents/MacOS/Docker --quit-after-install --unattended
nohup /Applications/Docker.app/Contents/MacOS/Docker --unattended &

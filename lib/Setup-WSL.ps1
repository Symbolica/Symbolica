Invoke-WebRequest -Uri https://aka.ms/wsl-ubuntu-1804 -OutFile ubuntu.zip
Expand-Archive ubuntu.zip
.\ubuntu\ubuntu1804.exe install --root

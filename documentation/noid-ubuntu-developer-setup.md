# NoID Development Machine Setup 
Tested with Ubuntu 16.04.1 Studio LTS, Release 16.04, Codename Xenial
https://ubuntustudio.org/
##  Patient Hub Requirements:
####  MonoDevelop
* http://www.monodevelop.com/download/linux/
```
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
sudo apt-get update -y && sudo apt-get install -y monodevelop
```
#### Coachbase Server
```
wget http://packages.couchbase.com/releases/4.5.0/couchbase-server-enterprise_4.5.0-ubuntu14.04_amd64.deb
sudo dpkg -i couchbase-server-enterprise_4.5.0-ubuntu14.04_amd64.deb
```
#### Coachbase Server .NET Package
* https://www.nuget.org/packages/CouchbaseNetClient/

#### Install Emercoin DDNS Blockchain dependancies
```
sudo apt-get install -y build-essential libtool autotools-dev autoconf pkg-config libssl-dev libcrypto++-dev git libboost-all-dev libqt4-dev libprotobuf-dev protobuf-compiler
# optional tools
sudo apt-get install -y zip unzip libminiupnpc-dev
sudo add-apt-repository ppa:bitcoin/bitcoin && sudo apt-get update -y && sudo apt-get install -y libdb4.8-dev libdb4.8++-dev

sudo apt-get install -y make libqt5webkit5-dev libqt5gui5 libqt5core5a libqt5dbus5 qttools5-dev qttools5-dev-tools qtcreator libboost-system-dev
sudo apt-get install -y libboost-filesystem-dev libboost-program-options-dev libboost-thread-dev libstdc++6 libevent-dev libcurl4-openssl-dev libpng-dev qrencode libqrencode-dev
sudo add-apt-repository ppa:bitcoin/bitcoin && sudo apt-get update -y && sudo apt-get install -y libdb4.8-dev libdb4.8++-dev
sudo apt-get update -y && sudo apt-get upgrade -y
```

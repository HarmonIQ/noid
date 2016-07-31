# NoID Development Machine Setup 
# Tested with Ubuntu 16.04.1 Studio LTS, Release 16.04, Codename Xenial
# https://ubuntustudio.org/
# Patient Hub Requirements:
# Requirements: MonoDevelop
# http://www.monodevelop.com/download/linux/
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
sudo apt-get update -y && sudo apt-get install -y monodevelop


# Coachbase Server .NET Package
# https://www.nuget.org/packages/CouchbaseNetClient/



# Install Emercoin Blockchain dependancies
sudo apt-get install -y build-essential libtool autotools-dev autoconf pkg-config libssl-dev libcrypto++-dev git libboost-all-dev libqt4-dev libprotobuf-dev protobuf-compiler
# optional tools
sudo apt-get install -y zip unzip libminiupnpc-dev
sudo add-apt-repository ppa:bitcoin/bitcoin && sudo apt-get update -y && sudo apt-get install -y libdb4.8-dev libdb4.8++-dev

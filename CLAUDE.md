# Claude


This is a practise project to build a cli tool for linux in c# (AOT compiled). The folder you are in is in github. 
Tasks
- create an AOT compiled c# cli app which prints "Hello world" to the console
- create github CICD script which will trigger a build on 'main' branch
- in CICD, build the project and use dpkg (or whatever is needed) to create a .deb file etc
- create whatever manifest file is needed so that we can install it on debian / ubuntu systems be creating an /etc/open/sources.list.d file.....
- host that package in a public place on github so that people can donwload the package
- create a readme file which includes all the instructions for people to add the package source on debian and install the pacakge
- allow for versioning etc so that subsequent pushes to 'main' branch will build new versions and 'apt update' and 'apt upgrade' will perform the upgrade
- have a --version / -v parameter passable to the cli which will show the version and the time and date it was built in both UTC and the local time on the system . 

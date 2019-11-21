# Contribute to 2sxc / EAV - Setup

## Path Changes 2018
In 2018 we are changing some paths, specifically the path to the main dnn in c:\ - so if you had a working setup before and now have trouble bilding, check the new path which changed from _c:\projects\2sxc\web_ to _c:\projects\2sxc-dnn742\website_.

2sxc / EAV consists of many parts (JS, C#, etc.) and various github repositories. This brief manual should help you setup the parts you want to work on, or everything.

_Important: If you only want to work with 2sxc / EAV, then **you do NOT need this**. This is meant for people who want to contribute to the source code of 2sxc and EAV._

## Pre-Requisites

1. Visual Studio 2015
1. MSBuild Community Tasks 1.4 or newer (required for automatic module packaging if you want to create dnn-module packages of 2sxc to distribute to another dnn) https://github.com/loresoft/msbuildtasks or chec https://www.microsoft.com/en-in/download/details.aspx?id=19372
1. We also use grunt, gulp and bower so if you want to do some JS you'll also need to install NPM, Gulp and Bower (grunt will auto-install as a dependency)

Recommendations:

1. SQL Server 2014, but it also works on older versions like 2008

## Basics
The full solution pulls together many projects, some responsible for server-code, some for UI, some for in-page JS and some just containing translations. We separated these to make it easier to contribute to a specific part without worrying about the rest. 

If you only want to contribute to something small - like the languages / translations - then you only need to download that repository. 

In the complex scenarios each project can build into the other target, so if you want to work full-stack and use all of the automation, you will want to put everything in `C:\Projects\`.

## Example when helping with translations
The github repo for translations is https://github.com/2sic/2sxc-eav-languages

1. with the command line, go to `C:\Projects\` (of course you may want to create that folder first)
1. clone the github repo with `git clone https://github.com/2sic/2sxc-eav-languages` - this will create a folder for you called `C:\Projects\2sxc-eav-languages` containing all the languages
1. work in that project
1. commit back

Note: as you probably won't have write permissions on the main language repo, you probably can't commit back. The correct procedure then is to fork the main repo into yours, and push into your own online repo. Then start a pull request, which we can then process. 

## Full Scale Setup
The full-scale setup is meant for people, who want to work with all the pieces. As of now (December 2017) you should clone all these projects into the following folders:

### Start by Installing your DNN
Installing DNN Please install a DNN - ideally a 7.4.2 version or similar so you wont use APIs which don't exist in that version in the path above. Steps are approx. like this:

* Recommended: using NVQuickSite you can easily install DNN 7.4.2 in the `C:\Projects\2sxc-dnn742\Website` folder. This exact folder is important, because other parts (like js projects) will copy their builds into this folder
* The domain isn't important, but we recommend `2sxc-dnn742.dnndev.me`
* Manual:  Download the minimal DNN version that 2sxc supports and extract it to `C:\Projects\2sxc-dnn742\Website` (the exact folder is important, because the grunt/gulp tasks refer to this path)
  * Configure the website in IIS (use binding 2sxc.dev)
    * Use a default ASP.NET 4 / 4.5 application pool with Network Service account
  * Add entry to local hosts file: 127.0.0.1 `2sxc-dnn742.dnndev.me` (could also be different, but this is our recommendation)
  * Setup an empty database in SQL Management Studio
  * Start the website and configure DNN to use the previously created database

## Installing 2sxc as Code in the DNN
1. Then install 2sxc as a normal installable module (to get all the tables etc. setup)
1. Now you must replace the installed 2sxc with the source-code version, linked to Github.
    1. /DesktopModules/ToSIC_SexyContent folder again (required because we clone the git repository there)
    1. Use Visual Studio, Git or SourceTree to clone the 2sxc repository at https://github.com/2sic/2sxc to the empty folder ToSIC_SexyContent - eg. in `/DesktopModules/` type `git clone https://github.com/2sic/2sxc ToSic_SexyContent`
    1. also run `bower install` and `npm install` in the folder (using command-line or visual studio)

_Note: don't open the 2sxc-solution yet in visual studio - you need to later open the full package which includes the EAV with 2sxc, otherwise project references get "fixed" in a wrong way_

### Installing the additional git repositories
1. in a command line, go to `c:\Projects\`
1. run `git clone https://github.com/2sic/eav-server` - the EAV server system
1. run `git clone https://github.com/2sic/eav-ui` - the EAV UI (javascript dialogs, etc.)
1. run `git clone https://github.com/2sic/2sxc-ui` - the 2sxc / DNN UIs
1. run `git clone https://github.com/2sic/2sxc-eav-languages` - the languages
1. run `git clone https://github.com/2sic/2sxc-icons` - our custom icon-fonts

for all of these you may need to run `npm install` and/or `bower install`

## GIT Branches
We use the [git-flow convention](https://jeffkreeftmeijer.com/git-flow/), and you should familiarize yourself with this, to properly work with the branches. As of now, the development-branch is called `dev` but we'll rename it to `develop` within the next few days.  

## The EAV / 2sxc Server Solution
Basically if you open the solution in the `eav-server` folder, it will open all the EAV server (C#) code and 2sxc code in Visual Studio. This is very important, because if you build anything extensive and rename anything, it guarantees that everything is correctly updated. 

## The EAV-UI
This is a lot of JS, which when you build it using gulp `develop`, it will collect / build it all, and auto-copy to the 2sxc-folders. 

## The 2sxc UI
This extends the EAV-UI with more field types, adds configuration and also adds a lot of other JS. if you run the gulp `develop` it too will auto-build and copy to the live DNN / 2sxc site. 


## The Importance of the right Folders
The current setup places the EAV project and the 2sxc-project in exactly defined folders. This helps that when we build the EAV or make changes to it, that it will auto-copy everything in the build-process to the 2sxc bin and dist. So this isn't required, but if you want to do some serious stuff it will save you time to copy/deploy the dependencies over and over.

The 2sxc dnn should run in `c:\projects\2SexyContent\Web`


## WIP (Work In Progress)
At the moment not important yet, but some new features we're working on are in these Repos:

* `git clone https://github.com/2sic/2sxc-inpage`
* `git clone https://github.com/2sic/eav-item-dialog-angular`

[CustomizeData]:Razor-SexyContentWebPage.CustomizeData
[InstancePurpose]:Razor-SexyContentWebPage.InstancePurpose
[CustomizeSearch]:Razor-SexyContentWebPage.CustomizeSearch
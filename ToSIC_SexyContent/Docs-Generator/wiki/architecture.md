# Solution Architecture of 2sxc & EAV

## Purpose / Description
These docs should help you understand how 2sxc and EAV are built, the architecture and parts of the entire solution. 

## Architecture Overview
At the very top level, the solution consists of the following parts

1. Server Parts
    1. Core: the basic data types and interfaces
    1. App: the system which bundles these basic things together into logical units
    1. ImportExport: save/load with xml, json, excel
    1. Persistence (save / load) mostly to/from sql, also some to/from files
    1. DataSource: the system which provides data and enables querying
    1. WebAPI: the connection to the js UI
    1. 2sxc
1. Client Parts
    1. Admin UIs
    1. In-Page scripts
    1. Quick-Dialogs
    1. Main edit-ui

To discover more about where the code is on GitHub, best consult the [Contribute Setup](contribute-setup) page. 


## How it works
[//]: # "Some explanations on the functionality"
...

## Notes and Clarifications
[//]: # "just add your special cases etc. here"
...

## Read also

* 

## History


1. 
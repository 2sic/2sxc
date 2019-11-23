---
uid: Specs.StyleGuide2020
---
# 2sxc Style Guide - Best Practices v2020 DRAFT

This _Style Guide_ should help you create best-practice solutions with 2sxc. It has the version **2017** so that you can reference it as your standard, and if larger recommendation changes are made or if the standard grows, we'll create a new standard. 

This is the **current version**

## How to use
This style guide is built according to our role-model the [Angular 2 style guide by John Papa](https://angular.io/docs/ts/latest/guide/style-guide.html). This style guide presents our preferred conventions and, as importantly, explains why.



## Table Of Contents

1. [General Principles](#general-principles)
2. Solution Architecture
3. File Structure
1. [Content Types](#content-types)
2. Fields
3. Using Items as Content or Data
5. Working with Presentation and Demo-Data
3. Templates in General
1. Token Templates
2. Razor Templates
3. JavaScript Templates
4. Querying Data



## General Principles


## Use Visual Studio Code
### Style 01.11
```diff
+ Consider
```
1. use [Visual Studio Code](https://code.visualstudio.com/) and your preferred code editor
```diff
? Why
```
* it's the most agile, best aligned code-editor for modern solutions on the MS-stack
* it's great for editing all kinds of modern file formats incl. HTML, Razor, Markdown, etc.
* it's great for quickly opening a single file or an App-folder
* it offers full Git integration
* there is a [2sxc code-snippet extension](https://marketplace.visualstudio.com/items?itemName=2sic.2sxc) for VS-Code



### Use Git to Version your Work - In Local Systems as Well
#### Style 21.01
```diff
+ Do
```
1. always initialize your solution (both _App_ or _Content_) into a local git
2. regularly commit your work into your local git
```diff
+ Consider
```
1. publish your work to a server git, like github or your company internal git-repo
2. adding a readme.md for everybody who is new to your system
```diff
? Why
```
* if ever you want to undo something later on, you'll be glad you have older versions
* if ever you want to compare a deployed version with your dev-history, you'll be glad
* 2sxc supports git and github by placing everything in files, and also exporting the database into a git-versionable XML


### Use International Naming Strategy
#### Style 11.21
```diff
+ Do
```
1. use english words for all content-types, fields, file-names, folder-names and variables

```diff
? Why
```
* easier to share both code and partial solutions
* consistent setup, as often other languages will be added and English is the best shared language
* fewer problems when writing code using English variable names and english property names
* avoid special characters/words of other languages in code-parts

### User Nice, Localized Naming for Editor UX
#### Style 11.22
```diff
+ Do
```
1. provide localized (translated) names for everything the editor sees, to improve his user experience
1. after creating the english-named items you can translate them into any other culture to improve the end-user experience
1. use nice labels with spaces and more, making it easy to read
1. apply this for all **content-types** in the main _Content_ (as they are shown in normal drop-downs), **view-names** both in _Content_ as well as _Apps_ and **field-names** and **help-texts**
```diff
? Why
```
* great editor user experience
* fewer editor mistakes and frustrations




## Solution Architecture
...to do...





## File and Folder Structure
### LIFT 
#### Style xx.xx
```diff
+ Do
```
1. structure the app such that you can Locate code quickly, Identify the code at a glance, keep the Flattest structure you can, and Try to be DRY.
1. place a nice app-icon.png file containing the app-image and place it in the main folder
2. make sure it is square and at least 200x200, ideally 500x500px in size
```diff
? Why
```
* LIFT Provides a consistent structure that scales well, is modular, and makes it easier to increase developer efficiency by finding code quickly. To confirm your intuition about a particular structure, ask: can I quickly open and start work in all of the related files for this feature?
* consistent with [other style guides](https://angular.io/docs/ts/latest/guide/style-guide.html#04-01)

See also [Locate](https://angular.io/docs/ts/latest/guide/style-guide.html#04-02), [Identify](https://angular.io/docs/ts/latest/guide/style-guide.html#04-03), [Flat](https://angular.io/docs/ts/latest/guide/style-guide.html#04-04) and [T-DRY](https://angular.io/docs/ts/latest/guide/style-guide.html#04-05) of the Angular Style Guide.


### Provide an app-icon.png in each App folder
#### Style xx.xx
```diff
+ Do
```
1. create an icon for each app
1. place a nice app-icon.png file containing the app-image and place it in the main folder
2. make sure it is square and at least 200x200, ideally 500x500px in size
```diff
? Why
```
* In a future version, app-choice will also show an icon. Provide one today to be sure that it will look great.

### Place only Template-Files in the Main Folder
#### Style xx.xx
```diff
+ Do
```
1. place all templates and sub-templates in the main folder of the _App_ or the _Content_
2. place all non-template files in sub-folders
```diff
? Why
```
* This provides for a maximum overview when working on solutions
* It ensures the developer sees all views that exist

Note: there are a few system files which will also reside in this folder, especially the `app-icon.png`. This of course will also be in this folder. 

### Use src and dist folders
#### Style xx.xx
```diff
+ Consider
```
1. place all _original_ assets incl. images, js, css into the `src` folder
2. place all _runtime_ files incl. copies of the images, compiled js/css into the `dist` folder
```diff
? Why
```
* Modern developement uses many source files and much fewer runtime files
* it's a big help to keep them clearly desingnated 
* the `src` with `dist` structure has been very established in the web and JS community

### Structure the folders by Topic/Component
#### Style xx.xx
```diff
+ Do
```
1. structure your folders - especially in `src` - by topic containing all files of that topic, no matter what type
2. place different file types (js/css) belonging to the same topic into the same folder
```diff
- Avoid
```
1. organizing your folders by data-type (js, images, etc.)
```diff
? Why
```
* consistently works both for large and simple solutions
* easier to maintain
* easier to grow into larger solutions
* consistent with recommendations of other style guides like the [Angular Style-Guide](https://angular.io/docs/ts/latest/guide/style-guide.html)




## Content-Types
...to do...







### Use Singular Naming for Content-Types
#### Style 21.01
```diff
+ Consider
```
* Use the singular form to name a content type, so `BlogPost` instead of `BlogPosts`, `Tag` instead of `Tags`
* The localized name can be different as your editors need it

```diff
? Why
```
* _Content-Types_ describe the type of an item, not the collection of all items. So by definition it's not the table-of-Tags but the schema-of-a-tag-item.
* consistency is key, and it helps to always do things the same way


## Fields
...to do...




## Using Items as Content
...to do...










## Notes and Clarifications
[//]: # "just add your special cases etc. here"
...

## Read also

* [InstancePurpose](xref:HowTo.Razor.Purpose) - which tells you why the current code is running so you could change the data added
* [CustomizeData](xref:HowTo.Razor.CustomizeData)

## Demo App and further links

You should find some code examples in this demo App
* [FAQ with Categories](http://2sxc.org/en/apps/app/faq-with-categories-and-6-views)

More links: [Description of the feature on 2sxc docs](http://2sxc.org/en/Docs-Manuals/Feature/feature/2683)

## History

1. Introduced in 2sxc ??.??
2. 


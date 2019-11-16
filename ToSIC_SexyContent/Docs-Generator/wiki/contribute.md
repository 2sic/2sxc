## How to contribute to this documentation
This documentation is a wiki, stored in a github repository. This means that

### Edit online
You can edit the documentation online directly if you want to - this is ideal for new pages, small changes, correction etc. All you need is a valid github account, and you're ready to go. Just use the edit-button on every page.

![Edit buttons](assets/contribute/Edit-in-a-page.png)

## Edit offline
You can download the entire documentation as a git repository, make loads of changes, and commit these again. The git repository is called `https://github.com/2sic/2sxc.wiki.git`

Right now I'm using [Visual Studio Code][vs-code] to edit this, and I must say I like the fact that I can make many changes and commit in one go, + the instant preview while typing is really nice: 

![Edit in Visual Studio Code](assets/contribute/edit-in-vs-code.png)

## Working with images and asset-files
Adding images is very difficult when editing online (there's no upload feature) but it's very easy when editing offline, because you can create folders and push them back. Please use the following structure for now

* assets (contains all the assets)
  * logos (contains various logos, for re-use)
  * contribute (assets for the page _Contribute_)
  * \[page-name\] (assets for each specific page)

If you have any other image/file needs which need different structure, please discuss with Daniel [@iJungleboy](/iJungleboy).

## Recommended Page structure
Most of the pages will document APIs and how to use them. In general, we recommend using the structure in this [api-template][api-template] page.  

## Linking in Markdown
In general you could link just using `[Visible Name](page-name)` or `[Visible name](https://url)` - but this makes it a bit 
harder to manage in the long run. It's easier to work with, if you use the syntax `[visible name][link-name]` inline, and then
add your links to the end of the page with the syntax `[link-name]:page-name` or `[link-name]:https://url`. With this syntax
you can also add nice hover-labels which are consistent using `[Link-name]:url "comment"`.

## About the Markdown Syntax
If you're new to wikis or github comments / documentation, you may be a bit lost when it comes to markdown. I think the most important things you need to know are:

* Headers have 1-6 hashes in front, like `## Header 2`
* **bold** uses two \* chars around the text, _\_italic\__ uses one \_ char, escaping chars uses the \ slash (so any character right after a \\ is shown and doesn't format)
* Linking has many options, better read the [manuals][git-manual]
* Lists have many options, better read the [manuals][git-manual]
* Images use a \!\[alt-text\](assets/path/file.ext) syntax
* Note that line-breaks usually don't cause a line-break in the result (except in code-samples). You can enforce a simple line break by adding two spaces at the end of a line like "check out:  " (two spaces after the ":") 

Inline code starts and ends with a "\`" character - like \`code\` - it will then look like `code`

Multi-line code blocks start with three of these, like  
\`\`\`  
var x = 17; // a comment  
\`\`\`  
resulting in  
```
var x = 17; // a comment
```
Multi-line code with syntax highlighting needs you to specifiy the language c#:  
\`\`\`c#  
var x = 17; // a comment  
\`\`\`  
resulting in  
```c#
var x = 17; // a comment
```  

## Copyright
![CC-BY][cc-icon]
All docs are licensed as [CC-BY][cc]. By contributing you agree that your work can be used in this way. 

[vs-code]:https://code.visualstudio.com/
[git-manual]:/adam-p/markdown-here/wiki/Markdown-Cheatsheet
[cc]:https://creativecommons.org/licenses/by/4.0/
[cc-icon]:https://licensebuttons.net/l/by/4.0/88x31.png
[api-template]:Template-Api-Docs.md
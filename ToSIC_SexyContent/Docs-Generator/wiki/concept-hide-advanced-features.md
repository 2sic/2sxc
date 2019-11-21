
# Concept: Hide Advanced Features From Non-Admins 

## Purpose / Description
For many users 2sxc is too powerful, so there are various mechanisms for hiding the complexity from normal editors. 

## Overview

1. You can _auto-hide_ all advanced toolbar buttons using a special security group.
2. You can create _custom toolbars_ for all users, or even for certain user groups inside a Razor template
3. You can see even more advanced stuff by going into a special _debug mode_
4. You can see more internal code and messages by loading the JS-Code unminified

## Auto-Hide Advanced Buttons From Non-Admins
This is very simple - 2sxc asks DNN if there is a security group called **2sxc Designers**. If such a group exists, then only users in that group (and all host-users) see the advanced button, any other user will not see advanced buttons any more. You can find more [instructions in this 2sxc Designers checklist][checklist-sxc-designers].

## Custom Toolbars
You can create any kind of toolbar, and you can decide to show/hide which ever you want based on security roles in DNN. This requires standard C# / Razor skills and is not explained more in detail right now. 

Note that you'll find some snippets which should help you in the snippets, to both check what group a user is in, and to generate various custom toolbars. 




## Read also
[//]: # "Additional links - often within this documentation, but can also go elsewhere"

* [Checklist to setup 2sxc Designers security group][checklist-sxc-designers]



## History
[//]: # "If possible, tell when it was added or modified strongly"

1. ...

[checklist-sxc-designers]:http://swisschecklist.com/en/fwttmwjx/2sxc-Hide-advanced-features-from-Content-Editors-with-Designer-Security-Role
# Concept: Permissions

2sxc / EAV permissions help you configure who may edit/create data - optionally with "may only save as draft" (new in 9.30). Note that if something doesn't have custom permissions, you'll still have default permissions that apply (see below):

## Introduction to Permissions

Permissions are usually a list of zero or more permissions - like this:

<img src="/assets/concepts/permissions-list.jpg" width="100%">

When you edit a permission, it looks like this:

<img src="/assets/concepts/permissions-edit.jpg" width="100%">

To access the permissions, you'll usually find a person-button in the actions of an item:

<img src="/assets/concepts/permissions-access.jpg" width="100%">


## Permission Overview

Items which can accept permissions are:

1. Content Types - permissions regulate who can create / edit items of that type
1. Views / Templates - permissions regulate, who can see this view if accessed through url-parameter
1. Apps - permissions regulate across all content types, who may edit/create etc. _new in 9.30_
1. Fields - permissions regulate, who may upload files _new in 9.30_

Permissions consist of the following parts:

1. Requirements - what the current user must fulfill, for it to apply
1. Grants - what is allowed with this permission

### Understanding Requirements

For a permission to apply, you must specify for whom this is. To do this, you can choose various requirements:

1. DNN Permissions: in this case, you can specify _if the user has view permissions, this applies_
1. User Identity: in this case you can specifiy a user GUID _new in 9.30, requires the [feature to be enabled](concept-features)_
1. Group ID(s): if a user is in any of these DNN groups, the rule applies _new in 9.30, requires the [feature to be enabled](concept-features)_

### Understanding Grants

If a permission applies, it will grant the current user some rights - like creating or editing data. Grants are internally coded as a letter of the alphabet, like:

1. `c` for create
1. `d` for delete
1. `č` for create drafts only
1. `f` for full (all) permissions

A grant can contain multiple rights - like `crud` for create, read, update, delete. Many grants like `f` will automatically grant other things (obviously).

### Default Permissions

It's important to understand that for many scenarios, default permissions already apply. Super-users may always do everything, and admins may read/write all data. These default permissions cannot be reduced with new permissions at the moment, so an admin always has read/write and cannot be degraded to lower permissions.

## Using Permissions in Code

As of now, the permissions API isn't final, so if you want to use it in code, you can immitate code you can find in the source. But these commands will change when the API is final, so you'll end up making adjustments. 

## History

1. Basic permissions (ContentType / View) added ca. 2sxc 7
1. Permissions for draft-save-only added in 2sxc 9.30
1. Permissions for user IDs and groups added in 2sxc 9.30

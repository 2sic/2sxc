---
uid: Concepts.Entities
---
# Concept: Entities (Data, Records, Items)

Every _thing_, _record_ or _object_ in 2sxc is called an **Entity**. So if you have a list of `Book` objects, then each `Book` is an entity. Many other systems use the term _record_ or _item_. 

## How it Works

Each _Entity_ has many fields, some containing text, numbers etc. The fields an Entity can have is defined in the [Content-Type](xref:Concepts.ContentTypes), so each Entity is like a record of that type. 

This basic principle is used everywhere in 2sxc. For example, all these things are Entities:

* _Simple Content_ items in the _Content-App_ are entities containing a title, body and image
* [View](xref:Concepts.Views) configurations are entities containing name, thumbnail, template-name etc.
* _Blog_ posts in the [Blog App](https://2sxc.org/en/apps/app/dnn-blog-app-for-dnn-dotnetnuke) are entities containing around 20 fields
* _Tag_ items in the [Blog App](https://2sxc.org/en/apps/app/dnn-blog-app-for-dnn-dotnetnuke) are also entities
* Anything you define in your apps will result in entities

## Rich, Multilanguage Data

Entities are much more than just records, as they can have [relationships](xref:Concepts.Relationships) and each field can also be multilanguage, so there are actually many `Descriptions` in a multi-language product Entity. 

## How to Use

todo: recipes to create, change etc.

## Future Features & Wishes

1. _none as of now_

## Read also

* _none_

## History

1. Introduced in 2sxc 1.0

---
uid: ToSic.Sxc.Blocks
summary: *content
---

A unit of output it 2sxc is called a **Block**. In DNN-Link, this can be like a _Module_, except that blocks can be inside other blocks. 

Looking at it from the Platform like DNN, the entry object is an ICmsBlock which contains context information (in DNNs case, ModuleId etc.). Inside it is an IBlock which can itself contain more IBlocks. 

Note that each _Block_ itself has _Views_ @ToSic.Sxc.Blocks.IView and data specific to that block. 
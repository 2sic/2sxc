# Guide to Working with LINQ and 2sxc/EAV Data

In many cases you will want to sort, filter or group some data, or quickly check if any data was found. When using Razor or working in WebApi, this is best done with LINQ. This guide will assist you to get everything working.

For a more API-oriented documentation, see [DotNet Query LINQ](dotnet-query-linq). We also recommend to play around with the [Razor Tutorial App](https://2sxc.org/en/apps/app/razor-tutorial)

## LINQ Basics

The way LINQ works is that the namespace `System.Linq` contains a bunch of [extension methods](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods) like `.Count()`, `.Where(...)` and more. So to use LINQ you need to add a `@using` statement to razor or just `using` in a WebApi class. Here's a simple razor example:

```razor
@using System.Linq;
@{
var newestPosts = AsDynamic(App.Data["BlogPost"])
    .OrderByDescending(b => b.PublicationDate)
    .Take(3);
}
```

This demonstrates:

1. adding the `using` statement
1. getting all the _BlogPost_ items using `App.Data["BlogPost"]`
1. converting it to a list of `dynamic` objects which will allow the nice syntax using `AsDynamic(...)`
1. sorting these with newest on top using `.OrderByDescending(...)` on the property _PublicationDate_
1. keeping only the first 3 using `.Take(3)`
1. it also shows how placing the parts on separate lines makes the code easier to read

## Important: Working with LINQ and dynamic objects

### LINQ needs IEnumerable<...>
Before we continue, it's important that you really understand that LINQ commands are stored as [extension methods](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods) of `IEnumerable<T>`. So this works:

```razor
@using System.Linq;
@{
  var list = new List<string> { "word", "word" };
  var x = list.First();
}
```

...whereas this does not:

```razor
@using System.Linq;
@{
  var y = 27.First();
}
```

This sounds obvious, but there's an important catch: if the compiler doesn't know that something is an `IEnumerable`, it will not even try to use the LINQ extension methods, because it doesn't know that it can. So let's look at that...

### LINQs Problems with dynamic objects #1
Here's an example that would fail:

```razor
@using System.Linq;
@{
  dynamic list = new List<string> { "word", "word" };
  var x = list.First();
}
```

The only difference to before is that _list_ ist now `dynamic`. It contains the same object, but the compiler doesn't treat it that way. In Razor, we use `dynamic` objects all the time, where we run into this problem. Here's an example which fails:

```razor
@using System.Linq;
@{
  var books = AsDynamic(App.Data["Books"]);
  var booksWithoutAuthors = books
    .Where(b => !b.Authors.Any());
}
```

Internally the _b.Authors_ returns a list of authors, but the compiler doesn't know this, since it's treated as a `dynamic` object. You would get an error. To solve this, we must tell the compiler that _b.Authors_ is an IEnumerable, like this:

```razor
@using System.Linq;
@using System.Collections.Generic;
@{
  var books = AsDynamic(App.Data["Books"]);
  var booksWithoutAuthors = books
    .Where(b => !(b.Authors as IEnumerable<dynamic>).Any());
}
```

But let's be honest - it's ugly, long and prone to typos. Especially in a complex query where you could have many of these. So we recommend to define a shorthand for it, like this:

```razor
@using System.Linq;
@using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;
@{
  var books = AsDynamic(App.Data["Books"]);
  var booksWithoutAuthors = books
    .Where(b => !(b.Authors as Dynlist).Any());
}
```

### LINQs problem with dynamic objects #2

LINQ methods often have multiple signatures. This means the same command can be written in different ways and with different parameters. To detect the right method, the compiler needs to know the data-types used in the parameters. This causes problem with `dynamic` objects because the compiler doesn't know what it is until runtime. Check this out:

```razor
@using System.Linq;
@{
  var dogString = "dog"
  dynamic dogDyn = "dog";
  var list = new List<string> { "dog", "cat", "hound" };
  var x = list.Contains(dogString); // this works
  var x = list.Contains(dogDyn);    // this fails
}
```

To fix this, we must tell the compiler it's an object:

```razor
@using System.Linq;
@{
  dynamic dynDog = "dog";
  var list = new List<string> { "dog", "cat", "hound" };
  var x = list.Contains(dynDog as object);
}
```

The above example is a bit trivial but here's a real life example, taken from the [2sxc razor tutorial](https://2sxc.org/en/apps/app/razor-tutorial):

```razor
@using System.Linq;
@using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;
@{
  var persons = AsDynamic(App.Data["Persons"]);
  var books = AsDynamic(App.Data["Books"]);
  var booksWithAwardedAuthors = books
    .Where(b => (b.Authors as Dynlist)
      .SelectMany(a => a.Awards as Dynlist)
      .Any()
    );
  var otherBooks = books
    .Where(b => !(booksWithAwardedAuthors as Dynlist)
      .Contains(b as object)
    );
}
```

### LINQs problem with dynamic object #3

The last bit has to do with how `dynamic` objects are built, since they are usually wrapper-objects to help write nicer template code. As wrappers, they are different objects every time. This shows the problem:

```razor
@using System.Linq;
@using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;
@{
  // this is just the data object, "@bookData.Author" wouldn't work
  var bookData1 = App.Data["Books"].First();
  var bookData2 = App.Data["Books"].First();

  // this is now a dynamic object, allowing @bookDyn1.Author"
  var bookDyn1 = AsDynamic(bookData1);
  var bookDyn2 = AsDynamic(bookData2);

  var dataIsSame = bookData1 == bookData2; // true
  var dynIsSame = bookDyn1 == bookDyn2; // false before 2sxc 9.42
}
```

This doesn't sound like a big deal, but it is. Look at this code from the example above:

```razor
  var otherBooks = books
    .Where(b => !(booksWithAwardedAuthors as Dynlist)
      .Contains(b as object)
    );
```

The `.Contains(...)` clause receives a variable `b` which is actually the dynamic wrapper, and will _not_ be the same as the dynamic wrapper of dynamic wrappers given in `booksWithAwardedAuthors`. So contains would always say "nope, didn't find it".

Solving the comparison / equality problem requires the underlying wrapper object to tell the .net framework, that `==`, `!=` and a few internal methods must work differently. 2sxc 9.42 does this, so the above code would actually work in 2sxc 9.42, but not in previous versions. If another system gives you `dynamic` objects, you will probably have to write it like this:

```razor
  // this example is for non-2sxc objects or 2sxc before 9.42
  var otherBooks = books
    .Where(b => !(booksWithAwardedAuthors as Dynlist)
      .Contains(bookWithAward => bookWithAward != null && bookWithAward.SomeProperty == b.SomeProperty)
    );
```

### LINQs problem with boolean null-objects

In many cases, dynamic objects could have a property like `Show` which could be a boolean, but it could also be `null`. So this could cause an error:

```razor
var show = links.Where(x => x.Show);
```

To fix this, the easiest way is to really compare it with `true` or `false` as you want, each way will result in treating the `null` as the opposite (so you decide if null should be yes or no):

```razor
@using System.Linq;
@using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;
Dynlist list;
list = links.Where(x => x.Show == true);  // take true, skip false & null
list = links.Where(x => x.Show != true);  // take false & null, skip true
list = links.Where(x => x.Show == false); // take false, skip true & null
list = links.Where(x => x.Show != false); // take true & null, skip false
list = links.Where(x => x.Show == null);  // take null, skip true & false
```


## Read also, Demo App and further links

1. [LINQ API Docs](dotnet-query-linq)
2. [Razor Tutorial App showing all kinds of Queries](https://2sxc.org/en/apps/app/razor-tutorial)

## History

1. Guide created 2019-03

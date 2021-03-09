# Upgrade Shims

The Upgrade Assistant is a tool designed to help customers upgrade from .NET Framework to the latest versions of .NET. As customers evaluate, or begin their journey, to the latest versions of .NET they are often challenged by the distance between .NET Framework and .NET 5, especially when it comes to the web app models. The changes involve a high concept count and and applying the new approaches create a ripple effect that force changes throughout a solution leading customers to feel like the upgrade process is "death by a thousand cuts".

To address this challenge, the .NET Upgrade Assistant automates several of the key steps required along this journey, including:

1. Switching to SDK Style Projects
2. Choosing a Target Framework Moniker
3. Upgrading NuGet Package References
4. Inserting missing templates (e.g. Program.cs for ASP.NET projects)
5. Raise diagnostics for unsupported APIs/patterns
6. Applies code fixes to address diagnostics

Automating these steps helps customers leap forward in the upgrade process but also changes the code to the point where it does not compile. As an example, ASP.NET web apps often leverage features from `System.Web.Mvc` that have drastically changed in latest versions of .NET. This then forces users who are upgrading to take an all-or-nothing approach.

To support incremental migration, we can use a shim. This would effectively replace the step (5) above that raises diagnostics for unsupported APIs, while giving additional benefits.

## What is a shim?
A shim is a library that transparently redirects API calls elsewhere. This is often used to fill gaps to simplify developer productivity. This is often done in web browsers (ie JavaScript polyfills), Windows with the Microsoft Windows Application Compatibility Toolkit to allow old programs to keep running, and others.

There was a compat shim for WebApi that existed until .NET Core 2.2 found [here](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.WebApiCompatShim). This package provided shims for a number of WebApi related concepts from `System.Web.Http` including `ApiController`. This was a good first step, but shims can be provided for many more APIs to help simplify moving to .NET 5.

A shim to help .NET developers move form .NET Framework to .NET 5 should have the following characteristics:

- Provide same API shape as .NET Framework for easy compilation
- Mimic behavior as much as possible
- Provide targets that allow compilation against .NET Framework as well as .NET 5 (or .NET Standard if applicable)
- Use type forwarders where appropriate for .NET Framework implementations

Setting up shims in this way allow for cross-compilation when needed so that customers with hybrid solutions or who are not fully ready to move to .NET 5 can continue to build and run their application and slowly move to .NET 5.

As an example, see [WCF](https://github.com/dotnet/wcf) that does this so that client libraries can be cross-compiled for both the .NET Core implementation and the .NET Framework implementation.

## What does a shim give us?
When upgrading users to .NET 5, there are a number of things to deal with at a source level. The biggest hurdles many customers face is the changing of the app models. Some app models, such as WinForms and WPF, are fairly similar and don't pose as much difficulty when upgrading. Others, such as ASP.NET Core has dramatically different namespaces, types, and even architecture. Cases like this can be helped greatly by a shim.

The benefits of a shimming approach include:

- Quickly moves a customer to be running on .NET 5 where they can incrementally remove the shim
- Allow incremental approach at all levels of a solution (such as a dependent library that provides MVC helpers including filters, etc) by having a .NET 5.0 target with the shims and a .NET 4.x target that type forwards to `System.Web`.
- Contained in a separate library that does not require Roslyn knowledge to support or work with
- Provides the shape of APIs that developers are used to from System.Web
- Shimmed APIs can provide their own diagnostic ids which can be used for code fixers, external documentation, etc via an expanded `ObsoleteAttribute` in .NET 5 that allows settings a diagnostic id.

## What are the drawbacks?
There are a few drawbacks with providing a shim:

- It is more code for us to maintain. The difficulty in maintaining the equivalent diagnostics/code fixers may be as high of a burden.
- Users may stay on it for a long time. Ideally, it would be used solely as a transition point to using proper ASP.NET Core APIs. We can use code fixers to help automate some of the movement off of the code and mark them as invisible to intellisense.
- Adds an extra layer to a customer's code base that may have different behavior than they may expect.
- The shims may not be as performant as directly accessing ASP.NET Core APIs

The value shims would give to customers while upgrading are much greater than these drawbacks that may be incurred.

## Prior Art
Attempts at this have been done at different times by different teams with various levels of support:

- [CoreFxLab](https://github.com/dotnet/corefxlab/tree/master/src/System.Web.Compatibility)
- [AspLabs](https://github.com/aspnet/AspLabs/tree/master/src/migrations/framework-migrator)
- [Microsoft.AspNetCore.Mvc.WebApiCompatShim](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.WebApiCompatShim) - Only support up to .NET Core 2.2

## What should be shimmed?

Shims should be provided for `System.Web.*` namespaces:

- `System.Web.dll`
- `System.Web.Http.dll`
- `System.Web.Mvc.dll`

Shims may also be provided for the `Microsoft.AspNet.*` packages:

- `Microsoft.AspNet.Mvc`
- `Microsoft.AspNet.Razor`
- `Microsoft.AspNet.WebPages`

## Example

When looking at shimming, there are many aspects that can be done:

- Handle namespace changes (ie `System.Web.Mvc.ViewResult` to `Microsoft.AspNetCore.Mvc.ViewResult`)
- Provide similar behavior (ie `HttpContext.Session` behavior as in ASP.NET)
- Provide the shape of an API (ie `HttpContext.Current`)

As an example, `System.Web.Mvc.Controller` can be shimmed as follows:

```csharp
using CoreController = Microsoft.AspNetCore.Mvc.Controller;

namespace System.Web.Mvc
{
    [Obsolete("System.Web shims should be temporary", DiagnosticId = "SYSWEB00001", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public class Controller : CoreController
    {
        public new HttpContextBase HttpContext => base.HttpContext.GetSystemWebContext();

        protected HttpSessionStateBase Session => base.HttpContext.GetSessionStateBase();

        protected HttpServerUtilityBase Server => base.HttpContext.GetServerUtilityBase();

        [Obsolete("TryUpdateModel is now async and code should be adapted to be async", DiagnosticId = "SYSWEB00002", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
        public bool TryUpdateModel<T>(T model)
                where T : class
                => TryUpdateModelAsync(model).GetAwaiter().GetResult();

        public ActionResult HttpNotFound() => NotFound();
    }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSystemWebShim() // Add services required to support the shim
            .AddHttpContextCurrent() // Add support for a HttpContext.Current shim
            .AddSession(); // Add support for HttpContext.Session emulation
        ...
    }
}
```

Some useful things to note:

- The names align so minimal changes to a user's code base must be made to accomodate this
- The shape of things, such as `HttpSessionStateBase` can be provided with similar behavior
- `Obsolete.DiagnosticId` is available in .NET 5+ that can be used as any diagnostic id:
    - Control its visibility via editorconfig, `nowarn`, etc
    - Provide additional documentation via links to docs.microsoft.com
    - Provide codefixers that can react to those diagnostics

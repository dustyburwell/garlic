##Garlic
Google Analytics Reporting Library In C#

Google Analytics is a powerful tool for analyzing traffic patterns and conversion on a website. 
With Garlic, it can become a powerful tool for analyzing usage of any application written in
.Net, be it an MsMVC web application or a WPF desktop application.

### Use Garlic

The `Garlic.Sample.WPF` projects contains a simple WPF application using the Caliburn.Micro MVVM
framework showing how to use Garlic to report analytics data for your desktop application.

Create an analytics session

```csharp
var session = new AnalyticsSession(Domain, GACode);
```

Optionally set session level custom variables

```csharp
session.SetCustomVariable(
  1,            // variable slot (1 - 5)
  "UserName",   // key
  "dracula");   // value
```

Create a page view request and wire it up to screen activation events

```csharp
var page = session.CreatePageViewRequest(
  "/",          // path
  "Home page"); // page title

// extension method for wiring a page view request to a Caliburn.Micro 
// screen (not included)
page.Track(screen); 

// or send page views manually
page.Send();
```

Optionally set page level custom variables (page level variables override session variables in the same `slot`)

```csharp
page.SetCustomVariable(
  2,            // variable slot (1 - 5) 
  "usertype",   // key
  "member");    // value
```

Submit events

```csharp
page.SendEvent("category", "action", "label", "value");
```

or timing data

```csharp
page.SendTiming("category", "variable", 1000 /* time in millis */, "label");
```

### Get Garlic

// todo:

### Known Issues

1. Garlic does not report accurate color depth, screen resoluation, view port size, or language
2. There's no way to report page load times

### Notable Gotchas

* Each `Session` represents a unique visit and each `Page` represents a page view within that visit.
* Source will always be `(direct)` and medium will always be `(none)`
* Browser and OS information is never sent
* All users are considered "Returning Visitors"
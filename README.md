# C# Discord bot template

This is a repository containing a simple C# Discord bot template.

## About

The main template targets **.NET Core** and uses the **Discord .NET** library.

## How to use

* Download or Clone this repository
* When running in VSCode, rename `App.config.template` to `App.config` and fill in the token.
* When deploying/publishing, the application requires `<ApplicationName>.<builtExtension>.config` for example after normal .NET Core build in the same directory as your `<ApplicationName>.dll`, you have to create a `<ApplicationName>.dll.config` containing the same XML structure as `App.config.template` included in this repository.

## More templates

_If more templates are added, they will become different branches._

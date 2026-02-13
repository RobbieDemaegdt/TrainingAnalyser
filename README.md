# Maui2

A cross-platform application built with .NET MAUI (Multi-platform App UI).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- .NET MAUI workload: `dotnet workload install maui`

## Target Platforms

- iOS
- macOS (Mac Catalyst)
- Windows

## Build

```bash
# Restore dependencies
dotnet restore

# Build for Windows
dotnet build -f net10.0-windows10.0.19041.0
```

## Run

```bash
# Run on Windows
dotnet run -f net10.0-windows10.0.19041.0
```

## Project Structure

| File/Folder | Description |
|---|---|
| `App.xaml` | Application-level resources and startup |
| `AppShell.xaml` | Shell navigation structure |
| `MainPage.xaml` | Main page UI |
| `MauiProgram.cs` | App builder and service registration |
| `Platforms/` | Platform-specific code |
| `Resources/` | Images, fonts, styles, and raw assets |

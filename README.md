# input-replay
C# Console CLI input recording and playback resolution-independent application for Windows

## Key Technical Features
* **Non-Blocking Capture:** Using async loop records system-wide keyboard and mouse events without freezing the application.
* **Resolution Independence:** Automatically scales mouse coordinates using a normalized 0-65535 coordinate system via Win32 `GetSystemMetrics`, ensuring replays work across different monitor sizes.
* **Low-Level Hooks:** Interfaces directly with Windows system hooks for precise event timing and interception.
* **Modular Architecture:** Clean separation of concerns between recording, playback, and native system interfacing.

## Built With
* C# 12 / .NET 8
* Win32 API (User32.dll)
* Windows Input Simulator

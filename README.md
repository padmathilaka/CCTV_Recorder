# CCTV Recorder

This project contains a full reference implementation of a lightweight Network Video Recorder (NVR) built using C# and Windows Forms, leveraging FFmpeg to record multiple RTSP-based IP cameras simultaneously. The system is designed to be configuration-driven, with all global recording parameters and camera definitions stored in a JSON file, allowing easy updates without recompiling the application.

Each camera is handled by an independent FFmpeg process, providing better isolation, stability, and fault tolerance. A built-in watchdog continuously monitors camera connectivity, FFmpeg process health, and disk space availability, automatically restarting failed streams when possible and preventing disk exhaustion. The system also includes scheduled maintenance tasks for cleaning up old recordings and log files based on configurable retention policies.

This source code is shared for educational, testing, and development purposes. While it has been tested in real-world scenarios, behavior may vary depending on camera firmware, network reliability, FFmpeg versions, and hardware performance. You are strongly encouraged to review, modify, and test the system thoroughly in your own environment before relying on it. Use, modification, and deployment of this code are entirely at your own risk.

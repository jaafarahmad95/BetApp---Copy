﻿using System;

namespace testtest.Contract
{
    public interface ILoggerManager
    {
        void LogInfo(string message);
        void LogError(string message);
        void LogWarn(string message);
        void LogDebug(string message);

    }
}

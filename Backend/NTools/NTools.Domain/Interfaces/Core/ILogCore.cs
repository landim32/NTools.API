using System;
using NTools.Domain.Impl.Core;
using Microsoft.Extensions.Logging;

namespace NTools.Domain.Interfaces.Core
{
    public interface ILogCore
    {
        void Log(string message, Levels level);
    }
}

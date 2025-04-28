using Microsoft.SemanticKernel;
using System.ComponentModel;
using System;

namespace Obnoarding.Plugins;

public class DateTimePlugin
{
    [KernelFunction, Description("Get the current date")]
    public string Date(IFormatProvider formatProvider = null)
    {
        var date = DateTimeOffset.UtcNow.ToString("D", formatProvider);
        return date;
    }
        
    [KernelFunction, Description("Get the current date")]
    public string Today(IFormatProvider formatProvider = null) =>Date(formatProvider);
}

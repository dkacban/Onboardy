using Microsoft.SemanticKernel;
using System.ComponentModel;
using System;

namespace Obnoarding.Plugins;

public class SchedulePlugin
{
    [KernelFunction, Description("Get the current date")]
    public string Date(IFormatProvider formatProvider = null)
    {
        var date = DateTimeOffset.Now.ToString("D", formatProvider);
        return date;
    }

    [KernelFunction, Description("Get the current date")]
    public string Today(IFormatProvider formatProvider = null) =>
        this.Date(formatProvider);

    [KernelFunction, Description("Get the current date and time in the local time zone")]
    public string Now(IFormatProvider formatProvider = null) =>
        DateTimeOffset.Now.ToString("f", formatProvider);
}

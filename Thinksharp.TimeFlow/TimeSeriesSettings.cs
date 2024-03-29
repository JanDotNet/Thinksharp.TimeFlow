﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Thinksharp.TimeFlow
{
  internal class TimeSeriesSettings : ITimeSeriesSettings
  { }

  public interface ITimeSeriesSettings
  { }

  public static class TimeSeriesSettingsExtensions
  {
    public static void SetDefaultTimeZome(this ITimeSeriesSettings settings, TimeZoneInfo timeZoneInfo) => DateHelper.SetDefaultTimeZone(timeZoneInfo);
  }
}

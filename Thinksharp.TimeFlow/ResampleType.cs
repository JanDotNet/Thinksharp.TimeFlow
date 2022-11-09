using System;
using System.Collections.Generic;
using System.Text;

namespace Thinksharp.TimeFlow
{
  /// <summary>
  /// The resample type.
  /// </summary>
  public enum ResampleType
  {
    /// <summary>
    /// Resampling considers the absolut period interval. E.g. monthly time series form 01.02.2022 to 01.02.2023 resampled to year results in 2 time points (01.02.2022 - 31.12.2022 and 01.01.2022 to 31.12.2023). 
    /// </summary>
    Absolut,

    /// <summary>
    /// Resampling considers the relative period interval. E.g. monthly time series form 01.02.2022 to 01.02.2023 resampled to year results in 1 time points (01.02.2022 - 01.02.2023). 
    /// </summary>
    Relative
  }
}

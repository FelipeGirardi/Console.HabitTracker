using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HabitTracker.FelipeGirardi.Helpers;
public class ValidatorService
{
    public bool IsDateValid(string date)
    {
        return DateTime.TryParseExact(date.Trim(), "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _);
    }

    public bool IsNumberValid(string num)
    {
        return int.TryParse(num.Trim(), out _) && Convert.ToInt32(num.Trim()) >= 0;
    }

    public bool IsStringValid(string measure)
    {
        return !string.IsNullOrEmpty(measure) && !IsNumberValid(measure);
    }
}

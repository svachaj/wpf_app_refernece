using System;
using System.Collections.Generic;
using System.Text;

namespace COS.RecurrenceGenerator
{
    public class RecurrenceMananger
    {
        /// <summary>
        /// Starting point for getting Recurrence items.
        /// </summary>
        public RecurrenceMananger() { }

        /// <summary>
        /// Get a Generic List of IRecurrenceItem objects
        /// </summary>
        /// <param name="recurrenceSettings"></param>
        /// <returns>List<IRecurrenceItem></returns>
        public RecurrenceValues GetRecurrenceItems(RecurrenceSettings recurrenceSettings)
        {
            RecurrenceValues returnValues = recurrenceSettings.GetValues();
            return returnValues;
        }

    }
}

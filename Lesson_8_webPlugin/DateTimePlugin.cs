using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson_8_webPlugin
{
    public class DateTimePlugin
    {
        [KernelFunction("get_date")]
        [Description("Get today's date")]
        [return: Description("Return date format DD-MM-YYYY.")]
        public string Today()
        {
            return DateTime.Now.ToString("dd-MM-yyyy");
        }
        
        [KernelFunction("get_time")]
        [Description("Get Current time")]
        public string Current_time()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}

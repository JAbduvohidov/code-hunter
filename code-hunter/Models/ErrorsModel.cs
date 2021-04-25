using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace code_hunter.Models
{
    public class ErrorsModel<T>
    {
        public IEnumerable<T> Errors { get; set; }
    }
}
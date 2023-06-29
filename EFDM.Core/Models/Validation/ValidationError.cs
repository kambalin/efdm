﻿using EFDM.Abstractions.Models.Validation;
using System.Linq;

namespace EFDM.Core.Models.Validation
{
    public class ValidationError : IValidationError
    {
        public string Block { get; set; }
        public int? Index { get; set; }
        public string Field { get; set; }
        public string Message { get; set; }


        public override string ToString()
        {
            return string.Join(" -> ", new[] { Block, Index?.ToString(), Field, Message }.Where(o => !string.IsNullOrWhiteSpace(o)));
        }
    }
}

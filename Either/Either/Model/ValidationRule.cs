using System;
using System.Linq.Expressions;

namespace Either.Model
{
    public class ValidationRule<TValue>
    {
        public string Name { get; set; }
        public Func<TValue, bool> Validator{ get; set; }
    }
}

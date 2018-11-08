//     ParserMapper.cs is part of PBXListener.
//     Copyright (C) 2018  Fahmi Noor Fiqri
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.
#region

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#endregion

namespace Kodesiana.IO.PBX
{
    public abstract class ParserMapper
    {
        protected Type Target { get; }
        protected readonly Dictionary<string, object> Parsers = new Dictionary<string, object>();

        public ParserMapper(Type target)
        {
            Target = target;
        }

        /// <summary>
        /// Configure mapping for specified type.
        /// </summary>
        public abstract void ConfigureMapping();

        protected void Map(Type parser, string property)
        {
            Parsers.Add(property, Activator.CreateInstance(parser));
        }

        protected internal void ClearMapping()
        {
            Parsers.Clear();
            ConfigureMapping();
        }

        public string Parse(string input)
        {
            var obj = Activator.CreateInstance(Target);
            foreach (var parser in Parsers)
            {
                obj.InvokeMember(parser.Key, Helpers.SetPropertyFlags, new object[] { ((IParser)parser.Value).Parse(input) });
            }
            return obj.ToString();
        }
    }

    /// <summary>
    /// Defines generic parser mapper to map parser with it's corresponding property.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public abstract class ParserMapper<TTarget> : ParserMapper where TTarget : class, new()
    {
        public ParserMapper() : base(typeof(TTarget))
        {
        }

        /// <summary>
        /// Map specified parser with specified property.
        /// </summary>
        /// <typeparam name="TParser">Parser used to parse the data.</typeparam>
        /// <param name="property">Destination property to hold parsed data.</param>
        /// <remarks>The returned data type MUST match with the mapped property in your mapping class.
        /// For example, if you define an <c>int</c> inside your model class, you should return an 
        /// <c>int</c> from this function too.</remarks>
        protected void Map<TParser>(Expression<Func<TTarget, object>> property) where TParser : IParser, new()
        {
            var expr = (MemberExpression)property.Body;
            Map(typeof(TParser), expr.Member.Name);
        }
    }
}

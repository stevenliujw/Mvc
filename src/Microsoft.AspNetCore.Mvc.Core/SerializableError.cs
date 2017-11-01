// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Defines a serializable container for storing ModelState information.
    /// This information is stored as key/value pairs.
    /// </summary>
    public sealed class SerializableError : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableError"/> class.
        /// </summary>
        public SerializableError()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableError"/>.
        /// </summary>
        /// <param name="modelState"><see cref="ModelStateDictionary"/> containing the validation errors.</param>
        public SerializableError(ModelStateDictionary modelState)
            : this()
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }

            if (modelState.IsValid)
            {
                return;
            }

            foreach (var keyModelStatePair in modelState)
            {
                var key = keyModelStatePair.Key;
                var errors = keyModelStatePair.Value.Errors;
                if (errors != null && errors.Count > 0)
                {
                    var errorMessages = errors.Select(error =>
                    {
                        if (error.Exception is InputFormatterException)
                        {
                            // InputFormatterException is a signal that the message is safe to
                            // return to clients
                            return error.Exception.Message;
                        }
                        else if (!string.IsNullOrEmpty(error.ErrorMessage))
                        {
                            return error.ErrorMessage;
                        }
                        else
                        {
                            return Resources.SerializableError_DefaultError;
                        }
                    }).ToArray();

                    Add(key, errorMessages);
                }
            }
        }
    }
}

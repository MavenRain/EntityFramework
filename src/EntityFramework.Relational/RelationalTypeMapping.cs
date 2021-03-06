// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Relational
{
    public class RelationalTypeMapping
    {
        public RelationalTypeMapping([NotNull] string storeTypeName, DbType storeType)
        {
            Check.NotEmpty(storeTypeName, nameof(storeTypeName));

            StoreTypeName = storeTypeName;
            StoreType = storeType;
        }

        public virtual string StoreTypeName { get; }

        public virtual DbType StoreType { get; }

        public virtual DbParameter CreateParameter([NotNull] DbCommand command, [NotNull] ColumnModification columnModification, bool useOriginalValue)
        {
            Check.NotNull(command, nameof(command));
            Check.NotNull(columnModification, nameof(columnModification));

            var parameter = command.CreateParameter();
            parameter.Direction = ParameterDirection.Input;
            if (useOriginalValue)
            {
                Check.NotNull(columnModification.OriginalParameterName, nameof(columnModification), "OriginalParameterName");
                parameter.ParameterName = columnModification.OriginalParameterName;
                parameter.Value = columnModification.OriginalValue ?? DBNull.Value;
            }
            else
            {
                Check.NotNull(columnModification.ParameterName, nameof(columnModification), "ParameterName");
                parameter.ParameterName = columnModification.ParameterName;
                parameter.Value = columnModification.Value ?? DBNull.Value;
            }

            ConfigureParameter(parameter, columnModification);

            return parameter;
        }

        protected virtual void ConfigureParameter([NotNull] DbParameter parameter, [NotNull] ColumnModification columnModification)
        {
            Check.NotNull(parameter, nameof(parameter));
            Check.NotNull(columnModification, nameof(columnModification));

            parameter.DbType = StoreType;
            parameter.IsNullable = columnModification.Property.IsNullable;
        }
    }
}

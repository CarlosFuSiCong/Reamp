using Microsoft.EntityFrameworkCore;
using Reamp.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySoftDeleteQueryFilters(this ModelBuilder builder)
        {
            var softDeletableTypes = builder.Model.GetEntityTypes()
                .Where(t => typeof(ISoftDeletable).IsAssignableFrom(t.ClrType)
                            && !t.IsOwned());

            foreach (var et in softDeletableTypes)
            {
                var clrType = et.ClrType;
                var parameter = Expression.Parameter(clrType, "e");

                var efPropertyMethod = typeof(EF).GetMethod(nameof(EF.Property))!
                    .MakeGenericMethod(typeof(DateTime?));
                var deletedAtExpr = Expression.Call(
                    efPropertyMethod,
                    parameter,
                    Expression.Constant(nameof(ISoftDeletable.DeletedAtUtc))
                );

                var body = Expression.Equal(
                    deletedAtExpr,
                    Expression.Constant(null, typeof(DateTime?))
                );

                var lambda = Expression.Lambda(body, parameter);
                builder.Entity(clrType).HasQueryFilter(lambda);
            }
        }
    }
}

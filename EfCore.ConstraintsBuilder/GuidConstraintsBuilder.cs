﻿using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.ConstraintsBuilder;

public sealed class GuidConstraintsBuilder<TEntity>  where TEntity : class
{
  private readonly EntityTypeBuilder<TEntity> _builder;
  private readonly SqlServerProvider _serverProvider;

  private readonly string _columnName;
  private readonly string _tableName;

  internal GuidConstraintsBuilder(
    EntityTypeBuilder<TEntity> builder,
    PropertyInfo propertyInfo,
    SqlServerProvider serverProvider) {
    var isDataTypeMatch = propertyInfo.PropertyType == typeof(Guid) ||
                          propertyInfo.PropertyType == typeof(Guid?);
    if (!isDataTypeMatch) {
      throw new ArgumentException("Property type is not datetime. PropertyName: " + propertyInfo.Name, nameof(propertyInfo));
    }

    _builder = builder;
    _serverProvider = serverProvider;
    _tableName = _builder.Metadata.GetTableName() ?? typeof(TEntity).Name;
    _columnName = _builder.Metadata.GetProperty(propertyInfo.Name).GetColumnName();
  }
  
  public GuidConstraintsBuilder<TEntity> NotNull() => NotNull(_builder.CreateUniqueConstraintName(_columnName, nameof(NotNull)));
  
  public GuidConstraintsBuilder<TEntity> NotNull(string uniqueConstraintName) {
    _builder.ToTable(x => x.HasCheckConstraint(uniqueConstraintName, $"[{_columnName}] IS NOT NULL"));
    return this;
  }
  
  public GuidConstraintsBuilder<TEntity> NotEmpty() => NotEmpty(_builder.CreateUniqueConstraintName(_columnName, nameof(NotEmpty)));
  
  public GuidConstraintsBuilder<TEntity> NotEmpty(string uniqueConstraintName) {
    _builder.ToTable(x => x.HasCheckConstraint(uniqueConstraintName, $"[{_columnName}] <> '00000000-0000-0000-0000-000000000000'"));
    return this;
  }
  
  public GuidConstraintsBuilder<TEntity> Empty() => Empty(_builder.CreateUniqueConstraintName(_columnName, nameof(Empty)));
  
  public GuidConstraintsBuilder<TEntity> Empty(string uniqueConstraintName) {
    _builder.ToTable(x => x.HasCheckConstraint(uniqueConstraintName, $"[{_columnName}] = '00000000-0000-0000-0000-000000000000'"));
    return this;
  }
  
  public GuidConstraintsBuilder<TEntity> NotEmptyOrNull() => NotEmptyOrNull(_builder.CreateUniqueConstraintName(_columnName, nameof(NotEmptyOrNull)));
  
  public GuidConstraintsBuilder<TEntity> NotEmptyOrNull(string uniqueConstraintName) {
    _builder.ToTable(x => x.HasCheckConstraint(uniqueConstraintName, $"([{_columnName}] IS NOT NULL AND [{_columnName}] <> '00000000-0000-0000-0000-000000000000') OR [{_columnName}] IS NULL"));
    return this;
  }
  
  public GuidConstraintsBuilder<TEntity> EmptyOrNull() => EmptyOrNull(_builder.CreateUniqueConstraintName(_columnName, nameof(EmptyOrNull)));
  
  public GuidConstraintsBuilder<TEntity> EmptyOrNull(string uniqueConstraintName) {
    _builder.ToTable(x => x.HasCheckConstraint(uniqueConstraintName, $"([{_columnName}] IS NULL OR [{_columnName}] = '00000000-0000-0000-0000-000000000000')"));
    return this;
  }
  public GuidConstraintsBuilder<TEntity> EqualsProperty(Expression<Func<TEntity, Guid>> propertySelector) => EqualsProperty(_builder.CreateUniqueConstraintName(_columnName, nameof(EqualsProperty)), propertySelector);
  
  public GuidConstraintsBuilder<TEntity> EqualsProperty(string constraintName, Expression<Func<TEntity, Guid>> propertySelector) {
    if (string.IsNullOrEmpty(constraintName)) throw new ArgumentNullException(nameof(constraintName));
    var propertyInfo = propertySelector.GetPropertyAccess();
    var compareColName = _builder.Metadata.GetProperty(propertyInfo.Name).GetColumnName();
    _builder.ToTable(x => x.HasCheckConstraint(constraintName, $"[{_columnName}] = [{_tableName}].[{compareColName}]"));
    return this;
  }
  
  public GuidConstraintsBuilder<TEntity> NotEqualsProperty(Expression<Func<TEntity, Guid>> propertySelector) => NotEqualsProperty(_builder.CreateUniqueConstraintName(_columnName, nameof(NotEqualsProperty)), propertySelector);
  public GuidConstraintsBuilder<TEntity> NotEqualsProperty(string uniqueConstraintName, Expression<Func<TEntity, Guid>> propertySelector) {
    if (string.IsNullOrEmpty(uniqueConstraintName)) throw new ArgumentNullException(nameof(uniqueConstraintName));
    var propertyInfo = propertySelector.GetPropertyAccess();
    var compareColName = _builder.Metadata.GetProperty(propertyInfo.Name).GetColumnName();
    _builder.ToTable(x => x.HasCheckConstraint(uniqueConstraintName, $"[{_columnName}] <> [{_tableName}].[{compareColName}]"));
    return this;
  }
  
  public GuidConstraintsBuilder<TEntity> EqualsValue(Guid value) => EqualsValue(_builder.CreateUniqueConstraintName(_columnName, nameof(EqualsValue)), value);
  
  public GuidConstraintsBuilder<TEntity> EqualsValue(string uniqueConstraintName, Guid value) {
    if (string.IsNullOrEmpty(uniqueConstraintName)) throw new ArgumentNullException(nameof(uniqueConstraintName));
    _builder.ToTable(x => x.HasCheckConstraint(uniqueConstraintName, $"[{_columnName}] = '{value}'"));
    return this;
  }
  
  public GuidConstraintsBuilder<TEntity> NotEqualsValue(Guid value) => NotEqualsValue(_builder.CreateUniqueConstraintName(_columnName, nameof(NotEqualsValue)), value);
  
  public GuidConstraintsBuilder<TEntity> NotEqualsValue(string uniqueConstraintName, Guid value) {
    if (string.IsNullOrEmpty(uniqueConstraintName)) throw new ArgumentNullException(nameof(uniqueConstraintName));
    _builder.ToTable(x => x.HasCheckConstraint(uniqueConstraintName, $"[{_columnName}] <> '{value}'"));
    return this;
  }
  
  public GuidConstraintsBuilder<TEntity> EqualsOneOf(IEnumerable<Guid> values) => EqualsOneOf(_builder.CreateUniqueConstraintName(_columnName, nameof(EqualsOneOf)), values);
  
  public GuidConstraintsBuilder<TEntity> EqualsOneOf(string uniqueConstraintName, IEnumerable<Guid> values) {
    if (string.IsNullOrEmpty(uniqueConstraintName)) throw new ArgumentNullException(nameof(uniqueConstraintName));
    var valuesString = string.Join(',', values.Select(x => $"'{x}'"));
    _builder.ToTable(x => x.HasCheckConstraint(uniqueConstraintName, $"[{_columnName}] IN ({valuesString})"));
    return this;
  }
  
  public GuidConstraintsBuilder<TEntity> NotEqualsOneOf(IEnumerable<Guid> values) => NotEqualsOneOf(_builder.CreateUniqueConstraintName(_columnName, nameof(NotEqualsOneOf)), values);
  
  public GuidConstraintsBuilder<TEntity> NotEqualsOneOf(string uniqueConstraintName, IEnumerable<Guid> values) {
    if (string.IsNullOrEmpty(uniqueConstraintName)) throw new ArgumentNullException(nameof(uniqueConstraintName));
    var valuesString = string.Join(',', values.Select(x => $"'{x}'"));
    _builder.ToTable(x => x.HasCheckConstraint(uniqueConstraintName, $"[{_columnName}] NOT IN ({valuesString})"));
    return this;
  }
  
}
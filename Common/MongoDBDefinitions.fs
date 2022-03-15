namespace EscortBookClaim.Common

open MongoDB.Driver
open System
open System.Linq.Expressions
open System.Collections.Generic
open System.Linq
open EscortBookClaim.Types
open EscortBookClaim.Constants
open EscortBookClaim.Extensions.StringExtensions

type MongoDBDefinitions<'a> private () =

    static member BuildFilter(filters: string) =
        let builder =
            match String.IsNullOrEmpty(filters) with
            | true ->
                Builders<'a>.Filter.Empty
            | false ->
                let lambda = MongoDBDefinitions<'a>.GenerateFilter(filters)
                Builders<'a>.Filter.Where(lambda)

        builder

    static member BuildSortFilter(sort : string) =
        let filter =
            match String.IsNullOrEmpty(sort) with
            | true ->
                let field = FieldDefinition<'a>.op_Implicit("createdAt")
                Builders<'a>.Sort.Descending(field)
            | false ->
                let isAscending = sort.Contains('-')
                let property = if isAscending then sort.Split('-').Last() else sort

                if isAscending then Builders<'a>.Sort.Ascending(FieldDefinition<'a>.op_Implicit(property))
                else Builders<'a>.Sort.Descending(FieldDefinition<'a>.op_Implicit(property))

        filter

    static member GenerateFilter(keys : string) =
        let keyValues = keys.Split(',')
        let operators = keyValues.Select(Func<string, TypeOperator>(fun key -> key.ClassifyOperation()))
        let parameterExpression = Expression.Parameter(typeof<'a>, "entity")
        let lambda = MongoDBDefinitions<'a>.BuildExpression(parameterExpression, operators)

        lambda

    static member BuildExpression(expression : ParameterExpression, operators : IEnumerable<TypeOperator>) =
        let operations = new Dictionary<int, Expression>()
        let mutable counter = 1

        for i in operators do
            let constant = Expression.Constant(i.Value)
            let property = Expression.Property(expression, i.Key)
            operations.Add(counter, MongoDBDefinitions<'a>.GenerateTypeExpression(property, constant, i))
            counter <- counter + 1

        let lambda = Expression.Lambda<Func<'a, bool>>(operations.First().Value, expression)
        lambda

    static member GenerateTypeExpression(property : Expression, constant : Expression, item : TypeOperator) =
        let expression =
            match item.Operation with
            | Constants.Same ->
                Expression.Equal(property, constant)
            | Constants.NotSame ->
                Expression.NotEqual(property, constant)
            | Constants.Greather ->
                Expression.GreaterThan(property, constant)
            | Constants.GreaterThan ->
                Expression.GreaterThanOrEqual(property, constant)
            | Constants.Lower ->
                Expression.LessThan(property, constant)
            | _ ->
                Expression.LessThanOrEqual(property, constant)

        expression

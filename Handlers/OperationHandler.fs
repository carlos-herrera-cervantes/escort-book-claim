namespace EscortBookClaim.Handlers

open System
open System.Reactive.Subjects
open System.Collections.Generic

type OperationHandler<'a> () =
    
    let subject = new Subject<'a>()

    let subscribers = new Dictionary<string, IDisposable>()

    interface IOperationHandler<'a> with
        
        member this.Publish(eventType: 'a) = subject.OnNext(eventType)

        member this.Subscribe(subscriberName: string)(action: Action<'a>) =
            if (subscribers.ContainsKey(subscriberName) = false) then
                subscribers.Add(subscriberName, subject.Subscribe(action))

    interface IDisposable with
        
        member this.Dispose() =
            let isSubjectNotNul = subject <> null

            if (isSubjectNotNul) then
                subject.Dispose()

            for subscriber in subscribers do
                subscriber.Value.Dispose()
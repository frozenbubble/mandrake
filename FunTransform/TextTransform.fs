namespace Mandrake.FunTransform.Text

open Mandrake.Model

type TextTransformer() = 

    member this.InsertInsert(o) = o
    
    member this.InsertDelete() = ()

    member this.DeleteInsert() = ()

    member this.DeleteDelete() = ()

    interface ITransform with
        member this.Transform(o1: Operation, o2: Operation)=
            null

        member this.Clear()= ()

        member this.Transform(o: Operation)= ()

        
        

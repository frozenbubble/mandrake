namespace Mandrake.FunTransform.Text

open Mandrake.Model



type TextTransformer() = 

    member this.InsertInsert(o) = o
    
    member this.InsertDelete() = ()

    member this.DeleteInsert() = ()

    member this.DeleteDelete() = ()

//    member this.Transform (oa: Operation) (ob: Operation) = 
//        match oa, ob with 
//        | InsertOperation o1, InsertOperation o2 -> //transformation
//        | DeleteOperation o1, InsertOperation o2 -> //transformation
//        | InsertOperation o1, DeleteOperation o2 -> //transformation
//        | DeleteOperation o1, DeleteOperation o2 -> //transformation

    

//    interface ITransform with
//        member this.Transform(o1: Operation, o2: Operation)=
//            null
//
//        member this.Clear()= ()
//
//        member this.Transform(o: Operation)= ()

        
        

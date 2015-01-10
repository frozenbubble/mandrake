namespace Mandrake

open Mandrake.Model

module Transform = 

    type TextTransformer() = 
        
        let (|Insert | Delete | Unknown |) (o: Operation) = 
            match o with
            | :? InsertOperation as iop -> Insert iop
            | :? DeleteOperation as dop -> Delete dop
            | _ -> Unknown
        
        interface ITransform with
            member this.Transform(o1: Operation, o2: Operation) = 
                match o1, o2 with
                | Insert o1, Insert o2 -> null //this.TransformI(o1, o2)
                | Insert o1, Delete o2 -> null
                | Delete o1, Insert o2 -> null
                | Delete o1, Delete o2 -> null
                | _ -> raise(new TransformationException("Operations are not text operations, TextTransformer should not have been called"))

            member this.Clear() = ()

        member this.TransformI(o1: InsertOperation, o2: InsertOperation) = o1.Clone()
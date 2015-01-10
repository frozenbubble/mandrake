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
                let o' = o2.Clone() :?> Operation
                
                match o1, o' with
                | Insert o1, Insert o' -> this.Transform(o1, o')
                | Insert o1, Delete o' -> this.Transform(o1, o')
                | Delete o1, Insert o' -> this.Transform(o1, o')
                | Delete o1, Delete o' -> this.Transform(o1, o')
                | _ -> raise(new TransformationException("Operations are not text operations, TextTransformer should not have been called"))

                o'

        
        member this.Transform(o1: InsertOperation, o2: InsertOperation) =
            if o1.Position < o2.Position || (o1.Position = o2.Position && o1.CreatedAt < o2.CreatedAt)
            
            then o2.Position <- o2.Position + o1.Length           

        
        member this.Transform(o1: InsertOperation, o2: DeleteOperation) = 
            if o1.Position < o2.StartPosition then o2.StartPosition <- o2.StartPosition + o1.Literal.Length

            else if o1.Position >= o2.StartPosition then o2.EndPosition <- o2.EndPosition + o1.Length
            
        
        member this.Transform(o1: DeleteOperation, o2: InsertOperation) = 
            if o2.Position > o1.EndPosition then o2.Position <- o2.Position - o1.Length

            else if o2.Position >= o1.StartPosition then o2.Literal <- ""


        member this.Transform(oa: DeleteOperation, ob: DeleteOperation) =
            if oa.EndPosition < ob.StartPosition then
                ob.StartPosition <- ob.StartPosition + oa.Length
                ob.EndPosition <- ob.EndPosition + oa.Length

            else if ob.StartPosition >= oa.StartPosition && oa.EndPosition >= ob.StartPosition then ob.EndPosition <- ob.StartPosition

            else if oa.StartPosition >= ob.StartPosition && ob.EndPosition >= oa.StartPosition then ob.EndPosition <- ob.EndPosition - oa.Length

            else if ob.StartPosition < oa.EndPosition then ob.StartPosition <- ob.StartPosition + oa.EndPosition - ob.StartPosition

            else if oa.StartPosition < ob.EndPosition then ob.EndPosition <- ob.EndPosition - (ob.EndPosition - oa.StartPosition)